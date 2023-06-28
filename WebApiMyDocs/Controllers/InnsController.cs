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
    [Route("api/inns")]
    public class InnsController : Controller
    {
        private readonly ApiDBContext _context;

        public InnsController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: Inns
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetInns([FromQuery] int userId, [FromQuery] string updateTimeString)
        {
            DateTime updateTime;
            DateTime.TryParse(updateTimeString, out updateTime);
            List<Item> items = _context.Items.Where(i => i.UserId == userId && (i.UpdateTime > updateTime || i.UpdateTime == null)).ToList();
            List<Inn> Inns = items
            .Join(_context.Inns,
                item => item.Id,
                Inn => Inn.Id,
                (item, Inn) => Inn)
            .Where(v => v.UpdateTime > updateTime || v.UpdateTime == null).ToList();
            foreach (var value in Inns)
            {
                value.PhotoPage164 = value.PhotoPage1 == null ? null : Convert.ToBase64String(value.PhotoPage1);
            }
            string json = JsonConvert.SerializeObject(Inns);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST Inns
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdateInns([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                List<Inn> Inns = JsonConvert.DeserializeObject<List<Inn>>(decryptedData);
                if (Inns.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in Inns)
                {
                    value.PhotoPage1 = (string.IsNullOrEmpty(value.PhotoPage164) ? null : Convert.FromBase64String(value.PhotoPage164));
                    var Inndb = await _context.Inns.FindAsync(value.Id);
                    if (Inndb == null)
                        _context.Add(value);
                    else
                        _context.Entry(Inndb).CurrentValues.SetValues(value);
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
