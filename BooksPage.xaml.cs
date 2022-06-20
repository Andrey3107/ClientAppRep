using ClientApp.Exceptions;
using ClientApp.Models;
using ClientApp.Repository;
using ClientApp.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ClientApp
{
    public partial class BooksPage : Page
    {
        private BookRepository _bookRepository;
        public BooksPage()
        {
            InitializeComponent();
            _bookRepository = new BookRepository();
            GetAllBooks();
        }

        private void phonesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreateEditPage((sender as Button).DataContext as BookViewModel));
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreateEditPage(null));
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var booksForDelete = booksList.SelectedItems.Cast<BookViewModel>().ToList();

            if (booksForDelete.Count != 0 && MessageBox.Show("Are you sure you want to delete selected books?", "Attention",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var book in booksForDelete)
                {
                    await DeleteBook(book.Id);
                }
                MessageBox.Show("All selected books was successfuly deleted!");
                GetAllBooks();
            }
        }
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            GetAllBooks();
        }
        private async void GetAllBooks()
        {
            booksList.ItemsSource = null;
            try
            {
                booksList.ItemsSource = await _bookRepository.GetAll();
            }
            catch (HttpException ex)
            {
                MessageBox.Show(ex.Message);
                if (ex.StatusCode == 500)
                {
                    NavigationService.Navigate(new WelcomePage());
                }
            }
        }
        private async Task DeleteBook(int id)
        {
            try
            {
                await _bookRepository.Delete(id);
            }
            catch (HttpException ex)
            {
                MessageBox.Show(ex.Message);
                if (ex.StatusCode == 500)
                {
                    NavigationService.Navigate(new WelcomePage());
                }
            }
        }
    }
}
