namespace ClinicWebApi.Models
{
    public class CategoriesModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public int CategoryDoctorsCount { get; set; }
    }
}
