using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interface.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUser user)
        {
            var result = await authenticationService.CreateUser(user);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(LoginUser user)
        {
            var result = await authenticationService.LoginUser(user);
            if (result.Success)
            {
                var options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(2), // Set token expiration time
                    SameSite = SameSiteMode.Strict // Optional, for CSRF protection
                };


                Response.Cookies.Append("Access_Token", result.Token, options);
                Response.Cookies.Append("SessionId", result.SessionId, options);
            }
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("refreshToken/{refreshToken}")]
        public async Task<IActionResult> ReviveToken(string refreshToken)
        {
            var result = await authenticationService.ReviveToken(refreshToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
