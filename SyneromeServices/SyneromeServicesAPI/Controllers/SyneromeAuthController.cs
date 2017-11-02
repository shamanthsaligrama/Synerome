using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SyneromeServicesAPI.Models;
using SyneromeServicesAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SyneromeServicesAPI.Controllers
{
    [Authorize]
    [Route("SyneromeAPI/[controller]")]
    public class SyneromeAuthController : Controller
    {
        #region Declarations
        private ISyneromeAuthServices _syneromeAuthService;
        private IMapper _mapper;
        IConfiguration _iConfig;
        private double _syneromeAppTimeout;
        #endregion Declarations

        #region Constructor
        public SyneromeAuthController(ISyneromeAuthServices syneromeAuthService, IMapper mapper, IConfiguration configValues)
        {
            _syneromeAuthService = syneromeAuthService;
            _mapper = mapper;
            _iConfig = configValues;
        }
        #endregion Constructor

        #region Get Users and Nutritionists
        [AllowAnonymous]
        [HttpGet("GetUsers")]
        public JsonResult GetUsers()
        {
            return new JsonResult(new List<object>()
            {
                new { id = 1, Name= "User1"},
                new { id = 2, Name= "User2"},
                new { id = 3, Name= "User3"},
                new { id = 4, Name= "User4"},
                new { id = 5, Name= "User5"}
            });
        }

        [Authorize]
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _syneromeAuthService.GetAllUsers();
                var userModel = _mapper.Map<IEnumerable<SyneromeServicesAPI.Models.Users>>(users);
                return Ok(userModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUser/{userid}")]
        public IActionResult GetUserById(int userid)
        {
            try
            {
                var user = _syneromeAuthService.GetUserById(userid);
                var userModel = _mapper.Map<Users>(user);
                return Ok(userModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllNutritionists")]
        public IActionResult GetAllNutritionists()
        {
            try
            {
                var nutritionists = _syneromeAuthService.GetAllNutritionists();
                var nutritionistModel = _mapper.Map<IEnumerable<SyneromeServicesAPI.Models.Nutritionists>>(nutritionists);
                return Ok(nutritionistModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetNutritionist/{nutritionistid}")]
        public IActionResult GetNutritionistById(int nutritionistid)
        {
            try
            {
                var nutritionist = _syneromeAuthService.GetNutritionistById(nutritionistid);
                var nutritionistModel = _mapper.Map<Nutritionists>(nutritionist);
                return Ok(nutritionistModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion Get Users and Nutritionists

        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //[Route("SignIn")]
        //public async Task SignIn(string userName)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, userName),
        //        new Claim("syneromeUserName", userName),

        //    };

        //    var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme, "syneromeUserName", null);
        //    var principal = new ClaimsPrincipal(identity);
        //    await HttpContext.SignInAsync(principal,
        //        new AuthenticationProperties
        //        {
        //            IsPersistent = false,
        //            ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
        //        });
        //}

        #region SignUp
        [AllowAnonymous]
        [HttpPost]
        [Route("SignUpUsers")]
        public IActionResult SignUpUsers([FromBody]Users userModel)
        {
            if (userModel == null)
                return BadRequest("Model data is null");

            try
            {
                // map dto to entity
                var user = _mapper.Map<SyneromeServices.Domain.Users>(userModel);

                _syneromeAuthService.CreateUsers(user, userModel.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SignUpNutritionists")]
        public IActionResult SignUpNutritionists([FromBody]Nutritionists nutritionistModel)
        {
            if (nutritionistModel == null)
                return BadRequest("Model data is null");
          
            try
            {
                // map dto to entity
                var nutritionist = _mapper.Map<SyneromeServices.Domain.Nutritionists>(nutritionistModel);

                _syneromeAuthService.CreateNutritionist(nutritionist, nutritionistModel.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }
        #endregion SignUp

        #region Authenticate
        [AllowAnonymous]
        [HttpPost("AuthenticateUser")]
        public IActionResult AuthenticateUser([FromBody]Users userModel)
        {
            if (userModel == null)
                return BadRequest("Model data is null");

            try
            {
                var user = _syneromeAuthService.AuthenticateUser(userModel.Email, userModel.Password);

                if (user == null)
                    return Unauthorized();

                _syneromeAppTimeout = Convert.ToDouble(_iConfig["AppTimeOut:SyneromeTimeOutInMinutes"]);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_iConfig["AppSettings:SyneromeAppSecret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Email.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(_syneromeAppTimeout),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // return basic user info (without password) and token to store client side
                return Ok(new
                {
                    Username = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [AllowAnonymous]
        [HttpPost("AuthenticateNutritionist")]
        public IActionResult AuthenticateNutritionist([FromBody]Nutritionists nutritionistModel)
        {
            if (nutritionistModel == null)
                return BadRequest("Model data is null");

            try
            {
                var nutritionist = _syneromeAuthService.AuthenticateNutritionist(nutritionistModel.Email, nutritionistModel.Password);

                if (nutritionist == null)
                    return BadRequest("Model data is null");

                _syneromeAppTimeout = Convert.ToDouble(_iConfig["AppTimeOut:SyneromeTimeOutInMinutes"]);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_iConfig["AppSettings:SyneromeAppSecret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, nutritionist.Email.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(_syneromeAppTimeout),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // return basic user info (without password) and token to store client side
                return Ok(new
                {
                    Username = nutritionist.Email,
                    FirstName = nutritionist.FirstName,
                    LastName = nutritionist.LastName,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        #endregion Authenticate

        #region Update Users and Nutritionists
        [HttpPut("UpdateUsers/{id}")]
        public IActionResult UpdateUsers(int id, [FromBody]Users userModel)
        {
            if (userModel == null)
                return BadRequest("Model data is null");

            try
            {
                // map dto to entity and set id
                var user = _mapper.Map<SyneromeServices.Domain.Users>(userModel);
                user.Id = id;

                // save 
                _syneromeAuthService.UpdateUsers(user, userModel.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateNutritionists/{id}")]
        public IActionResult UpdateNutritionists(int id, [FromBody]Nutritionists nutritionistModel)
        {
            if (nutritionistModel == null)
                return BadRequest("Model data is null");

            try
            {
                // map dto to entity and set id
                var nutritionist = _mapper.Map<SyneromeServices.Domain.Nutritionists>(nutritionistModel);
                nutritionist.Id = id;

                // save 
                _syneromeAuthService.UpdateNutritionist(nutritionist, nutritionistModel.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(ex.Message);
            }
        }

        #endregion Update Users and Nutritionists

        //[AllowAnonymous]
        //[HttpPost]
        //[Route("SignOff")]
        //public async Task<IActionResult> SignOff()
        //{
        //    await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
        //    return this.Ok(new { message = "Signed Out successfully!" });
        //}

    }
}