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

namespace cMud2.UI
{
    /// <summary>
    /// Interaction logic for TestMessageWindow.xaml
    /// </summary>
    public partial class TestMessageWindow : Window
    {
        public string Message
        { get; set; }

        public TestMessageWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Message = tbxMain.Text;
            this.Close();
        }
    }
}
