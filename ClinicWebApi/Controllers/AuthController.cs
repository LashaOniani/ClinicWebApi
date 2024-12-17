using ClinicWebApi.Auth;
using ClinicWebApi.Models;
using ClinicWebApi.Packages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : MainController
    {
        private readonly IJwtManager jwtManager;
        IPKG_User user_package;
        IPKG_Employee employee_package;

        public AuthController(IPKG_User user_package, IJwtManager jwtManager, IPKG_Employee employee_package)
        {
            this.user_package = user_package;
            this.jwtManager = jwtManager;
            this.employee_package = employee_package;
        }

        [HttpPost]
        public IActionResult Authenticate(LoginModel logindata)
        {
            Token token = null;
            UserModel? user = null;

            try
            {
                user = user_package.get_user_id(logindata);
                if (user == null) return Unauthorized("password or username is inccorect!");

                token = jwtManager.GetToken(user);
                if (token == null || string.IsNullOrEmpty(token.AccessToken))
                    return StatusCode(StatusCodes.Status500InternalServerError, "Token generation failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"System error: {ex.Message}");
            }

            return Ok(token);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get_Auth_Account()
        {
            try
            {
                var userClaim = AuthUser;

                if (userClaim == null)
                {
                    return Unauthorized("მომხმარებელი, ავტორიზირებული არ არის");
                }

                object? user = null;

                if (userClaim.Role_id == 5)
                {
                    user = user_package.get_user(userClaim.Id);
                    if (user == null)
                    {
                        return NotFound("მომხმარებელი ვერ მოვიძიეთ, ცადეთ ხელახლა");
                    }
                }
                else
                {
                    user = employee_package.get_emp_by_id(userClaim.Id);
                    if (user == null)
                    {
                        return NotFound("User Not Found");
                    }
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
    }
}
