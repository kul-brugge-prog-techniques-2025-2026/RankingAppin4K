using business;
using Models;
using persistentie;
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

namespace ui
{
    /// <summary>
    /// Interaction logic for StartScreen.xaml
    /// </summary>
    public partial class StartScreen : UserControl
    {
        //referentie pointer naar business laag
        private Business _business;
        private PersistenceObject _persistence;

        private int _subjectId = -1;
        const int DefaultSubjectId = 1;

        public StartScreen()
        {
            InitializeComponent();

            _persistence = new PersistenceObject();
            //init bussiness logica
            _business = new Business(DefaultSubjectId, _persistence);

            LoadCategories();
        }

        private void LoadCategories()
        {
            //Vraag lijst van subject-objecten op via business laag
            List<Subject> subjects = _business.GiveAllSubjects();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var subject in subjects)
            {
                List<subjectItem> items = _persistence.GetSubjectItems(subject.Id);

                if (items != null && items.Count > 0)
                {
                    string relativePath = items[0].Image;

                    string fullPath = System.IO.Path.Combine(baseDir, relativePath);

                    subject.Photo = new Uri(fullPath).AbsoluteUri;
                }
            }

            //Koppelt de opgehaalde lijst aan de listbox
            lbSubjects.ItemsSource = subjects;
        }

        private void lbSubjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbSubjects.SelectedItem is Subject selectedSubject)
            {
                _subjectId = selectedSubject.Id;

                LoadFilters(selectedSubject);

                //btnStartRanking.IsEnabled = true;
            }
        }

        private void LoadFilters(Subject currentSubject)
        {
            wpFilters.Children.Clear();

            if (currentSubject.Categories != null)
            {
                foreach (var category in currentSubject.Categories)
                {
                    CheckBox cb = new CheckBox
                    {
                        Content = category.Name,
                        Tag = category.Id,
                        Margin = new Thickness(5),
                        IsChecked = true
                    };

                    cb.Checked += (s, e) => UpdateStartButton();
                    cb.Unchecked += (s, e) => UpdateStartButton();

                    wpFilters.Children.Add(cb);
                }
            }
            UpdateStartButton();
        }

        private List<int> GetSelectedCategories()
        {
            List<int> selectedIds = new List<int>();

            foreach (var child in wpFilters.Children)
            {
                if (child is CheckBox cb && cb.IsChecked == true)
                {
                    selectedIds.Add((int)cb.Tag);
                }
            }

            return selectedIds;
        }

        private void btnStartRanking_Click(object sender, RoutedEventArgs e)
        {
            if (_subjectId == -1)
            {
                MessageBox.Show("Selecteer eerst een categorie.");
                return;
            }

            List<int> selectedCategories = GetSelectedCategories();

            MainWindow parentWindow = (MainWindow)Window.GetWindow(this);

            if (parentWindow != null)
            {
                parentWindow.StartRanking(_subjectId, selectedCategories);
            }
        }

        private void UpdateStartButton()
        {
            List<int> selectedCategories = GetSelectedCategories();
            btnStartRanking.IsEnabled = (_subjectId != -1 && selectedCategories.Count > 0);
        }
    }
}
