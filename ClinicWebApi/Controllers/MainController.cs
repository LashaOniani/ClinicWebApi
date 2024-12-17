using ClinicWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicWebApi.Controllers
{
    public class MainController : ControllerBase
    {
        protected UserModel? AuthUser
        {
            get
            {
                var currentUser = HttpContext.User;

                if (currentUser != null && currentUser.HasClaim(c => c.Type == "UserID"))
                {
                    var userIdClaim = currentUser.FindFirst("UserID");
                    var roleIdClaim = currentUser.FindFirst("RoleID"); // role_ID

                    if (userIdClaim != null)
                    {
                        return new UserModel
                        {
                            Id = int.Parse(userIdClaim.Value),
                            Role_id = int.Parse(roleIdClaim.Value)
                        };
                    }
                }
                return null;
            }
        }


    }
}
