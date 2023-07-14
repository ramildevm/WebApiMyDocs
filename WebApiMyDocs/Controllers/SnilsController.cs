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
    [Route("api/snils")]
    public class SnilsController : Controller
    {
        private readonly ApiDBContext _context;

        public SnilsController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: Snils
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetSnils([FromQuery] int userId, [FromQuery] string updateTimeString)
        {
            MongoDBContext mongoDb = new MongoDBContext();
            DateTime updateTime;
            DateTime.TryParse(updateTimeString, out updateTime);
            List<Item> items = _context.Items.Where(i => i.UserId == userId && (i.UpdateTime > updateTime || i.UpdateTime == null)).ToList();
            List<Snil> Snils = items
            .Join(_context.Snils,
                item => item.Id,
                Snil => Snil.Id,
                (item, Snil) => Snil)
            .Where(v => v.UpdateTime > updateTime || v.UpdateTime == null).ToList();
            foreach (var value in Snils)
            {
                value.PhotoPage1 = value.PhotoPage1 == null ? null : mongoDb.GetBase64File(MongoDB.Bson.ObjectId.Parse(value.PhotoPage1));
            }
            string json = JsonConvert.SerializeObject(Snils);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST Snils
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdateSnils([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                MongoDBContext mongoDb = new MongoDBContext();
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                List<Snil> Snils = JsonConvert.DeserializeObject<List<Snil>>(decryptedData);
                if (Snils.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in Snils)
                {
                    var Snildb = await _context.Snils.FindAsync(value.Id);
                    value.PhotoPage1 = mongoDb.SaveUpdateBase64File(value.PhotoPage1, Snildb == null ? null : Snildb.PhotoPage1, MongoDBContext.GenerateRandomFilename(value.Id)).ToString();

                    if (Snildb == null)
                        _context.Add(value);
                    else
                        _context.Entry(Snildb).CurrentValues.SetValues(value);
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
