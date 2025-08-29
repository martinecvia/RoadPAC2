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
  /// Interaction logic for MyModelessWindow.xaml
  /// </summary>
  public partial class MyModelessWindow : Window
  {
    public MyModelessWindow()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(MyModelessWindow_Loaded);
    }

    void MyModelessWindow_Loaded(object sender, RoutedEventArgs e)
    {
      if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
        m_layers.LoadLayers2();
    }
  }

}
