namespace CustomerAndAppointmentProject.Models
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }


        public DateTime Created { get; set; }

    }
}
