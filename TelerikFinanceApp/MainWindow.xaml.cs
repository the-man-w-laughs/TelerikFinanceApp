using FinanceApp.MVVM;
using Services;
using System.Windows;

namespace TelerikFinanceApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var currencyLoaderService = new CurrencyLoaderService();
            DataContext = new MainViewModel(currencyLoaderService);
        }
    }
}
