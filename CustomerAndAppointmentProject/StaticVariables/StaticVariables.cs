using CustomerAndAppointmentProject.Models;

namespace CustomerAndAppointmentProject.StaticVariables
{
    public static class StaticVariables
    {
        public static bool LoggedIn = false;

        public static string? LoggedInAs;

        public static UserLoginEntity? User;

        public static DateTime Created = DateTime.MinValue;
    }
}
