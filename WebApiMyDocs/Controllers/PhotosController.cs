using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApiMyDocs.Models;
using WebApiMyDocs.Services;

namespace WebApiMyDocs.Controllers
{
    [ApiController]
    [Route("api/photos")]
    public class PhotosController : Controller
    {
        private readonly ApiDBContext _context;

        public PhotosController(ApiDBContext context)
        {
            _context = context;
        }
        
        //GET: Photos
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetPhotos([FromQuery] int userId, [FromQuery] string updateTimeString)
        {
            DateTime updateTime;
            DateTime.TryParse(updateTimeString, out updateTime);
            List<Item> items = _context.Items.Where(i => i.UserId == userId && i.Type=="Collection" && (i.UpdateTime > updateTime || i.UpdateTime == null)).ToList();
            List<Photo> Photos = new List<Photo>();
            foreach(var item in items)
            {
                var photoList = _context.Photos.Where(p => p.CollectionId == item.Id && (p.UpdateTime > updateTime || p.UpdateTime == null)).ToList();
                Console.WriteLine(photoList.Count.ToString());
                foreach (var photo in photoList)
                {

                    Console.WriteLine(photo==null);
                    photo.Image64 = photo.Image == null ? null : Convert.ToBase64String(photo.Image);
                    Photos.Add(photo);
                }
            }
            string json = JsonConvert.SerializeObject(Photos);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST Photos
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdatePhotos([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                List<Photo> Photos = JsonConvert.DeserializeObject<List<Photo>>(decryptedData);
                if (Photos.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in Photos)
                {
                    value.Image = (string.IsNullOrEmpty(value.Image64) ? null : Convert.FromBase64String(value.Image64));
                    var Photodb = await _context.Photos.FindAsync(value.Id);
                    if (Photodb == null)
                        _context.Add(value);
                    else
                        _context.Entry(Photodb).CurrentValues.SetValues(value);
                }
                await _context.SaveChangesAsync();
                string responseJson = JsonConvert.SerializeObject("Done");
                string encryptedResponseData = CryptoService.EncryptData(responseJson);
                EncryptedResponse encryptedResponse = new EncryptedResponse
                {
                    EncryptedData = encryptedResponseData
                };
                return await Task.FromResult(Ok(encryptedResponse));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(StatusCode((int)HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }
}