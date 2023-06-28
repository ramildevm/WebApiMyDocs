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
    [Route("api/templatedocumentdata")]
    public class TemplateDocumentDatumsController : Controller
    {
        private readonly ApiDBContext _context;

        public TemplateDocumentDatumsController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: TemplateDocumentDatas
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetTemplateDocumentDatas([FromQuery] int userId, [FromQuery] string updateTimeString)
        {
            DateTime updateTime;
            DateTime.TryParse(updateTimeString, out updateTime);
            List<Item> items = _context.Items.Where(i => i.UserId == userId && (i.UpdateTime > updateTime || i.UpdateTime == null)).ToList();
            List<TemplateDocument> TemplateDocuments = items
            .Join(_context.TemplateDocuments,
                item => item.Id,
                TemplateDocument => TemplateDocument.Id,
                (item, TemplateDocument) => TemplateDocument)
            .Where(v => v.UpdateTime > updateTime || v.UpdateTime == null).ToList();
            List<TemplateDocumentDatum> TemplateDocumentDatas = TemplateDocuments
           .Join(_context.TemplateDocumentData,
               item => item.Id,
               value => value.TemplateDocumentId,
               (item, value) => value)
           .Where(v => v.UpdateTime > updateTime || v.UpdateTime == null).ToList();
            string json = JsonConvert.SerializeObject(TemplateDocumentDatas);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST TemplateDocumentDatas
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdateTemplateDocumentDatas([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                List<TemplateDocumentDatum> TemplateDocumentDatas = JsonConvert.DeserializeObject<List<TemplateDocumentDatum>>(decryptedData);
                if (TemplateDocumentDatas.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in TemplateDocumentDatas)
                {
                    var TemplateDocumentDatadb = await _context.TemplateDocumentData.FindAsync(value.Id);
                    if (TemplateDocumentDatadb == null)
                        _context.Add(value);
                    else
                        _context.Entry(TemplateDocumentDatadb).CurrentValues.SetValues(value);
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