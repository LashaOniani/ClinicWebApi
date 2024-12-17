using ClinicWebApi.Auth;
using ClinicWebApi.Models;
using ClinicWebApi.Packages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : MainController
    {
        private readonly IJwtManager jwtManager;
        IPKG_User user_package;
        IPKG_Employee employee_package;

        public UserController(IPKG_User user_package, IJwtManager jwtManager, IPKG_Employee employee_package)
        {
            this.user_package = user_package;
            this.jwtManager = jwtManager;
            this.employee_package = employee_package;   
        }

        [HttpPost]
        public async Task<IActionResult> Save_User([FromForm] UserModel user)
        {
            try
            {
                byte[] picture = null;
                if (user.Picture != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        user.Picture.CopyTo(ms);
                        picture = ms.ToArray();
                    }
                }

                user.Role_id = 5;
                user_package.save_user(user, picture);

                return Ok("User registered and verification code sent.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get_Users()
        {
            try
            {
                var users = user_package.get_all_user();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}