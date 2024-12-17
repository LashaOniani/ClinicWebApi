using ClinicWebApi.Models;
using ClinicWebApi.Packages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace ClinicWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : MainController
    {
        IPKG_Employee emp_package;

        public EmployeeController(IPKG_Employee emp_package)
        {
            this.emp_package = emp_package;
        }

        [HttpPost]
        //[Authorize]
        public IActionResult Save_Employee([FromForm] EmployeeModel employee)
        {
            try
            {
                byte[] cvData = null;
                byte[] picData = null;

                if(employee.Cv != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        employee.Cv.CopyTo(ms);
                        cvData = ms.ToArray();
                    }
                }

                if (employee.Picture != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        employee.Picture.CopyTo(ms);
                        picData = ms.ToArray();
                    }
                }

                emp_package.save_emp(employee, picData, cvData);
                return Ok("Employee saved successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet]
        //[Authorize]
        public IActionResult Get_Employees()
        {
            try
            {
                var employees = emp_package.get_emp();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // მოცემული მეთოდი ეძებს თანამშრომელს v_lo_clinic_employee ვიუში სადაც id არის person_id
        // (ცხრილში გაერთიანებულია person ების employee ების კატეგორიების და ა.შ. ცხრილები)
        // ფლიტრაცია ხდება persone ების ცხრილის id ის მიხედვით
        [HttpGet]
        public IActionResult Get_Employee_By_Id(int id)
        {
            try
            {
                var employee = emp_package.get_emp_by_id(id);
                if (employee != null)
                {
                    return Ok(employee);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex) 
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        //[Authorize]
        public IActionResult Get_Employee_By_Full_Name(string fullname)
        {
            try
            {
                var employees = emp_package.filter_emp_by_fullname(fullname);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        //[Authorize]
        public IActionResult Get_Employee_By_Category(string category)
        {
            try
            {
                var employees = emp_package.filter_emp_by_category(category);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Authorize]
        public IActionResult Delete_Employee(int id)
        {
            try
            {
                emp_package.delete_emp(id);
                return Ok($"Employee with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult Update_Employee(EmployeeModel employee)
        {
            try
            {
                emp_package.update_emp(employee);
                return Ok("Employee updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Get_First_Nine(int id) 
        {
            try
            {
                var employees = emp_package.get_emp_lazy_load(id);
                return Ok(employees);
            }
            catch (Exception ex)
            { 
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]

        public IActionResult Update_View_Count(int id)
        {
            try
            {
                if(id == 0)
                {
                    return BadRequest("აიდი ნოლი ვერ იქნება");
                }
                emp_package.update_view_count(id);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult Get_Employee_By_Emp_Id(int id)
        {
            try
            {
                var employee = emp_package.get_emp_by_emp_id(id);
                if (employee != null)
                {
                    return Ok(employee);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
