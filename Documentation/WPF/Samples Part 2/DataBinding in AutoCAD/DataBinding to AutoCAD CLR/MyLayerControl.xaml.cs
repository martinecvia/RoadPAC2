using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.AutoCAD.DatabaseServices;

namespace DataBindingToAcadCLR
{
  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class MyLayerControl : UserControl, IDisposable
  {
    public MyLayerControl()
    {
      InitializeComponent();
    }

    Transaction m_transaction;
    public void LoadLayerModal()
    {
      m_transaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction();
      ObjectId ltId = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.LayerTableId;
      LayerTable lt = (LayerTable)ltId.GetObject(OpenMode.ForRead);
      foreach (ObjectId id in lt)
        m_list.Items.Add(id.GetObject(OpenMode.ForWrite));
    }

    public void Commit()
    {
      if (m_transaction != null)
      {
        m_transaction.Commit();
        m_transaction = null;
      }
    }

    #region IDisposable Members

    public void Dispose()
    {
      if (m_transaction != null)
      {
        m_transaction.Dispose();
        m_transaction = null;
      }
    }
    public void LoadLayers2()
    {
      m_list.ItemsSource = new DataItemCollection(
          delegate { return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database.LayerTableId; });
    }
    #endregion
  }
}
