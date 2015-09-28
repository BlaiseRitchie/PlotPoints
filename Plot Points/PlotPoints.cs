using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.Diagnostics;
using System.IO;

namespace Plot_Points {
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class PlotPoints : IExternalCommand {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements) {
            //Get application and document objects
            UIApplication uiApp = commandData.Application;
            
            Document doc = uiApp.ActiveUIDocument.Document;

            var form = new PlotPointsForm(doc);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(new WindowHandle(Process.GetCurrentProcess().MainWindowHandle));
            form.Dispose();

            return Result.Succeeded;
        }
    }

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class AddButton : IExternalApplication {
        public Result OnStartup(UIControlledApplication application) {
            try {
                Directories.initialize(application.ControlledApplication.VersionNumber);
            }
            catch (Exception e) {
                var form = new ExceptionForm(e);
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog(new WindowHandle(Process.GetCurrentProcess().MainWindowHandle));
                form.Dispose();
                return Result.Failed;
            }

            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Earth2Revit");
            {
                var pushButton = ribbonPanel.AddItem(new PushButtonData("PlotPoints", "Plot Points", Path.Combine(Directories.contents, @"PlotPoints.dll"), "Plot_Points.PlotPoints")) as PushButton;

                var uriImage = new Uri(Path.Combine(Directories.resources, @"plotpoints.png"));
                var largeImage = new BitmapImage(uriImage);
                pushButton.LargeImage = largeImage;
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application) {
            return Result.Succeeded;
        }
    }

    class WindowHandle : IWin32Window {
        public IntPtr Handle { get; protected set; }

        public WindowHandle(IntPtr handle) {
            this.Handle = handle;
        }
    }
}