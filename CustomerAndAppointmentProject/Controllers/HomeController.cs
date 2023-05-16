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
        private readonly ILogger<HomeController> _logger;

        string BASEURL = "https://2117-2603-9001-3f00-2ac4-597d-c83c-7c35-59f3.ngrok-free.app/";

        HttpClient client = new HttpClient();
        



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            client.BaseAddress = new Uri(BASEURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IActionResult> Index()
        {
           
            return View();
        }

        public async Task<IActionResult> Users()
        {
            IList<UserEntity> users = new List<UserEntity>();

            try
            {
                if (StaticVariables.StaticVariables.LoggedInAs == "ADMIN")
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

        public IActionResult Logout()
        {
            StaticVariables.StaticVariables.LoggedIn = false;
            StaticVariables.StaticVariables.User = new UserLoginEntity();
            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            ViewData.Model = new UserLoginEntity();
            return View(new UserLoginEntity());
        }

        [HttpPost]
        public IActionResult Login(UserLoginEntity entity)
        {
            StaticVariables.StaticVariables.LoggedIn = true;
            if (entity.Email.Contains("doctorsofamerica.com"))
            {
                StaticVariables.StaticVariables.LoggedInAs = "admin".ToUpper();
            }
            else
            {
                StaticVariables.StaticVariables.LoggedInAs = "patient".ToUpper();
            }
            StaticVariables.StaticVariables.User = entity;
            //StaticVariables.StaticVariables.User = //get UserEntity from api using entity credentials and set the global user to it
            return RedirectToAction("Index");
        }

        public IActionResult Appointment()
        {
            return View(new AppointmentEntity());
        }

        [HttpPost]
        public IActionResult Appointment(AppointmentEntity appointment)
        {
            string data = JsonConvert.SerializeObject(appointment);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = client.PostAsync($"{client.BaseAddress}appointments/post", content).Result;
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
        public async Task<IActionResult> Appointments()
        {
            IList<AppointmentEntity> appointments = new List<AppointmentEntity>();

            try
            {
                HttpResponseMessage getData = await client.GetAsync("appointments");

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

        public IActionResult NewUser()
        {
            ViewData.Model = new UserEntity();
            return View(new UserEntity());
        }

        [HttpPost]
        public IActionResult NewUser(UserEntity user)
        {
            user.Created = DateTime.Now;
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            UserEntity user = new UserEntity();
            HttpResponseMessage response = client.GetAsync(client.BaseAddress + "users/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserEntity>(data);
                DateTime created = user.Created;
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(UserEntity user)
        {
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
            UserEntity user = new UserEntity();
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
            catch(Exception)
            {
                return RedirectToAction("Users");
            }
            return RedirectToAction("Users");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}