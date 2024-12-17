namespace ClinicWebApi.Models
{
    public class EmployeeModel : UserModel
    {
        public int? EmployeeId { get; set; }
        public int? Category_id { get; set; }
        public string? Category_name { get; set; }
        public IFormFile? Cv { get; set; }
        public double? Review_count { get; set; }
        public int? View_count { get; set; }
    }
}
