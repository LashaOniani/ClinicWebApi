namespace ClinicWebApi.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public DateTime Birthday { get; set; }
        public int Gender { get; set; } 
        public string Id_card { get; set; }
        public string? Gender_str {  get; set; }
        public string? person_id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Role_id { get; set; }
        public string? Role_name { get; set; }   
        public string Password { get; set; }
        public IFormFile? Picture { get; set; }
        public string? PicData { get; set; }
    }
}
