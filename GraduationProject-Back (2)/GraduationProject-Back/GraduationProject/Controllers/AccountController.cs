using GraduationProject.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;

        }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;


        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterModel model)
        {
            // Check if the incoming model data is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Status = "Error",
                    Message = "Invalid input data.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            // Create a new ApplicationUser instance based on the received data


            ApplicationUser user = new()
            {
                Email = model.Email,
                UserName = model.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok(new { Status = "Success", Message = "User successfully registered." });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "Error",
                    Message = "User creation failed! Please check user details and try again.",
                    Errors = result.Errors.Select(e => e.Description)
                });
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  
                    };

                    var roles = await _userManager.GetRolesAsync(user);
                    foreach(var role in roles)
                    {
                        claims.Add(new (ClaimTypes.Role,role.ToString()));
                    }
                    var token = GetToken(claims);
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                    });
                }
               return Unauthorized();
            }
            return BadRequest(ModelState);
        }

        private JwtSecurityToken GetToken(List<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        expires: DateTime.Now.AddHours(3), 
        claims: claims, 
        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256) 
    );

            return token;
        }

        [HttpPost("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthDto externalAuth)
        {
            var payload = await VerifyGoogleToken(externalAuth);

            if (payload == null)
            {
                return BadRequest(new { Message = "Invalid External Authentication." });
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new ApplicationUser()
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = true 
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        Status = "Error",
                        Message = "User creation failed!",
                        Errors = result.Errors.Select(e => e.Description)
                    });
                }
            }

            var token = await GenerateJwtToken(user);
            return Ok(new
            {
                Token = token,
                Expiration = DateTime.Now.AddHours(3)
            });
        }

        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { "901065506309-j4v8i0u0e48ih27mgs7iq0s5d5d9pjdc.apps.googleusercontent.com" } 
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.Token, settings);
            return payload;
        }
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }

}

