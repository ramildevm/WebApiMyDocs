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
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly ApiDBContext _context;

        public UsersController(ApiDBContext context)
        {
            _context = context;
        }

        //GET: users/byemail
        [HttpGet]
        [Route("byemail")]
        public async Task<ActionResult<EncryptedResponse>> GetUserByEmailAndPassword([FromQuery] String email, [FromQuery] String password)
        {
            try
            {
                User user = _context.Users.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
                if (user == null)
                    return await Task.FromResult(Conflict());
                user.Photo64 = user.Photo == null ? null : Convert.ToBase64String(user.Photo);
                string responseJson = JsonConvert.SerializeObject(user);
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
        //POST users
        [HttpPost]
        public async Task<ActionResult<EncryptedResponse>> CreateNewUser([FromBody] EncryptedResponse encrypted)
        {
            try
            {
                String encryptedData = encrypted.EncryptedData;
            string decryptedData = CryptoService.DecryptData(encryptedData);
            User user = JsonConvert.DeserializeObject<User>(decryptedData);
            if (_context.Users.Count(u => u.Email == user.Email) > 0)
                return await Task.FromResult(Conflict());

            user.Photo = (string.IsNullOrEmpty(user.Photo64) ? null : Convert.FromBase64String(user.Photo64));
            await _context.AddAsync(user); ;
            await _context.SaveChangesAsync();
            string responseJson = JsonConvert.SerializeObject(_context.Users.Where(u => u.Email == user.Email).FirstOrDefault());
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
        //PUT users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<EncryptedResponse>> UpdateUser(int id, [FromBody] EncryptedResponse encrypted)
        {
            try
            {
                string decryptedData = CryptoService.DecryptData(encrypted.EncryptedData);
                User updatedUser = JsonConvert.DeserializeObject<User>(decryptedData);

                User existingUser = _context.Users.Find(id);
                if (existingUser == null)
                {
                    return await Task.FromResult(NotFound());
                }
                if (_context.Users.Count(u => updatedUser.Email != existingUser.Email && u.Email == updatedUser.Email) > 0)
                    return await Task.FromResult(Conflict());
                existingUser.Login = updatedUser.Login;
                existingUser.Password = updatedUser.Password;
                existingUser.AccessCode = updatedUser.AccessCode;
                existingUser.Photo = (string.IsNullOrEmpty(updatedUser.Photo64) ? null : Convert.FromBase64String(updatedUser.Photo64));
                _context.Entry(existingUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                string responseJson = JsonConvert.SerializeObject(existingUser);
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
