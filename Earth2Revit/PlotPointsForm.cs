using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Creation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Earth2Revit {
    public partial class PlotPointsForm : System.Windows.Forms.Form {
        private readonly Autodesk.Revit.DB.Document doc;
        public PlotPointsForm(Autodesk.Revit.DB.Document doc) {
            InitializeComponent();

            this.doc = doc;
        }

        private void browseButton_Click(object sender, EventArgs e) {
            if (this.openFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                this.fileBox.Text = this.openFileDialog.FileName;
        }

        private Placemark loadPlacemark(XElement placemark) {
            Placemark geom = new Placemark();
            foreach (var property in placemark.Elements()) {
                if (property.Name.LocalName == "name") {
                    geom.name = property.Name.LocalName;
                }
                else if (property.Name.LocalName == "Polygon") {
                    foreach (var polyProperty in property.Elements()) {
                        if (polyProperty.Name.LocalName == "outerBoundaryIs") {
                            foreach (var boundaryProperty in polyProperty.Elements()) {
                                if (boundaryProperty.Name.LocalName == "LinearRing") {
                                    foreach (var lineProperty in boundaryProperty.Elements()) {
                                        if (lineProperty.Name.LocalName == "coordinates") {
                                            var coordinates = lineProperty.Value.Split(' ');
                                            foreach (var coordinate in coordinates) {
                                                var coord = coordinate.Split(',');
                                                if (coord.Length == 3)
                                                    geom.coordinates.Add(new XYZ(double.Parse(coord[0]), double.Parse(coord[1]), double.Parse(coord[2])));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return geom;
        }

        private List<Placemark> getPlacemarks(string filename) {
            XmlTextReader reader = new XmlTextReader(filename);

            while (reader.NodeType != XmlNodeType.Element)
                reader.Read();

            List<Placemark> placemarks = new List<Placemark>();

            XElement xml = XElement.Load(reader);
            foreach (var document in xml.Elements()) {
                if (document.Name.LocalName == "Document") {
                    foreach (var placemark in document.Elements()) {
                        if (placemark.Name.LocalName == "Placemark") {
                            placemarks.Add(loadPlacemark(placemark));
                        }
                        else if (placemark.Name.LocalName == "Folder") {
                            foreach (var placemark2 in placemark.Elements()) {
                                if (placemark2.Name.LocalName == "Placemark") {
                                    placemarks.Add(loadPlacemark(placemark2));
                                }
                            }
                        }
                    }
                }
            }

            return placemarks;
        }

        private static XYZ haversine(XYZ start, XYZ end) {
            XYZ ret = start - end;
            if (ret.GetLength() < 0.0000000001 && ret.GetLength() > -0.0000000001)
                return new XYZ(0, 0, 0);

            var earthRadius = 6371000; // metres
            var degreesToRadians = Math.PI / 180;

            var lat1 = start.Y * degreesToRadians;
            var lat2 = end.Y * degreesToRadians;
            var long1 = start.X * degreesToRadians;
            var long2 = end.X * degreesToRadians;

            var Δlat = lat2 - lat1;
            var Δlong = long2 - long1;

            var a = Math.Sin(Δlat / 2) * Math.Sin(Δlat / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Sin(Δlong / 2) * Math.Sin(Δlong / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            ret = ret.Normalize();
            ret = ret.Multiply(earthRadius * c);

            return ret;
        }

        private void loadButton_Click(object sender, EventArgs e) {
            if (this.fileBox.Text.Length > 0) {
                List<Placemark> placemarks = getPlacemarks(this.fileBox.Text);

                XYZ offset = new XYZ();
                var numcoordinates = 0;

                foreach (var placemark in placemarks)
                    foreach (var coordinate in placemark.coordinates) {
                        numcoordinates++;
                        offset += coordinate;
                    }
                offset /= numcoordinates;
                Transaction trans = new Transaction(this.doc);
                trans.Start("Plot Points");
                foreach (var placemark in placemarks) {
                    if (placemark.coordinates.Count <= 1)
                        continue;
                    if(offset == null)
                        offset = placemark.coordinates[0];
                    for (var i = 0; i < placemark.coordinates.Count - 1; ++i) {
                        XYZ start = placemark.coordinates[i];
                        XYZ end = placemark.coordinates[i + 1];
                        var startX = new XYZ(start.X, offset.Y, offset.Z);
                        var startY = new XYZ(offset.X, start.Y, offset.Z);
                        var endX = new XYZ(end.X,offset.Y, offset.Z);
                        var endY = new XYZ(offset.X, end.Y, offset.Z);


                        endX = haversine(endX, offset);
                        endY = haversine(endY, offset);
                        startX = haversine(startX, offset);
                        startY = haversine(startY, offset);
                        start = new XYZ(startX.X, startY.Y, start.Z);
                        end = new XYZ(endX.X, endY.Y, end.Z);
                        XYZ origin = new XYZ(0, 0, 0);
                        XYZ normal = new XYZ(0, 0, 1);

                        var app = this.doc.Application.GetType();
                        var createProperty = app.GetProperty("Create");

                        if(createProperty != null) {
                            var create = createProperty.GetGetMethod().Invoke(this.doc.Application, new object[] {});
                            var createNewLineMethod = create.GetType().GetMethod("NewLine");
                            var createBoundMethod = typeof(Line).GetMethod("CreateBound");

                            if(createNewLineMethod != null) {
                                // Line line = this.doc.Application.Create.NewLine(start, end, true);
                                Line line = (Line)createNewLineMethod.Invoke(create, new object[] { start, end, true });

                                // Create a geometry plane in Revit application
                                Plane geomPlane = this.doc.Application.Create.NewPlane(normal, origin);

                                // Create a sketch plane in current document
                                //SketchPlane sketch = this.doc.Create.NewSketchPlane(geomPlane);
                                var docCreate = this.doc.Create;
                                var createNewSketchPlaneMethod = docCreate.GetType().GetMethod("NewSketchPlane", new [] { typeof(Plane) });
                                SketchPlane sketch = (SketchPlane)createNewSketchPlaneMethod.Invoke(docCreate, new object[] { geomPlane });

                                this.doc.Create.NewModelCurve(line, sketch);
                            }
                            else if (createBoundMethod != null) {
                                //Line.CreateBound(start, end);
                                Line line = (Line)createBoundMethod.Invoke(null, new object[] { start, end });
                                Plane geomPlane = this.doc.Application.Create.NewPlane(normal, origin);

                                var createNewSketchPlaneMethod = typeof(SketchPlane).GetMethod("Create", new Type[] { typeof(Autodesk.Revit.DB.Document), typeof(Plane) });
                                //var sketch = SketchPlane.Create(this.doc, geomPlane);
                                var sketch = (SketchPlane)createNewSketchPlaneMethod.Invoke(null, new object[] { this.doc, geomPlane });
                                this.doc.Create.NewModelCurve(line, sketch);
                            }
                        }
                    }
                }
                trans.Commit();
            }
            /*
            BuildingSiteExportOptions options = new BuildingSiteExportOptions();
            PropertyLine line = options.PropertyLine;
            MessageBox.Show(line.Id.ToString(), "PlotPoints", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);

            Transaction trans = new Transaction(doc);
            trans.Start("Lab");
            trans.Commit();*/
        }
    }
    public class Placemark {
        public string name;
        public List<XYZ> coordinates;
        public Placemark(string name, List<XYZ> coordinates) {
            this.name = name;
            this.coordinates = coordinates;
        }
        public Placemark() {
            this.name = "";
            this.coordinates = new List<XYZ>();
        }
    }
}
