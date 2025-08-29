using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;

namespace DataBindingToAcadClrUIBindings
{
  public class Commands
  {
    [CommandMethod("modalUIBindings")]
    public static void Modal()
    {
      // create a new window
      MyModalWindow dlg = new MyModalWindow();
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModalWindow(dlg);
    }

    [CommandMethod("modelessUIBindings")]
    public static void Modeless()
    {
      // create a new instance
      MyModelessWindow dlg = new MyModelessWindow();
      // and show it
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessWindow(dlg);
    }
  }
}
