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

namespace VerySimpleDatabinding
{
  /// <summary>
  /// Interaction logic for UserControl1.xaml
  /// </summary>
  public partial class UserControl1 : UserControl
  {
    Object mydata;
    public Object MyData
    {
      get { return mydata; }
      set { mydata = value; }
    } 

    public UserControl1(Object obj)
    {
      MyData = obj;
      InitializeComponent();
    }
  }
}
