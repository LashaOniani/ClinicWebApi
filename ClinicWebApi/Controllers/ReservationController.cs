using ClinicWebApi.Models;
using ClinicWebApi.Packages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

namespace ClinicWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReservationController : MainController
    {
        IPKG_Reservation reservation_package;
        IPKG_Employee employee_package;
        IPKG_User user_package;

        public ReservationController(IPKG_Reservation reservation_package, IPKG_Employee employee_package, IPKG_User user_package)
        {
            this.reservation_package = reservation_package;
            this.employee_package = employee_package;
            this.user_package = user_package;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add_Reservation(ReservationModel reservation) 
        {
            try
            {
                var userClaim = AuthUser;

                if (userClaim != null) { 

                    if(userClaim.Role_id == 1 || userClaim.Role_id == 2 || userClaim.Role_id == 3)
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, "თანამშრომელს არ შეუძლია, ჯავშნის გაკეთება"); // მუშაობს :) ადმინზე, ედიტორზე, მოდერატორზე
                    }

                    ReservationModel reservationModel = reservation;
                    reservationModel.User_Id = userClaim.Id;
                    //reservationModel.Start_Date = DateTime.Parse(reservation.Start_Date);
                    reservation_package.add_reservation(reservationModel);
                }

                else
                {   
                    return NotFound("იუზერი ვერ მოიძებნა");
                }

            }
            catch (OracleException oex)
            {
                if (oex.Number == 20004)
                {
                    return BadRequest("მოცემული დრო უკვე დაკავებულია");
                }
                if (oex.Number == 20003)
                {
                    return BadRequest("საწყისი დრო და ვიზიტის დამთავრების დრო ერთმანეთს არ უნდა ემთხვეოდეს");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get_User_Reservation_List()
        {
            try
            {
                var userToken = AuthUser;
                if (userToken != null) 
                {
                    if(userToken.Role_id == 5 || userToken.Role_id == 3 || userToken.Role_id == 2 || userToken.Role_id == 1)
                    {
                        var reservationList = reservation_package.get_user_reservation_list(userToken.Id);
                        return Ok(reservationList);
                    }else if (userToken.Role_id == 4)
                    {
                        var reservationList = reservation_package.get_doctor_reservation_list(userToken.Id);
                        return Ok(reservationList);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, "მონაცემების სიმცირის გამო, თქვენს დახმარებას ვერ ვახერხებთ, ცადეთ მოგვიანებით");
                    }
                }
                else
                {
                    return Unauthorized("საჭიროა მომხმარებლის ავტორიზაცია");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public IActionResult Get_doctors_reservations(int id)
        {
            try
            {
                var userToken = AuthUser;

                if(userToken != null)
                {
                    var reservationList = reservation_package.get_doctor_reservation_list_public(id, userToken.Id);
                    return Ok(reservationList);
                }
                else
                {
                    var reservationList = reservation_package.get_doctor_reservation_list_public(id, 0);
                    return Ok(reservationList);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        public IActionResult Delete_reservation(int reservation_id, int employee_id, int user_id)
        {
            try
            {
                var userClaim = AuthUser;
                if(userClaim != null)
                {
                    if (userClaim.Role_id == 5 || userClaim.Role_id == 4) // მომხმარებელი და დოქტორი
                    {
                        if(userClaim.Role_id == 5)
                        {
                            var user = user_package.get_user(userClaim.Id);
                            if(user.UserId == user_id)
                            {
                                reservation_package.delete_reservation(reservation_id, employee_id, user_id);
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status403Forbidden, "მომხმარებელს არ შეუძლია რეზერვაციის წაშლა");
                            }
                        }
                        else
                        {
                            var employee = employee_package.get_emp_by_id(userClaim.Id);
                            if (employee.EmployeeId == employee_id) 
                            {
                                reservation_package.delete_reservation(reservation_id, employee_id, user_id);
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status403Forbidden, "მომხმარებელს არ შეუძლია რეზერვაციის წაშლა");
                            }
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, "მომხმარებელს არ შეუძლია რეზერვაციის წაშლა");
                    }
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "მომხმარებელი არ არის ავტორიზირებული");
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult Upadte_Reservation(ReservationModel reservation)
        {
            try
            {
                var user = AuthUser;

                if (user == null) return Unauthorized();

                if(reservation != null)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
