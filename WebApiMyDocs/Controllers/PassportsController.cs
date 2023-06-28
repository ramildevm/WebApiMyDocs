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
                value.FacePhoto64 = value.FacePhoto == null ? null : Convert.ToBase64String(value.FacePhoto);
                value.PhotoPage164 = value.PhotoPage1 == null ? null : Convert.ToBase64String(value.PhotoPage1);
                value.PhotoPage264 = value.PhotoPage2 == null ? null : Convert.ToBase64String(value.PhotoPage2);
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
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                List<Passport> passports = JsonConvert.DeserializeObject<List<Passport>>(decryptedData);
                if (passports.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in passports)
                {
                    value.FacePhoto = (string.IsNullOrEmpty(value.FacePhoto64) ? null : Convert.FromBase64String(value.FacePhoto64));
                    value.PhotoPage1 = (string.IsNullOrEmpty(value.PhotoPage164) ? null : Convert.FromBase64String(value.PhotoPage164));
                    value.PhotoPage2 = (string.IsNullOrEmpty(value.PhotoPage264) ? null : Convert.FromBase64String(value.PhotoPage264));
                    var passportdb = await _context.Passports.FindAsync(value.Id);
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
