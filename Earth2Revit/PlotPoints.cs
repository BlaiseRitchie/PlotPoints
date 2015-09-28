using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Earth2Revit {
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
}
