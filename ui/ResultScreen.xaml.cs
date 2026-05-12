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
        private Business _business;
        private int _subjectId;

        public ResultScreen(Business business, int subjectId)
        {
            InitializeComponent();

            _business = business;
            _subjectId = subjectId;

            LoadResults();
        }

        private void LoadResults()
        {
            //Vraagt de gesorteerde lijst op aan de business laag
            List<RankingItem> rankedItems = _business.GetFinalRankedList();

            //Maakt tijdelijke lijst voor objecten te tonen op ui
            var displayList = new List<RankedItemDisplay>();

            //Iteratie door lijst om data te mappen
            foreach (var item in rankedItems)
            {
                displayList.Add(new RankedItemDisplay
                {
                    Rank = item.Rank + 1,
                    Name = item.subjectitem.Text[0],
                    Image = item.subjectitem.Image
                });
            }
            //Koppel de gevulde lijst aan de Itemsource van de listview
            lvResultList.ItemsSource = displayList;

            //Roep de vergelijkings method op in de business laag
            //ComparedRankingResult[] comparisons = _business.Compare();
            //lvComparison.ItemsSource = comparisons;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Haalt de text uit de text box en trimt begin en einde
            string userName = txtUserName.Text.Trim();

            //Controle of gebruiker iets heeft ingevuld
            if (string.IsNullOrEmpty(userName) )
            {
                MessageBox.Show("Enter name before saving", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Stuurt de naam naar de business laag.
            _business.SaveCurrent(userName);

            MessageBox.Show($"Ranking saved succesfully for {userName}");
        }

        private void lvComparison_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvComparison.SelectedItem is ComparedRankingResult selectedMatch)
            {
                //Vernieuwt de onderste lijst met een lijst van andere user
                lvMatchResults.ItemsSource = selectedMatch.RankedItems;
            }     
        }

        private void btnReturnStart_Click(object sender, RoutedEventArgs e)
        {
            MainWindow parent = (MainWindow)Window.GetWindow(this);
            if (parent != null)
            {
                parent.MainContainer.Content = new StartScreen();
            }
        }
    }
}
