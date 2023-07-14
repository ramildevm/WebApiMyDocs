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
    [Route("api/passports")]
    public class PassportsController : Controller
    {
        private readonly ApiDBContext _context;

        public PassportsController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: passports
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetPassports([FromQuery] int userId, [FromQuery] string updateTimeString)
        {
            MongoDBContext mongoDb = new MongoDBContext();
            DateTime updateTime;
            DateTime.TryParse(updateTimeString, out updateTime);
            List<Item> items = _context.Items.Where(i => i.UserId == userId && (i.UpdateTime > updateTime || i.UpdateTime == null)).ToList();
            List<Passport> passports = items
            .Join(_context.Passports,
                item => item.Id,
                passport => passport.Id,
                (item, passport) => passport)
            .Where(v => v.UpdateTime > updateTime || v.UpdateTime == null).ToList();
            foreach(var value in passports)
            {
                value.FacePhoto = value.FacePhoto == null ? null : mongoDb.GetBase64File(MongoDB.Bson.ObjectId.Parse(value.FacePhoto));
                value.PhotoPage1 = value.PhotoPage1 == null ? null : mongoDb.GetBase64File(MongoDB.Bson.ObjectId.Parse(value.PhotoPage1));
                value.PhotoPage2 = value.PhotoPage2 == null ? null : mongoDb.GetBase64File(MongoDB.Bson.ObjectId.Parse(value.PhotoPage2));
            }
            string json = JsonConvert.SerializeObject(passports);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST passports
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdatePassports([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                    MongoDBContext mongoDb = new MongoDBContext();
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                List<Passport> passports = JsonConvert.DeserializeObject<List<Passport>>(decryptedData);
                if (passports.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in passports)
                {

                    var passportdb = await _context.Passports.FindAsync(value.Id);

                    value.PhotoPage1 = mongoDb.SaveUpdateBase64File(value.PhotoPage1, passportdb == null ? null : passportdb.PhotoPage1, MongoDBContext.GenerateRandomFilename(value.Id)).ToString();
                    value.PhotoPage2 = mongoDb.SaveUpdateBase64File(value.PhotoPage2, passportdb == null ? null : passportdb.PhotoPage2, MongoDBContext.GenerateRandomFilename(value.Id)).ToString();
                    value.FacePhoto = mongoDb.SaveUpdateBase64File(value.FacePhoto, passportdb == null ? null : passportdb.FacePhoto, MongoDBContext.GenerateRandomFilename(value.Id)).ToString();
                    if (passportdb == null)
                        _context.Add(value);
                    else
                        _context.Entry(passportdb).CurrentValues.SetValues(value);
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
