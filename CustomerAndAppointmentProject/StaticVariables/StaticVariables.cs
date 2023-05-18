using CustomerAndAppointmentProject.Models;

namespace CustomerAndAppointmentProject.StaticVariables
{
    public static class StaticVariables
    {
        public static bool LoggedIn = false;

        public static string? LoggedInAs;

        public static UserEntity User;

        public static DateTime Created = DateTime.MinValue;
    }
}
