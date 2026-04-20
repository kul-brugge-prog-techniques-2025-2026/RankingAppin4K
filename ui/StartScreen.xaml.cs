using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
    /// Interaction logic for StartScreen.xaml
    /// </summary>
    public partial class StartScreen : UserControl
    {
        //referentie pointer naar business laag
        private BusinessLayer _business;

        public StartScreen()
        {
            InitializeComponent();

            //init bussiness logica
            _business = new BusinessLayer();

            LoadCategories();
        }

        private void LoadCategories()
        {
            //Vraag lijst van subject-objecten op via business laag
            List<Subject> subjects = _business.Give_all_subjects();
            
            //Koppelt de opgehaalde lijst aan de listbox
            lbSubjects.ItemsSource = subjects;
        }

        private void lbSubjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Controleert of het geselecteerde item van het type subject is
            if (lbSubjects.SelectedItem is Subject selectedSubject)
            {
                //Haalt reff op naar MainWindow
                MainWindow parentWindow = (MainWindow)Window.GetWindow(this);

                if (parentWindow != null)
                {
                    //Geeft id door aan MainWindow om naar MainGame state te gaan
                    parentWindow.StartRanking(selectedSubject.Id);
                }
            }
        }
    }
}
