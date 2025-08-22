using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Controllers;
using Shared.Controllers.Models.RibbonXml;
using Shared.Controllers.Models.RibbonXml.Items;
using Shared.Controllers.Models.RibbonXml.Items.CommandItems;
using ZwSoft.Windows;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Runtime;

[assembly: CommandClass(typeof(ZWC_47_TEST.TestEx))]
namespace ZWC_47_TEST
{ 
    public class TestEx : IExtensionApplication
    {
        private static RibbonControl Ribbon => ComponentManager.Ribbon;

        public void Initialize()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            ResourceController.LoadEmbeddedResources(); // To load icons, configuration files etc.
            RibbonController.CreateTab("rp_RoadPAC");
            RibbonController.CreateContextualTab("rp_Contextual_SelectView", selection =>
            {
                return true;
            });
        }

        public void Terminate()
        { }

        [CommandMethod("HIT_BREAKPOINT")]
        public void Breakpoint()
        {
            var Ribbons = Ribbon;
        }

        RibbonTab _ctxTab = null;

        [CommandMethod("CtxTabUponSelect")]

        public void CtxTabUponSelect()

        {

            Document doc = Application.DocumentManager.MdiActiveDocument;



            //Set up event for selection changed

            doc.ImpliedSelectionChanged +=

                new EventHandler(doc_ImpliedSelectionChanged);

        }



        void doc_ImpliedSelectionChanged(object sender, EventArgs e)

        {

            Document doc = Application.DocumentManager.MdiActiveDocument;

            PromptSelectionResult psr = doc.Editor.SelectImplied();



            //if no entities are selected, we hide our context tab

            if (psr.Value == null)

            {

                HideTab();

                return;

            }



            //In this example we only display the tab if only circles are

            // selected. You may want to change this condition of course.

            foreach (SelectedObject selObj in psr.Value)

            {

                if (selObj.ObjectId.ObjectClass.DxfName.ToLower()

                    != "circle")

                {

                    HideTab();

                    return;

                }

            }



            //We will use the Application.Idle event to safely display our tab

            if (_ctxTab == null || !_ctxTab.IsVisible)

            {

                ZwSoft.ZwCAD.ApplicationServices.Application.Idle +=

                    new EventHandler(OnIdle);

            }

        }



        void OnIdle(object sender, EventArgs e)

        {

            //Make sure ribbon manager is available

            if (ComponentManager.Ribbon != null)

            {

                //Create tab if it doesn't exist

                if (_ctxTab == null)

                    CreateCtxTab();



                //Otherwise make it visible

                if (!_ctxTab.IsVisible)

                {

                    RibbonControl ribbonCtrl = ComponentManager.Ribbon;



                    ribbonCtrl.ShowContextualTab(_ctxTab, false, true);



                    _ctxTab.IsActive = true;

                }



                if (!_ctxTab.IsActive)

                    _ctxTab.IsActive = true;

            }

        }



        //Tab creation method

        void CreateCtxTab()

        {

            RibbonControl ribbonCtrl = ComponentManager.Ribbon;



            _ctxTab = new RibbonTab();



            _ctxTab.Name = "MyTab";

            _ctxTab.Id = "MY_CTX_TAB_ID";

            _ctxTab.IsVisible = true;

            _ctxTab.Title = _ctxTab.Name;

            _ctxTab.IsContextualTab = true;



            ribbonCtrl.Tabs.Add(_ctxTab);

        }



        void HideTab()

        {

            ZwSoft.ZwCAD.ApplicationServices.Application.Idle

                -= new EventHandler(OnIdle);



            if (_ctxTab != null)

            {

                RibbonControl ribbonCtrl = ComponentManager.Ribbon;



                ribbonCtrl.HideContextualTab(_ctxTab);

                _ctxTab.IsVisible = false;

                _ctxTab = null;

            }

        }
    }
}
