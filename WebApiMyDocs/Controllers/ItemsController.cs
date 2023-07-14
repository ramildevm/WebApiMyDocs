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
    [Route("api/items")]
    public class ItemsController : Controller
    {
        private readonly ApiDBContext _context;

        public ItemsController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: itmes
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetItems([FromQuery] int userId, [FromQuery] string updateTimeString)
        {
            MongoDBContext mongoDb = new MongoDBContext();
            DateTime updateTime;
            DateTime.TryParse(updateTimeString, out updateTime);
            List<Item> itemList = _context.Items.Where(i => i.UserId == userId && (i.UpdateTime > updateTime || i.UpdateTime == null)).ToList();

            foreach(var value in itemList)
            {
                if (value.Image == null)
                    continue;
                value.Image = mongoDb.GetBase64File(MongoDB.Bson.ObjectId.Parse(value.Image));
            }

            string json = JsonConvert.SerializeObject(itemList);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST items
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdateItems([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                MongoDBContext mongoDb = new MongoDBContext();
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(decryptedData);
                if (items.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var item in items)
                {

                    var itemdb = await _context.Items.FindAsync(item.Id);
                    item.Image = mongoDb.SaveUpdateBase64File(item.Image, itemdb == null ? null : itemdb.Image, MongoDBContext.GenerateRandomFilename(item.Id)).ToString();

                    if (itemdb == null)
                        _context.Add(item);
                    else
                        _context.Entry(itemdb).CurrentValues.SetValues(item);
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