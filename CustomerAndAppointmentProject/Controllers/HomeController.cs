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

        string BASEURL = "https://6506-2603-9001-3f00-2ac4-77ac-a753-ae0d-f488.ngrok-free.app/";
        //string BASEURL = "http://localhost:10888";

        HttpClient client = new HttpClient();
        DateTime UsersCreatedDate = DateTime.MinValue;
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

                HttpResponseMessage getData = await client.GetAsync("users/" + StaticVariables.StaticVariables.User.Id.ToString());
                if (getData.IsSuccessStatusCode)
                {
                    string results = getData.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<UserEntity>(results);
                }
            }
            catch(Exception ex) { 
            }
            return View(user);
        }

        public async Task<IActionResult> Users()
        {
            IList<UserEntity>? users = new List<UserEntity>();

            try
            {
                if (StaticVariables.StaticVariables.LoggedInAs.ToLower() == "admin")
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
            return View();
        }

        public IActionResult NewUser()
        {
            ViewData.Model = new UserEntity();
            return View(new UserEntity());
        }

        [HttpPost]
        public IActionResult NewUser(UserEntity user)
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
                    Console.WriteLine("Error calling Web API");
                }
            }
            catch (Exception)
            {

                throw;
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
                StaticVariables.StaticVariables.Created = user.Created;
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(UserEntity user)
        {
            if (StaticVariables.StaticVariables.Created != DateTime.MinValue && user.Created != StaticVariables.StaticVariables.Created)
            {
                user.Created = StaticVariables.StaticVariables.Created;
            }
            string data = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = client.PutAsync(client.BaseAddress + "users/" + user.Id, content).Result;

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
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {

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
            return View(new AppointmentEntity());
        }

        [HttpPost]
        public IActionResult Appointment(AppointmentEntity appointment)
        {
            appointment.AppointmentDate = new DateTime(appointment.AppointmentDate.Ticks, DateTimeKind.Local);
            string data = JsonConvert.SerializeObject(appointment);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = client.PostAsync($"{client.BaseAddress}appointment/post", content).Result;
                // Need to add a api call appointments/post so need a new controller on backend

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Appointments");
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
                HttpResponseMessage getData = await client.GetAsync(client.BaseAddress + "appointment/by/" + StaticVariables.StaticVariables.User.Email.ToLower());

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
            return View(appointment);
        }

        [HttpPost]
        public IActionResult EditAppointment(AppointmentEntity appointment)
        {
            //if (StaticVariables.StaticVariables.Created != DateTime.MinValue && user.Created != StaticVariables.StaticVariables.Created)
            //{
            //    user.Created = StaticVariables.StaticVariables.Created;
            //}
            string data = JsonConvert.SerializeObject(appointment);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = client.PutAsync(client.BaseAddress + "appointment/" + appointment.Id, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Appointments");
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
            return View(appointment);
        }

        [HttpPost, ActionName("DeleteAppointment")]
        public IActionResult DeleteAppointmentConfirm(int id)
        {

            try
            {
                HttpResponseMessage response = client.DeleteAsync(client.BaseAddress + "appointment/delete/" + id).Result;
                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction("Appointments");
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