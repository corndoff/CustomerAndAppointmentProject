using System.ComponentModel;

namespace CustomerAndAppointmentProject.Models
{
    public class AppointmentEntity
    {
        public int Id { get; set; }
        [DisplayName("Full Name")]
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        [DisplayName("Appointment Date")]
        public DateTime AppointmentDate { get; set; }
        [DisplayName("Notes for Doctor")]
        public string? Notes { get; set; }
    }
}
