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
    [Route("api/templates")]
    public class TemplatesController : Controller
    {
        private readonly ApiDBContext _context;

        public TemplatesController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: templates
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetTemplates([FromQuery] int userId, [FromQuery] string updateTimeString)
        {
            DateTime updateTime;
            DateTime.TryParse(updateTimeString, out updateTime);
            List<Template> Templates = _context.Templates.Where(i => i.UserId == userId && (i.UpdateTime > updateTime || i.UpdateTime == null)).ToList();
            string json = JsonConvert.SerializeObject(Templates);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }

        //GET: templates
        [HttpGet]
        [Route ("published")]
        public async Task<ActionResult<EncryptedResponse>> GetPublishedTemplates([FromQuery] int userId)
        {
            List<Template> Templates = _context.Templates.Where(i => i.UserId != userId && i.Status =="Published" && i.UpdateTime != null).ToList();
            string json = JsonConvert.SerializeObject(Templates);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST templates
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdateTemplates([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                List<Template> Templates = JsonConvert.DeserializeObject<List<Template>>(decryptedData);
                if (Templates.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in Templates)
                {
                    var Templatedb = await _context.Templates.FindAsync(value.Id);
                    if (Templatedb == null)
                        _context.Add(value);
                    else
                        _context.Entry(Templatedb).CurrentValues.SetValues(value);
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