namespace ClinicWebApi.Models
{
    public class ReservationModel
    {
        public int? Id { get; set; }
        public int Employee_Id { get; set; }
        public int? User_Id { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public string? Reservation_Description { get; set; }
        public string? Status { get; set; }
    }
}
