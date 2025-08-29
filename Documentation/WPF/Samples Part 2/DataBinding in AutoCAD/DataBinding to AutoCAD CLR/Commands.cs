using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Runtime;

namespace DataBindingToAcadCLR
{
  public class Commands
  {
    [CommandMethod("modal")]
    public static void Modal()
    {
      // create a new window
      using (MyModalWindow dlg = new MyModalWindow())
      {
        // start it 
        bool ret = (bool)Autodesk.AutoCAD.ApplicationServices.Application.ShowModalWindow(dlg);
        // if the user didn't cancel
        if (ret)
        {
          // then commit all changes
          dlg.Commit();
        }
      }
    }

    [CommandMethod("modeless")]
    public static void Modeless()
    {
      // create a new instance
      MyModelessWindow dlg = new MyModelessWindow();
      // and show it
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessWindow(dlg);
    }
  }
}
