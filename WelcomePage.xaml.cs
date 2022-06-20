using ClientApp.Repository;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ClientApp
{
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
            UrlTextBox.Text = "https://localhost:5001/books";
        }

        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient()) 
                { 
                    await client.GetAsync(UrlTextBox.Text); 
                }

                BookRepository._serviceUrl = UrlTextBox.Text;
                NavigationService.Navigate(new BooksPage());
            }
            catch (Exception)
            {
                MessageBox.Show("Server connection error!");
            }
        }
    }
}
