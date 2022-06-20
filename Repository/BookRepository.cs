using ClientApp.Models;
using ClientApp.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using ClientApp.ViewModels;

namespace ClientApp.Repository
{
    public class BookRepository
    {
        public static string _serviceUrl;
        public BookRepository()
        {
        }
        public async Task<List<BookViewModel>> GetAll()
        {
            await ServerConnectionCheck();
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync(_serviceUrl);
                string stringresult = await GetResult(result);
                var listResult = JsonConvert.DeserializeObject<List<BookViewModel>>(stringresult);
                return listResult;
            }
        }

        public async Task<string> Create(Book book)
        {
            await ServerConnectionCheck();
            var json = JsonConvert.SerializeObject(book);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                var result = await client.PostAsync(_serviceUrl, data);
                return await GetResult(result);
            }
        }

        public async Task<string> Update(Book book)
        {
            await ServerConnectionCheck();
            var json = JsonConvert.SerializeObject(book);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                var result = await client.PutAsync(_serviceUrl, data);
                return await GetResult(result);
            }
        }

        public async Task<string> Delete(int id)
        {
            await ServerConnectionCheck();
            string editedPath = _serviceUrl + "/" + id;
            using (HttpClient client = new HttpClient())
            {
                var result = await client.DeleteAsync(editedPath);
                return await GetResult(result);
            }
        }

        public async Task<string> UploadImage(string fileRoute)
        {
            await ServerConnectionCheck();
            var fileName = Path.GetFileName(fileRoute);
            MultipartFormDataContent requestContent = new MultipartFormDataContent();
            FileStream fileStream = File.OpenRead(fileRoute);
            requestContent.Add(new StreamContent(fileStream), "uploadedFile", fileName);
            using (HttpClient client = new HttpClient())
            {
                var result = await client.PostAsync(_serviceUrl + "/img", requestContent);
                return await GetResult(result);
            }
        }

        private async Task<string> GetResult(HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                string stringresult = await responseMessage.Content.ReadAsStringAsync();
                return stringresult;
            }
            else
            {
                switch (responseMessage.StatusCode)
                {
                    case HttpStatusCode.NoContent: 
                        throw new HttpException("File is empty!", 204);
                    case HttpStatusCode.NotFound: 
                        throw new HttpException(await responseMessage.Content.ReadAsStringAsync(), 404);
                    case HttpStatusCode.BadRequest: 
                        throw new HttpException(await responseMessage.Content.ReadAsStringAsync(), 400);
                    default: 
                        throw new HttpException("Something went wrong!", 400);
                }
            }
        }

        private async Task ServerConnectionCheck()
        {
            try
            {
                using (HttpClient client = new HttpClient()) 
                { 
                    await client.GetAsync(_serviceUrl); 
                }
            }
            catch (Exception)
            {
                throw new HttpException("Server connection error!", 500);
            }
        }
    }
}
