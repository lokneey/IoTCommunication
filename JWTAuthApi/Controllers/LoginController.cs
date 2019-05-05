/*
    During creation of this file there were used following websites as source of knowledge:
    https://www.c-sharpcorner.com/article/jwt-json-web-token-authentication-in-asp-net-core/
*/

using JWTAuthApi.Models;
using JWTAuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace JWTAuthApi.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private MySqManager _mySqManager;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody]UserModel login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user.UserException == null)
            {
                var tokenString = GenerateJSONWebToken(user);

                //Adding token to database for debug and biulding resons
                _mySqManager = HttpContext.RequestServices.GetService(typeof(JWTAuthApi.Services.MySqManager)) as MySqManager;
                _mySqManager.AddNewToken(new TokenModelForStorageModel() { TokenToken = tokenString, TokenUser = user.UserLogin });

                response = Ok(new { token = tokenString });
            }
            else
            {
                response = BadRequest(new { token = "Nieprawidłowe dane logowania" });
            }
            return response;
        }

        private string GenerateJSONWebToken(UserModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel AuthenticateUser(UserModel user)
        {
            if (user.UserLogin != null && user.UserPassword != null)
            {                      
                _mySqManager = HttpContext.RequestServices.GetService(typeof(JWTAuthApi.Services.MySqManager)) as MySqManager;
                user = _mySqManager.GetSpecificUserWithPassword(user);
            }
            else
            {
                user.UserException = "Nieprawidłowy format otrzymanych danych. Wymagane dane są null.";
            }
            return user;
        }


    }
}
