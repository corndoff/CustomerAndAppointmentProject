using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CustomerAndAppointmentProject.Models
{
    public class UserEntity
    {
        public int Id { get; set; }
        [DisplayName("Full Name")]
        public string? Fullname { get; set; }
        [DisplayName("Type of User")]
        public string? TypeOfUser { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }


        public DateTime Created { get; set; }

    }
}
