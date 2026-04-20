using business;
using System;
using System.Collections.Generic;
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
using Models;
using business;

namespace ui
{
    /// <summary>
    /// Interaction logic for ResultScreen.xaml
    /// </summary>
    public partial class ResultScreen : UserControl
    {
        private BusinessLayer _business;
        private int _subjectId;

        public ResultScreen(BusinessLayer business, int subjectId)
        {
            InitializeComponent();

            _business = business;
            _subjectId = subjectId;

            LoadResults();
        }

        private void LoadResults()
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lvComparison_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
