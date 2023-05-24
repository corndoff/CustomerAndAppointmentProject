using CustomerAndAppointmentProject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace CustomerAndAppointmentProject.Controllers
{
    public class HomeController : Controller
    {
        #region Variables
        private readonly ILogger<HomeController> _logger;

        string BASEURL = "https://customerandappointmentrestfulapi.azurewebsites.net";
        //string BASEURL = "https://06b7-2603-9001-3f00-2ac4-77ac-a753-ae0d-f488.ngrok-free.app/";
        //string BASEURL = "http://localhost:10888";

        HttpClient client = new HttpClient();
        #endregion

        #region Ctor
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            client.BaseAddress = new Uri(BASEURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

        #region Index
        public IActionResult Index()
        {
           if(!string.IsNullOrEmpty(HttpContext.Session.GetString("Current User")))
            {
                if(JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
                {
                    var user = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                    ViewData["loggedIn"] = true;
                    ViewData["userName"] = user.Fullname;
                    if (user.Email.ToLower().Contains("doctorsofamerica"))
                    {
                        ViewData["admin"] = true;
                    }
                    else
                    {
                        ViewData["admin"] = false;
                    }
                    return View();
                }
            }
            ViewData["loggedIn"] = false;
            return View();
        }
        #endregion

        #region Users
        public async Task<IActionResult> User()
        {
            UserEntity user = new UserEntity();

            try
            {
                HttpResponseMessage getData = await client.GetAsync("users/by/" + JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")).Email);


                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<UserEntity>(results);
                }
            }
            catch(Exception ex) { 
            }

            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Users()
        {
            IList<UserEntity>? users = new List<UserEntity>();

            try
            {
                if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")).TypeOfUser.ToLower() == "admin")
                {
                    HttpResponseMessage getData = await client.GetAsync("users");

                    if (getData.IsSuccessStatusCode)
                    {
                        string results = getData.Content.ReadAsStringAsync().Result;
                        users = JsonConvert.DeserializeObject<List<UserEntity>>(results);
                    }
                    else
                    {
                        Console.WriteLine("Error calling Web API");
                    }
                    ViewData.Model = users;
                }
                else
                {
                    HttpResponseMessage getData = await client.GetAsync("users/1");

                    if (getData.IsSuccessStatusCode)
                    {
                        string results = getData.Content.ReadAsStringAsync().Result;
                        //users = new List<UserEntity> { JsonConvert.DeserializeObject<UserEntity>(results) };
                        users = JsonConvert.DeserializeObject<List<UserEntity>>(results);
                    }
                    else
                    {
                        Console.WriteLine("Error calling Web API");
                    }
                    ViewData.Model = users;
                }
            }
            catch (Exception)
            {

                throw;
            }

            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
                ViewData["id"] = globalUser.Id;
            }
            return View(users);
        }

        public IActionResult Register()
        {
            ViewData.Model = new UserEntity();
            return View(new UserEntity());
        }

        [HttpPost]
        public IActionResult Register(UserEntity user)
        {
            user.Created = DateTime.Now;
            if (user.TypeOfUser.ToLower() == "admin" && !user.Email.ToLower().Contains("doctorsofamerica"))
            {
                return View();
            }
            string data = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = client.PostAsync($"{client.BaseAddress}users/post", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    //StaticVariables.StaticVariables.LoggedIn = true;
                    //StaticVariables.StaticVariables.LoggedInAs = user.TypeOfUser;
                    //StaticVariables.StaticVariables.User = user;
                    HttpContext.Session.SetString("Current User", JsonConvert.SerializeObject(user));
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {

                return View();
            }

            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            return View();
        }

        public IActionResult NewUser()
        {
            ViewData.Model = new UserEntity();
            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
                ViewData["id"] = globalUser.Id;
            }
            return View(new UserEntity());
        }

        [HttpPost]
        public IActionResult NewUser(UserEntity user)
        {
            user.Created = DateTime.Now;
            user.Created = new DateTime(DateTime.Now.Ticks, DateTimeKind.Local);
            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }

            if (user.TypeOfUser.ToLower() == "admin" && !user.Email.ToLower().Contains("doctorsofamerica"))
            {
                return View();
            }
            string data = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = client.PostAsync($"{client.BaseAddress}users/post", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Users");
                }
            }
            catch (Exception)
            {

                return View();
            }
            return View();
        }

        public IActionResult Logout()
        {
            //StaticVariables.StaticVariables.LoggedIn = false;
            //StaticVariables.StaticVariables.User = new UserEntity();
            HttpContext.Session.Clear();
            if(!string.IsNullOrEmpty(HttpContext.Session.GetString("Current User")))
            {
                if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
                {
                    var user = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            ViewData.Model = new UserLoginEntity();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Current User")))
            {
                if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
                {
                    var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                    ViewData["loggedIn"] = true;
                    ViewData["userName"] = globalUser.Fullname;
                    if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                    {
                        ViewData["admin"] = true;
                    }
                    else
                    {
                        ViewData["admin"] = false;
                    }
                    ViewData["id"] = globalUser.Id;
                }
            }
            return View(new UserLoginEntity());
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginEntity entity)
        {
            try
            {
                HttpResponseMessage getData = await client.GetAsync(client.BaseAddress + "users/by/" + entity.Email);

                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    HttpContext.Session.SetString("Current User", JsonConvert.SerializeObject(JsonConvert.DeserializeObject<UserEntity>(results)));
                    //StaticVariables.StaticVariables.User = JsonConvert.DeserializeObject<UserEntity>(results);

                    //StaticVariables.StaticVariables.LoggedIn = true;
                    //StaticVariables.StaticVariables.LoggedInAs = StaticVariables.StaticVariables.User.TypeOfUser;
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {

                throw;
            }

            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            UserEntity? user = new UserEntity();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "users/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserEntity>(data);
                //StaticVariables.StaticVariables.Created = user.Created;
            }

            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(UserEntity user)
        {
            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            string data = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = client.PutAsync(client.BaseAddress + "users/" + user.Id, content).Result;

                //if (response.IsSuccessStatusCode)
                //{
                //    return RedirectToAction("Users");
                //}
                if (response.IsSuccessStatusCode)
                {
                    if (ViewData["admin"] != null && (bool)ViewData["admin"])
                    {
                        return RedirectToAction("Users");
                    }
                    return RedirectToAction("User");
                    //return RedirectToAction("Appointments");
                }

            }
            catch (Exception)
            {
                return RedirectToAction("Users");
            }
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            UserEntity? user = new UserEntity();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "users/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserEntity>(data);
            }

            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            try
            {
                HttpResponseMessage response = client.DeleteAsync(client.BaseAddress + "users/" + id).Result;
                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction("Users");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Users");
            }
            return RedirectToAction("Users");
        }

        #endregion

        #region Appointments

        public IActionResult Appointment()
        {
            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
                ViewData["fullName"] = globalUser.Fullname;
                ViewData["email"] = globalUser.Email;
            }
            return View(new AppointmentEntity());
        }

        [HttpPost]
        public IActionResult Appointment(AppointmentEntity appointment)
        {
            bool admin = false;
            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                    admin = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
                ViewData["fullName"] = globalUser.Fullname;
                ViewData["email"] = globalUser.Email;
            }
            appointment.AppointmentDate = new DateTime(appointment.AppointmentDate.Ticks, DateTimeKind.Local);
            string data = JsonConvert.SerializeObject(appointment);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = client.PostAsync($"{client.BaseAddress}appointment/post", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    if (admin)
                    {
                        return RedirectToAction("Appointments");
                    }
                    return RedirectToAction("PatientAppointments");
                }
            }
            catch (Exception)
            {

                return View();
            }
            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> PatientAppointments(string email)
        {
            IList<AppointmentEntity>? appointments = new List<AppointmentEntity>();

            try
            {
                HttpResponseMessage getData = await client.GetAsync(client.BaseAddress + "appointment/by/" + JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")).Email);//StaticVariables.StaticVariables.User.Email.ToLower());

                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    appointments = JsonConvert.DeserializeObject<List<AppointmentEntity>>(results);
                }
                else
                {
                    Console.WriteLine("Error calling Web API");
                }
                ViewData.Model = appointments;
            }
            catch (Exception)
            {

                throw;
            }

            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Appointments()
        {
            IList<AppointmentEntity>? appointments = new List<AppointmentEntity>();

            try
            {
                HttpResponseMessage getData = await client.GetAsync("appointment");

                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    appointments = JsonConvert.DeserializeObject<List<AppointmentEntity>>(results);
                }
                else
                {
                    Console.WriteLine("Error calling Web API");
                }
                ViewData.Model = appointments;
            }
            catch (Exception)
            {

                throw;
            }

            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            return View(appointments);
        }

        [HttpGet]
        public IActionResult EditAppointment(int id)
        {
            AppointmentEntity appointment = new AppointmentEntity();
            HttpResponseMessage response = client.GetAsync($"{client.BaseAddress}appointment/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                appointment = JsonConvert.DeserializeObject<AppointmentEntity>(data);
                //StaticVariables.StaticVariables.Created = appointment.;
            }

            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            return View(appointment);
        }

        [HttpPost]
        public IActionResult EditAppointment(AppointmentEntity appointment)
        {
            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            string data = JsonConvert.SerializeObject(appointment);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = client.PutAsync($"{client.BaseAddress}appointment/{appointment.Id}", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    if (ViewData["admin"] != null && (bool)ViewData["admin"])
                    {
                        return RedirectToAction("Appointments");
                    }
                    return RedirectToAction("PatientAppointments");
                    //return RedirectToAction("Appointments");
                }

            }
            catch (Exception)
            {
                return RedirectToAction("Appointments");
            }
            return RedirectToAction("Appointments");
        }

        [HttpGet]
        public IActionResult DeleteAppointment(int id)
        {
            AppointmentEntity? appointment = new AppointmentEntity();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "appointment/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                appointment = JsonConvert.DeserializeObject<AppointmentEntity>(data);
            }

            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            return View(appointment);
        }

        [HttpPost, ActionName("DeleteAppointment")]
        public IActionResult DeleteAppointmentConfirm(int id)
        {
            if (JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User")) != null)
            {
                var globalUser = JsonConvert.DeserializeObject<UserEntity>(HttpContext.Session.GetString("Current User"));
                ViewData["loggedIn"] = true;
                ViewData["userName"] = globalUser.Fullname;
                if (globalUser.Email.ToLower().Contains("doctorsofamerica"))
                {
                    ViewData["admin"] = true;
                }
                else
                {
                    ViewData["admin"] = false;
                }
            }
            try
            {
                HttpResponseMessage response = client.DeleteAsync(client.BaseAddress + "appointment/delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    if(ViewData["admin"] != null && (bool)ViewData["admin"])
                    {
                        return RedirectToAction("Appointments");
                    }
                    return RedirectToAction("PatientAppointments");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Appointments");
            }
            return RedirectToAction("Appointments");
        }

        #endregion

        #region ErrorModel
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}