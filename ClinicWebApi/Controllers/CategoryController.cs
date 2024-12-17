using ClinicWebApi.Models;
using ClinicWebApi.Packages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : MainController
    {
        private readonly IPKG_Category category;

        public CategoryController(IPKG_Category category)
        {
            this.category = category;
        }
            
        [HttpGet]
        //[Authorize]
        public ActionResult<List<CategoriesModel>> Get_categories()
        {
            var categories = category.get_all_categories();

            if (categories == null || !categories.Any()) 
            {
                return NotFound("No categories found.");
            }

            return Ok(categories);
        }
    }
}
