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
using System.IO;
using System.Diagnostics;

namespace Earth2Revit {
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
                var pushButton = ribbonPanel.AddItem(new PushButtonData("PlotPoints", "Plot Points", Path.Combine(Directories.contents, @"Earth2Revit.dll"), "Earth2Revit.PlotPoints")) as PushButton;

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