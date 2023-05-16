using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CustomerAndAppointmentProject.Models
{
    public class NewModel : PageModel
    {
        public UserEntity user = new UserEntity();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnPost()
        {
            UserEntity user = new UserEntity();
            user.Firstname = Request.Form["Firstname"];
            user.Lastname = Request.Form["lname"];
            user.Email = Request.Form["email"];
            user.Address = Request.Form["address"];
            user.Created = DateTime.Now;

            if (user.Email.Length == 0 || user.Address.Length == 0 || user.Firstname.Length == 0
                || user.Lastname.Length == 0)
            {
                errorMessage = "All Fields are required";
                return;
            }

            successMessage = "User was successfully added";
            return;
        }
    }
}
