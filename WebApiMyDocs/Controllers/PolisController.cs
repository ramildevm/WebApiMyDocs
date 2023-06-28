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
    [Route("api/polis")]
    public class PolisController : Controller
    {
        private readonly ApiDBContext _context;

        public PolisController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: Polis
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetPolis([FromQuery] int userId, [FromQuery] string updateTimeString)
        {
            DateTime updateTime;
            DateTime.TryParse(updateTimeString, out updateTime);
            List<Item> items = _context.Items.Where(i => i.UserId == userId && (i.UpdateTime > updateTime || i.UpdateTime == null)).ToList();
            List<Poli> Polis = items
            .Join(_context.Polis,
                item => item.Id,
                Poli => Poli.Id,
                (item, Poli) => Poli)
            .Where(v => v.UpdateTime > updateTime || v.UpdateTime == null).ToList();
            foreach (var value in Polis)
            {
                value.PhotoPage164 = value.PhotoPage1 == null ? null : Convert.ToBase64String(value.PhotoPage1);
                value.PhotoPage264 = value.PhotoPage2 == null ? null : Convert.ToBase64String(value.PhotoPage2);
            }
            string json = JsonConvert.SerializeObject(Polis);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST Polis
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdatePolis([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                List<Poli> Polis = JsonConvert.DeserializeObject<List<Poli>>(decryptedData);
                if (Polis.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in Polis)
                {
                    value.PhotoPage1 = (string.IsNullOrEmpty(value.PhotoPage164) ? null : Convert.FromBase64String(value.PhotoPage164));
                    value.PhotoPage2 = (string.IsNullOrEmpty(value.PhotoPage264) ? null : Convert.FromBase64String(value.PhotoPage264));
                    var Polidb = await _context.Polis.FindAsync(value.Id);
                    if (Polidb == null)
                        _context.Add(value);
                    else
                        _context.Entry(Polidb).CurrentValues.SetValues(value);
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
