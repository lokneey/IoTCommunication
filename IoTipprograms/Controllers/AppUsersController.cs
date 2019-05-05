using System.Threading.Tasks;
using IoTipprograms.Models;
using IoTipprograms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IoTipprograms.Controllers
{
    public class AppUsersController : Controller
    {
        RequestManager requestManager = new RequestManager();

        public IActionResult Index()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("actualToken")))      
                {
                    return RedirectToAction("LogIn", "AppUsers");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return RedirectToAction("Error", "AppUsers");
            }
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(UserModel userInput)     
        {
            try
            {
                var task = Task.Run(() => requestManager.HttpUserComunication(userInput, "http://postiotapi.ipprograms.pl/api/login", "POST", HttpContext.Session.GetString("actualToken")));
                task.Wait();

                HttpContext.Session.SetString("actualToken", requestManager.token.token);
                if (requestManager.token.role != null)
                {
                    HttpContext.Session.SetString("actualRole", requestManager.token.role);
                }
                else
                {
                    HttpContext.Session.SetString("actualRole", "");
                }

                return RedirectToAction("Index", "AppUsers");
            }
            catch
            {
                return RedirectToAction("Error", "AppUsers");
            }
        }

        public IActionResult LogOut()
        {
            try
            {
                HttpContext.Session.SetString("actualToken", "");
                HttpContext.Session.SetString("actualRole", "");

                return RedirectToAction("LogIn", "AppUsers");
            }
            catch
            {
                return RedirectToAction("Error", "AppUsers");
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("actualToken")))     
            {
                return RedirectToAction("LogIn", "AppUsers");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult ChangePassword(UserModel userInput)
        {
            try
            {
                var task = Task.Run(() => requestManager.HttpUserComunication(userInput, "http://postiotapi.ipprograms.pl/api/values/change-password", "PUT", HttpContext.Session.GetString("actualToken")));
                task.Wait();

                return RedirectToAction("Index", "AppUsers");       
            }
            catch
            {
                return RedirectToAction("Error", "AppUsers");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserModel userInput)
        {
            try
            {
                var task = Task.Run(() => requestManager.HttpUserComunication(userInput, "http://postiotapi.ipprograms.pl/api/values/new-user", "POST", HttpContext.Session.GetString("actualToken")));
                task.Wait();

                return RedirectToAction("LogIn", "AppUsers");       
            }
            catch
            {
                return RedirectToAction("Error", "AppUsers");
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}