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
using System.Windows;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(ModelessDependencyProperty.Commands))]

namespace ModelessDependencyProperty
{
  class CompanyDetails : DependencyObject
  {
    // create a global instance of some data we are going to bind to
    static internal CompanyDetails details = new CompanyDetails();

    public CompanyDetails()
    {
      firstName = DependencyProperty.Register("FirstName", typeof(string), typeof(CompanyDetails), new PropertyMetadata("Fenton"));
      lastName = DependencyProperty.Register("LastName", typeof(string), typeof(CompanyDetails), new PropertyMetadata("Webb"));
      jobTitle = DependencyProperty.Register("JobTitle", typeof(string), typeof(CompanyDetails), new PropertyMetadata("Washer Woman"));
      companyName = DependencyProperty.Register("CompanyName", typeof(string), typeof(CompanyDetails), new PropertyMetadata("Autodesk"));
    }

    public DependencyProperty firstName;
    public string FirstName
    {
      get { return (string)this.GetValue(firstName); }
      set { this.SetValue(firstName, value); }
    }

    public DependencyProperty lastName;
    public string LastName
    {
      get { return (string)this.GetValue(lastName); }
      set { this.SetValue(lastName, value); }
    }

    public DependencyProperty jobTitle;
    public string JobTitle
    {
      get { return (string)this.GetValue(jobTitle); }
      set { this.SetValue(jobTitle, value); }
    }

    public DependencyProperty companyName;
    public string CompanyName
    {
      get { return (string)this.GetValue(companyName); }
      set { this.SetValue(companyName, value); }
    }

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
    // If the method is an intance member then the enclosing class is 
    // intantiated for each document. If the member is a static member then
    // the enclosing class is NOT intantiated.
    //
    // NOTE: CommandMethod has overloads where you can provide helpid and
    // context menu.
    // command line input method for gaining the data

    [CommandMethod("testDependency")]
    public void test() // This method can have any name
    {
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
    }

    static PaletteSet ps = null;
    static VerySimpleDatabinding.UserControl1 userControl = null;
    // command line input method for gaining the data
    [CommandMethod("dependency")]
    public void modeless() // This method can have any name
    {
      // check to see if the ps is already up and running, if not
      if (ps == null)
      {
        // create it
        ps = new PaletteSet("MyPalette");
        userControl = new VerySimpleDatabinding.UserControl1(CompanyDetails.details);
        ps.AddVisual("Details", userControl);
        // as it's a small palette let's make it floating
        ps.DockEnabled = DockSides.None;
      }

      ps.Visible = true;
    }
  }
}
