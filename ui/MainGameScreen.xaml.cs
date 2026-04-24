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
using persistentie;

namespace ui
{
    public partial class MainGameScreen : UserControl
    {
        private Business _business;
        private PersistenceObject _persistence;
        private subjectItem[] _currentOptions;
        private int _subjectId;

        public MainGameScreen(int subjectId)
        {
            InitializeComponent();

            _subjectId = subjectId;
            _persistence = new PersistenceObject();

            //BusinessLayer krijgt subject id en persistence pointer mee, zorgt dat business layer weet welk thema er gerankt wordt
            _business = new Business(_subjectId, _persistence);

            LoadFilters();
            NextRound();
        }

        private void NextRound()
        {
            //Vraag de volgende 2 opties op van business laag
            _currentOptions = _business.Give_options();

            //CHecken of ranking klaar is
            if (_currentOptions == null || _currentOptions.Length < 2)
            {
                ShowResults();
                return;
            }

            //koppelt tekst en foto aan de ui elementen
            textA.Text = _currentOptions[0].Text[0];
            imageA.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + _currentOptions[0].Image, UriKind.Absolute));

            textB.Text = _currentOptions[1].Text[0];
            imageB.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + _currentOptions[1].Image, UriKind.Absolute));

            UpdateProgress();
        }

        private void UpdateProgress()
        {
            //vooruitgang van de ranking
        }

        private void btnOptionA_Click(object sender, RoutedEventArgs e)
        {
            //Stuurt keuze naar business laag: optie A winnaar (index 0), optie B verliezer (index 1)
            subjectItem[] result = { _currentOptions[0], _currentOptions[1] };
            _business.Give_result(result);
            NextRound();
        }

        private void btnOptionB_Click(object sender, RoutedEventArgs e)
        {
            //Stuurt keuze naar business laag: optie B winnaar (index 0), optie A verliezer (index 1)
            subjectItem[] result = { _currentOptions[1], _currentOptions[0] };
            _business.Give_result(result);
            NextRound();
        }

        private void btnTie_Click(object sender, RoutedEventArgs e)
        {
            NextRound();
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            //Undo functie business laag oproepen
        }

        private void LoadFilters()
        {
            //Haalt alle items op via de persistence pointer om de categorieen te bepalen
            var items = _persistence.Get_subjectItems(_subjectId);
            //Haalt alle categorieen op en filtert dubbels weg
            var categories = items.SelectMany(i => i.Text).Distinct.Skip(1);

            foreach (var category in categories)
            {
                //Maakt voor elke categorie een nieuwe checkbox aan
                CheckBox cb = new CheckBox {
                    Content = category, 
                    Margin = new Thickness(5), 
                    IsChecked = true
                };
                wpFilters.Children.Add(cb);
            }

        }

        private void ShowResults()
        {
            //Gebruikt functie om over te schakelen naar ResultScreen
            MainWindow parent = (MainWindow)Window.GetWindow(this);
            if (parent != null)
            {
                parent.ShowResults(_business, _subjectId);
            }
        }
    }
}
