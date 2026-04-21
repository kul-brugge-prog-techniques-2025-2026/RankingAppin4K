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
            //Vraagt de gesorteerde lijst op aan de business laag
            List<subjectItem> rankedItems = _business.GetFinalRankedList();

            //Maakt tijdelijke lijst voor objecten te tonen op ui
            var displayList = new List<RankedItemDisplay>();

            //Iteratie door lijst om data te mappen
            for (int i = 0; i < rankedItems.Count; i++ )
            {
                displayList.Add(new RankedItemDisplay
                {
                    Rank = i + 1,
                    Name = rankedItems[i].Text[0],
                    Image = AppDomain.CurrentDomain.BaseDirectory + rankedItems[i].Image
                });
            }
            //Koppel de gevulde lijst aan de Itemsource van de listview
            lvResultList.ItemsSource = displayList;

            //Roep de vergelijkings method op in de business laag
            ComparedRankingResult[] comparisons = _business.Compare();
            lvComparison.ItemsSource = comparisons;
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
    }
}
