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
    [Route("api/templatedocuments")]
    public class TemplateDocumentsController : Controller
    {
        private readonly ApiDBContext _context;

        public TemplateDocumentsController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: TemplateDocuments
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetTemplateDocuments([FromQuery] int userId, [FromQuery] string updateTimeString)
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
            string json = JsonConvert.SerializeObject(TemplateDocuments);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST TemplateDocuments
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdateTemplateDocuments([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                List<TemplateDocument> TemplateDocuments = JsonConvert.DeserializeObject<List<TemplateDocument>>(decryptedData);
                if (TemplateDocuments.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in TemplateDocuments)
                {
                    var TemplateDocumentdb = await _context.TemplateDocuments.FindAsync(value.Id);
                    if (TemplateDocumentdb == null)
                        _context.Add(value);
                    else
                        _context.Entry(TemplateDocumentdb).CurrentValues.SetValues(value);
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
