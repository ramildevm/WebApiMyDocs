using System;
using System.Collections.Generic;
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
    [Route("api/creditcards")]
    public class CreditCardsController : Controller
    {
        private readonly ApiDBContext _context;

        public CreditCardsController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: CreditCards
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetCreditCards([FromQuery] int userId, [FromQuery] string updateTimeString)
        {
            MongoDBContext mongoDb = new MongoDBContext();
            DateTime updateTime;
            DateTime.TryParse(updateTimeString, out updateTime);
            List<Item> items = _context.Items.Where(i => i.UserId == userId && (i.UpdateTime > updateTime || i.UpdateTime == null)).ToList();
            List<CreditCard> CreditCards = items
            .Join(_context.CreditCards,
                item => item.Id,
                CreditCard => CreditCard.Id,
                (item, CreditCard) => CreditCard)
            .Where(v => v.UpdateTime > updateTime || v.UpdateTime == null).ToList();
            foreach (var value in CreditCards)
            {
                if (value.PhotoPage1 == null)
                    continue;
                value.PhotoPage1 = mongoDb.GetBase64File(MongoDB.Bson.ObjectId.Parse( value.PhotoPage1));
            }
            string json = JsonConvert.SerializeObject(CreditCards);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST CreditCards
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdateCreditCards([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                MongoDBContext mongoDb = new MongoDBContext();

                List<CreditCard> CreditCards = JsonConvert.DeserializeObject<List<CreditCard>>(decryptedData);
                if (CreditCards.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in CreditCards)
                {
                    var CreditCarddb = await _context.CreditCards.FindAsync(value.Id);                
                    value.PhotoPage1 = mongoDb.SaveUpdateBase64File(value.PhotoPage1, CreditCarddb==null?null:CreditCarddb.PhotoPage1, MongoDBContext.GenerateRandomFilename(value.Id)).ToString();
                    if (CreditCarddb == null)                    
                        _context.Add(value);                    
                    else
                        _context.Entry(CreditCarddb).CurrentValues.SetValues(value);
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
