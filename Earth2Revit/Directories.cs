using System;
using System.IO;

namespace Earth2Revit {
  class Directories {
    public static string resources;
    public static string contents;
    public const string appName = "Earth2Revit";

    static public void initialize(string revitVersion) {
      contents = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Autodesk\Revit\Addins\" + revitVersion + @"\" + appName + @".bundle\");

      if(!Directory.Exists(contents))
        contents = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Autodesk\ApplicationPlugins\" + appName + @".bundle\");

      if(!Directory.Exists(contents))
        contents = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Autodesk\ApplicationPlugins\" + appName + @".bundle\");

      if(!Directory.Exists(contents))
        throw new Exception("Couldn't find installation directory");

      contents = Path.Combine(contents, @"Contents");
      resources = Path.Combine(contents, @"Resources");

      if(Directory.Exists(Path.Combine(contents, revitVersion)))
        contents = Path.Combine(contents, revitVersion);
    }
  }
}
