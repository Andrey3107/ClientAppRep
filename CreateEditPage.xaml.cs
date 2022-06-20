using ClientApp.Exceptions;
using ClientApp.Models;
using ClientApp.Repository;
using ClientApp.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ClientApp
{
    public partial class CreateEditPage : Page
    {
        private BookRepository _bookRepository;
        private Book _newBook = new Book();
        private string imagePath;
        public CreateEditPage(BookViewModel selectedBook)
        {
            InitializeComponent();
            _bookRepository = new BookRepository();

            if (selectedBook != null)
            {
                _newBook = selectedBook.ConvertIntoBook();
            }

            DataContext = _newBook;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (DataIsValid())
            {
                _newBook.ImgPath = BtnUpload.Content.ToString();
                string message;
                try
                {
                    if (_newBook.Id == 0)
                    {
                        message = await _bookRepository.Create(_newBook);
                        await _bookRepository.UploadImage(imagePath);
                    }
                    else
                    {
                        message = await _bookRepository.Update(_newBook);
                    }
                    MessageBox.Show(message);
                    NavigationService.Navigate(new BooksPage());
                }
                catch (HttpException ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    if (ex.StatusCode == 500)
                    {
                        NavigationService.Navigate(new WelcomePage());
                    }
                }
            }
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Image files (*.jpg)|*.jpg|All files (*.*)|*.*";

            bool? response = dialog.ShowDialog();

            if (response == true)
            {
                string filePath = dialog.FileName;
                BtnUpload.Content = Path.GetFileName(filePath);
                imagePath = filePath;
            }
        }

        private bool DataIsValid()
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_newBook.Name))
            {
                errors.AppendLine("Enter book name!");
            }

            if (BtnUpload.Content == null)
            {
                errors.AppendLine("Choose image!");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return false;
            }
            return true;
        }
    }
}
