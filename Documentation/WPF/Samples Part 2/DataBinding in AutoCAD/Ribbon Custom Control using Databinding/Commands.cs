// (C) Copyright 2002-2009 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to 
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;
using System.ComponentModel;

using Autodesk.Windows;
using System.Windows;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(RibbonCustCtrlDataBinding.Commands))]

namespace RibbonCustCtrlDataBinding
{
  class CompanyDetails : INotifyPropertyChanged
  {
    // create a global instance of some data we are going to bind to
    // this needs to be public as we are binding to it directly from our 
    // ribbon control
    static public CompanyDetails details = new CompanyDetails();

    public CompanyDetails()
    {
      FirstName = "Fenton";
      LastName = "Webb";
      JobTitle = "Washer Woman";
      CompanyName = "Autodesk";
    }

    string firstName;
    public string FirstName
    {
      get { return firstName; }
      set { firstName = value; NotifyPropertyChanged("FirstName"); }
    }

    string lastName;
    public string LastName
    {
      get { return lastName; }
      set { lastName = value; NotifyPropertyChanged("LastName"); }
    }

    string jobTitle;
    public string JobTitle
    {
      get { return jobTitle; }
      set { jobTitle = value; NotifyPropertyChanged("JobTitle"); }
    }

    string companyName;
    public string CompanyName
    {
      get { return companyName; }
      set { companyName = value; NotifyPropertyChanged("CompanyName"); }
    }

    #region INotifyPropertyChanged Members
    // in order to get normal CLR properties to react 2 way, we need to wire in 
    // the PropertyChanged event handler - so when our property changes from the
    // commandLine input, the modeless dialog knows about it
    public event PropertyChangedEventHandler PropertyChanged;
    protected internal void NotifyPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }

  // This class is instantiated by AutoCAD for each document when
  // a command is called by the user the first time in the context
  // of a given document. In other words, non static data in this class
  // is implicitly per-document!
  public class Commands
  {
    // The CommandMethod attribute can be applied to any public  member 
    // function of any public class.
    // The function should take no arguments and return nothing.
    // If the method is an instance member then the enclosing class is 
    // instantiated for each document. If the member is a static member then
    // the enclosing class is NOT instantiated.
    //
    // NOTE: CommandMethod has overloads where you can provide helpid and
    // context menu.
    // command line input method for gaining the data

    [CommandMethod("testRibbon")]
    public void test() // This method can have any name
    {
      //enable contextual tab
      MyContextualRibbon.contextRibbon(true);

      Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
      // setup the options for the String input
      PromptStringOptions opts1 = new PromptStringOptions("Enter First Name");
      opts1.DefaultValue = CompanyDetails.details.FirstName;
      opts1.UseDefaultValue = true;

      // now get the input from the user via the commandline
      PromptResult res1 = ed.GetString(opts1);
      // now check the result, if ok
      if (res1.Status == PromptStatus.OK)
      {
        CompanyDetails.details.FirstName = res1.StringResult;
        // setup the options for the String input
        PromptStringOptions opts2 = new PromptStringOptions("Enter Last Name");
        opts2.DefaultValue = CompanyDetails.details.LastName;
        opts2.UseDefaultValue = true;

        // now get the input from the user via the commandline
        PromptResult res2 = ed.GetString(opts2);
        // now check the result, if ok
        if (res2.Status == PromptStatus.OK)
        {
          CompanyDetails.details.LastName = res2.StringResult;
          // setup the options for the String input
          PromptStringOptions opts3 = new PromptStringOptions("Enter Job Title");
          opts3.DefaultValue = CompanyDetails.details.JobTitle;
          opts3.UseDefaultValue = true;

          // now get the input from the user via the commandline
          PromptResult res3 = ed.GetString(opts3);
          // now check the result, if ok
          if (res3.Status == PromptStatus.OK)
          {
            CompanyDetails.details.JobTitle = res3.StringResult;
            // setup the options for the String input
            PromptStringOptions opts4 = new PromptStringOptions("Enter Company Name");
            opts4.DefaultValue = CompanyDetails.details.CompanyName;
            opts4.UseDefaultValue = true;

            // now get the input from the user via the commandline
            PromptResult res4 = ed.GetString(opts4);
            // now check the result, if ok
            if (res4.Status == PromptStatus.OK)
            {
              CompanyDetails.details.CompanyName = res4.StringResult;
            }
          }
        }
      }

      //disable contextual ribbon
      MyContextualRibbon.contextRibbon(false);
    }

    [CommandMethod("createRibbon")]
    public static void createRibbon()
    {
      MyContextualRibbon.createRibbon();
    }

  }

  public class MyContextualRibbon
  {
    #region Contextual Ribbon

    /// <summary>
    /// Control function for contextual
    /// </summary>
    /// <param name="enable">TRUE to enable ribbon, FALSE to disable</param>
    public static void contextRibbon(bool enable)
    {
      //runtime
      if (ribContextualTab != null)
      {
        ribContextualTab.IsVisible = enable;
        ribContextualTab.IsActive = enable;
      }
    }

    #region Create a ribbon tab on runtime
    //create the variable as static for further reference
    private static RibbonTab ribContextualTab;

    public static void createRibbon()
    {
      if (ribContextualTab != null) return; //already created

      //get the active ribbon control (NOTE: do not call on Initialize, see TS88988)
      RibbonControl ribCntrl = Autodesk.AutoCAD.Ribbon.RibbonServices.RibbonPaletteSet.RibbonControl;

      //create a new RibbonTab and add to ribbon control
      ribContextualTab = new RibbonTab();
      ribContextualTab.Title = "Fentons Contextual tab";
      ribContextualTab.Id = "MY_TAB_ID-Fenton";
      ribContextualTab.IsContextualTab = true;
      ribContextualTab.IsVisible = false;
      ribCntrl.Tabs.Add(ribContextualTab);

      //create a new RibbonPanel
      //first the panelsource
      RibbonPanelSource ribSourcePanel = new RibbonPanelSource();
      ribSourcePanel.Title = "My Panel";
      //now the panel
      RibbonPanel ribPanel = new RibbonPanel();
      ribPanel.Source = ribSourcePanel;
      ribContextualTab.Panels.Add(ribPanel);

      Uri uri = new Uri("/RibbonCustCtrlDataBinding;component/MyRibbonControl.xaml", System.UriKind.Relative);
      ResourceDictionary resDict = (ResourceDictionary)System.Windows.Application.LoadComponent(uri);
      ribSourcePanel.Items.Add((RibbonItem)resDict["fentonsSimpleDataBinding"]);
    }

    #endregion
    #endregion
  }
}
