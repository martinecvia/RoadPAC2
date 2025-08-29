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
using System.Windows.Shapes;

namespace DataBindingToAcadCLR
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class MyModalWindow : Window, IDisposable
  {
    public MyModalWindow()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(MyModalWindow_Loaded);
    }

    void MyModalWindow_Loaded(object sender, RoutedEventArgs e)
    {
      if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
        m_layers.LoadLayerModal();
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }
    public void Commit()
    {
      m_layers.Commit();
    }
    public void Dispose()
    {
      m_layers.Dispose();
    }
  }
}
