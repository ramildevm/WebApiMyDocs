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
    [Route("api/templateobjects")]
    public class TemplateObjectObjectsController : Controller
    {
        private readonly ApiDBContext _context;

        public TemplateObjectObjectsController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: TemplateObjects
        [HttpGet]
        public async Task<ActionResult<EncryptedResponse>> GetTemplateObjects([FromQuery] int userId, [FromQuery] string updateTimeString)
        {
            DateTime updateTime;
            DateTime.TryParse(updateTimeString, out updateTime);
            List<Template> Templates = _context.Templates.Where(i => i.UserId == userId && (i.UpdateTime > updateTime || i.UpdateTime == null)).ToList();
            List<TemplateObject> TemplateObjects = Templates
           .Join(_context.TemplateObjects,
               item => item.Id,
               value => value.TemplateId,
               (item, value) => value)
           .Where(v => v.UpdateTime > updateTime || v.UpdateTime == null).ToList();
            string json = JsonConvert.SerializeObject(TemplateObjects);
            string encryptedData = CryptoService.EncryptData(json);
            return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = encryptedData }));
        }
        //POST TemplateObjects
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> UpdateTemplateObjects([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                String encryptedData = encrypted.EncryptedData;
                string decryptedData = CryptoService.DecryptData(encryptedData);
                List<TemplateObject> TemplateObjects = JsonConvert.DeserializeObject<List<TemplateObject>>(decryptedData);
                if (TemplateObjects.Count() == 0)
                    return await Task.FromResult(Ok(new EncryptedResponse() { EncryptedData = null }));
                foreach (var value in TemplateObjects)
                {
                    var TemplateObjectdb = await _context.TemplateObjects.FindAsync(value.Id);
                    if (TemplateObjectdb == null)
                        _context.Add(value);
                    else
                        _context.Entry(TemplateObjectdb).CurrentValues.SetValues(value);
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