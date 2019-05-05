using System.Threading.Tasks;
using IoTipprograms.Models;
using IoTipprograms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IoTipprograms.Controllers
{
    public class PostIotController : Controller
    {
        RequestManager requestManager = new RequestManager();

        [HttpGet]
        public IActionResult Message()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("actualToken")))      
            {
                return RedirectToAction("LogIn", "AppUsers");
            }
            else
            {
                try
                {
                    var task = Task.Run(() => requestManager.HttpMessageComunication(new MessageModel(), "http://postiotapi.ipprograms.pl/api/values/get-all-messages", "GET", HttpContext.Session.GetString("actualToken")));
                    task.Wait();

                    MessageModel messagesToDisplay = new MessageModel();
                    messagesToDisplay.MessagesList = requestManager.messagesList;

                    return View(messagesToDisplay);
                }
                catch
                {
                    return RedirectToAction("Error", "AppUsers");
                }
            }
        }

        [HttpPost]
        public IActionResult Message(MessageModel message)
        {
            try
            {
                var task = Task.Run(() => requestManager.HttpMessageComunication(message, "http://postiotapi.ipprograms.pl/api/values/new-message", "POST", HttpContext.Session.GetString("actualToken")));
                task.Wait();

                return RedirectToAction("Message", "PostIot");
            }
            catch
            {
                return RedirectToAction("Error", "AppUsers");
            }
        }

        [HttpPost]
        public IActionResult MessageNfc(MessageModel message)
        {
            //Field prepared for future implementation of NFC comunication

            return RedirectToAction("Message", "PostIot");
        }

        public IActionResult DeleteMessage(string messageAuthor, string messageContent)
        {
            try
            {
                MessageModel message = new MessageModel();
                message.MessageAuthor = messageAuthor;
                message.MessageContent = messageContent;
                var task = Task.Run(() => requestManager.HttpMessageComunication(message, "http://postiotapi.ipprograms.pl/api/values/delete-message", "PUT", HttpContext.Session.GetString("actualToken")));
                task.Wait();

                return RedirectToAction("message", "PostIot");
            }
            catch
            {
                return RedirectToAction("Error", "AppUsers");
            }
        }

        public IActionResult EditMessage(MessageModel message)
        {
            //Field prepared for future implementation

            return RedirectToAction("message", "PostIot");
        }

        [HttpGet]
        public IActionResult Users()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("actualToken")))      
            {
                return RedirectToAction("LogIn", "AppUsers");
            }
            else
            {
                try
                {
                    var task = Task.Run(() => requestManager.HttpUserComunication(new UserModel(), "http://postiotapi.ipprograms.pl/api/values/get-all-users", "GET", HttpContext.Session.GetString("actualToken")));
                    task.Wait();

                    UserModel usersToDisplay = new UserModel();
                    usersToDisplay.UsersList = requestManager.usersList;

                    return View(usersToDisplay);
                }
                catch
                {
                    return RedirectToAction("Error", "AppUsers");
                }
            }
        }

        [HttpPost]
        public IActionResult Users(UserModel newUser)
        {
            try
            {
                var task = Task.Run(() => requestManager.HttpUserComunication(newUser, "http://postiotapi.ipprograms.pl/api/values/new-user", "POST", HttpContext.Session.GetString("actualToken")));
                task.Wait();

                return RedirectToAction("Users", "PostIot");
            }
            catch
            {
                return RedirectToAction("Error", "AppUsers");
            }
        }

        public IActionResult DeleteUser(string userLogin)
        {
            try
            {
                UserModel user = new UserModel();
                user.UserLogin = userLogin;
                var task = Task.Run(() => requestManager.HttpUserComunication(user, "http://postiotapi.ipprograms.pl/api/values/delete-user", "PUT", HttpContext.Session.GetString("actualToken")));
                task.Wait();

                return RedirectToAction("Users", "PostIot");
            }
            catch
            {
                return RedirectToAction("Error", "AppUsers");
            }
        }

        public IActionResult EditeUser(UserModel user)
        {
            return RedirectToAction("ChangePassword", "AppUsers");
        }
    }
}