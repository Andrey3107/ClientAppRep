using ClientApp.Models;
using ClientApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.ViewModels
{
    public class BookViewModel
    {
        private string imgPath;
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgPath {
            get 
            { 
                return BookRepository._serviceUrl + "/img/" + imgPath; 
            }
            set 
            {
                imgPath = value; 
            }
        }
        public Book ConvertIntoBook()
        {
            return new Book() { Id = this.Id, Name = this.Name, ImgPath = imgPath };
        }
    }
}
