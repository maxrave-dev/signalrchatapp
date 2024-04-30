using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Chat.Web.Models;
using Chat.Web.Models.BodyModel;
using Chat.Web.Models.ResponseModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Chat.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private IConfiguration _config;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        
        public AccountController(IConfiguration config, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]UserModel login)
        {
            IActionResult response = Unauthorized();
            var authenticateUser = await AuthenticateUser(login);
    
            if (authenticateUser)
            {
                var tokenString = GenerateJSONWebToken(login);
                Response.StatusCode = StatusCodes.Status200OK; 
                response = new JsonResult(tokenString);
            }
            else
            {
                var error = new ErrorResponse(message:
                    "Invalid Credentials"
                    );
                Response.StatusCode = StatusCodes.Status401Unauthorized; 
                response = new JsonResult(error);
            }
    
            return response;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel register)
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var user = new ApplicationUser { UserName = register.UserName, Email = register.Email, FullName = register.FullName };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded) {
                Response.StatusCode = StatusCodes.Status200OK; 
                return new JsonResult(
                    new ErrorResponse(
                        "User created successfully!",
                        false
                    ));
            }
            var message = "";
            foreach (var error in result.Errors)
            {
                message += error.Description + "\n";
            }
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new ErrorResponse(
                message
            ));
        }
    
        private TokenResponse GenerateJSONWebToken(UserModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(60);
    
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                },
                expires: expires,
                signingCredentials: credentials);
    
            return new TokenResponse(
                    userInfo.UserName, new JwtSecurityTokenHandler().WriteToken(token), expires
                );
        }
    
        private async Task<Boolean> AuthenticateUser(UserModel login)
        {
            //Validate the User Credentials
            //Demo Purpose, I have Passed HardCoded User Information
            var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, true, lockoutOnFailure: false);
    
            if (result.Succeeded)
            {
                var user = _userManager.Users.FirstOrDefault(u => u.UserName == login.UserName);

                //get the current user claims principal
                var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
                //get the current user's claims.
                var claimresult = claimsPrincipal.Claims.ToList();
                //it it doesn't contains the Role claims, add a role claims
                if (!claimresult.Any(c => c.Type == ClaimTypes.NameIdentifier))
                { 
                    //add claims to current user. 
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, user.UserName));
                }
                //refresh the Login
                await _signInManager.RefreshSignInAsync(user);
                return true;
            }
            else
            {
                return false;
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    
    }
}