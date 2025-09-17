using System.IO;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using AcApp = ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Runtime;
#else
using AcApp = Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
#endif
#endregion

using Shared.Controllers;

[assembly: CommandClass(typeof(Shared.Commands.Commands))]
namespace Shared.Commands
{
    internal class Commands
    {
        [CommandMethod("RP_PROSPECTOR_SOLUTION", CommandFlags.Session)]
        public void __rkqopzla947()
        {
            AcApp.Document _document = AcApp.Application.DocumentManager.MdiActiveDocument;
            if (_document != null)
            {
                // https://keanw.com/2007/08/using-autocads-.html
                // https://keanw.com/2007/03/replacing_the_o.html
                // https://help.autodesk.com/view/OARX/2025/ENU/?guid=OARX-ManagedRefGuide-Autodesk_AutoCAD_EditorInput_PromptOpenFileOptions
                PromptOpenFileOptions options = new PromptOpenFileOptions("Select a .rpsol solution file:")
                {
                    // https://www.sqlnethub.com/blog/how-to-set-filters-for-openfiledialog-and-savefiledialog-in-c-sharp/
                    Filter = "Solution (*.rpsol)|*.rpsol"
                };
                PromptFileNameResult result = _document.Editor.GetFileNameForOpen(options);
                if (result.Status == PromptStatus.OK)
                {
                    string lsPath = Path.GetDirectoryName(result.StringResult);
                    string lsFile = Path.GetFileName(result.StringResult);
                    RPApp.FileWatcher.AddDirectory(lsPath);
                    ProjectController.RPConfig config = ConfigController.LoadConfig<ProjectController.RPConfig>(lsFile, lsPath);
                    if (config != null)
                    {
                        _document.Editor.WriteMessage(string.Format("[&] Changed current project to: {0}\n", config?.LastRoute));
                        try { RPApp.RDPHelper.CreateConfigRDP(lsPath, config?.LastRoute); }
                        catch { }
                    }
                }
            }
        }
    }
}