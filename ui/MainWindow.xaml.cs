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

namespace ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Zet bij het opstarten StartScreen in de container
            MainContainer.Content = new StartScreen();
        }

        public void StartRanking(int subjectId)
        {
            //Vervangt huidige content door MainGameScreen en geeft subjectId door aan de contstructor
            MainContainer.Content = new MainGameScreen(subjectId);
        }
    }
}