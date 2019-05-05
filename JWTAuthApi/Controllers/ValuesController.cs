using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JWTAuthApi.Models;
using JWTAuthApi.Services;
using System;
using Newtonsoft.Json;

namespace JWTAuthApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private MySqManager _mySqManager;
        private InfoMsgModel infoMsgModel = new InfoMsgModel();
        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "test" };
        }

        [Authorize]
        [HttpGet("{id}")]
        public string Get(string id)
        {
            _mySqManager = HttpContext.RequestServices.GetService(typeof(MySqManager)) as MySqManager;
            if (id == "get-best-five-messages")
            {
                try
                {
                    MessagesSorterModel messagesSorterModel = new MessagesSorterModel();
                    MessagesSorter messagesSorter = new MessagesSorter(messagesSorterModel);
                    messagesSorterModel = messagesSorter.MostImportantMessagesJson(_mySqManager.GetAllMessages());
                    if (messagesSorterModel.ToRemoveMessages.Count != 0)
                    {
                        _mySqManager.DeleteMessages(messagesSorterModel.ToRemoveMessages);
                    }

                    return messagesSorterModel.jsonResponse.ToString();
                }
                catch
                {
                    infoMsgModel.InfoMsg = "Wystąpił błąd pobierania pięciu wiadomości";
                    return "[" + JsonConvert.SerializeObject(infoMsgModel) + "]";
                }
            }
            else if (id == "get-all-messages")
            {
                try
                {
                    MessagesSorterModel messagesSorterModel = new MessagesSorterModel();
                    MessagesSorter messagesSorter = new MessagesSorter(messagesSorterModel);
                    return messagesSorter.AllMessagesJson(_mySqManager.GetAllMessages()).ToString();
                }
                catch
                {
                    infoMsgModel.InfoMsg = "Wystąpił błąd pobierania wszystkich wiadomości";
                    return JsonConvert.SerializeObject(infoMsgModel);
                }
            }
            else if (id == "get-all-users")
            {
                try
                {
                    UserToJson usersToJson = new UserToJson();
                    return usersToJson.AllUsersToJson(_mySqManager.GetAllUsers()).ToString();
                }
                catch
                {
                    infoMsgModel.InfoMsg = "Wystąpił błąd pobierania wszystkich użytkowników";
                    return JsonConvert.SerializeObject(infoMsgModel);
                }
            }
            else
            {
                infoMsgModel.InfoMsg = "Błędne id zapytania";
                return "[" + JsonConvert.SerializeObject(infoMsgModel) + "]";
            }

        }

        [Authorize]
        [HttpPost]
        [Route("new-user")]
        public string PostNewUser([FromBody] UserModel newUser)
        {
            _mySqManager = HttpContext.RequestServices.GetService(typeof(MySqManager)) as MySqManager;
            try
            {
                UserModel newUserTemp = _mySqManager.GetSpecificUserWithNoPassword(newUser);
                if (newUserTemp.UserId == 0)
                {
                    _mySqManager.AddNewUser(newUser);                
                    if (newUser.UserException == null)
                    {
                        infoMsgModel.InfoMsg = "Pomyślnie dodano nowego użytkownika";
                        return JsonConvert.SerializeObject(infoMsgModel);
                    }
                    else
                    {
                        infoMsgModel.InfoMsg = newUser.UserException;
                        return JsonConvert.SerializeObject(infoMsgModel);
                    }
                }
                else
                {
                    infoMsgModel.InfoMsg = "Użytkownik o podanym Loginie już istnieje";
                    return JsonConvert.SerializeObject(infoMsgModel);
                }

            }
            catch
            {
                infoMsgModel.InfoMsg = "Błąd dodawania nowego użytkownika";
                return JsonConvert.SerializeObject(infoMsgModel);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("new-message")]
        public string PostNewMessage([FromBody] MessageModel newMessage)
        {
            _mySqManager = HttpContext.RequestServices.GetService(typeof(MySqManager)) as MySqManager;
            try
            {
                newMessage.MessageBirth = DateTime.Now.ToString();

                if (newMessage.MessagePriority == null)
                {
                    newMessage.MessagePriority = "MEDIUM";
                }
                if (newMessage.MessageDeath == null)
                {
                    newMessage.MessageDeath = new DateTime(3000, 1, 1, 1, 1, 1).ToString("dd.MM.yyyy HH:mm:ss");
                }
                _mySqManager.AddNewMessages(newMessage);          
                if (newMessage.MessageException == null)
                {
                    infoMsgModel.InfoMsg = "Pomyślnie dodano nową wiadomość";
                    return "[" + JsonConvert.SerializeObject(infoMsgModel) + "]";
                }
                else
                {
                    infoMsgModel.InfoMsg = newMessage.MessageException;
                    return "[" + JsonConvert.SerializeObject(infoMsgModel) + "]";
                }
            }
            catch
            {
                infoMsgModel.InfoMsg = "Błąd dodawania nowej wiadomości";
                return "[" + JsonConvert.SerializeObject(infoMsgModel) + "]";
            }
        }

        [Authorize]
        [HttpPut]
        [Route("change-password")]
        public string PutChangePassword([FromBody] UserModel userToModify)
        {
            _mySqManager = HttpContext.RequestServices.GetService(typeof(MySqManager)) as MySqManager;
            try
            {
                UserModel userToModifyTemp = _mySqManager.GetSpecificUserWithPassword(userToModify);
                if (userToModifyTemp.UserId != 0)
                {
                    _mySqManager.ChangeUserPassword(userToModifyTemp);
                    if (userToModifyTemp.UserException == null)
                    {
                        infoMsgModel.InfoMsg = "Pomyślnie zmieniono hasło";
                        return JsonConvert.SerializeObject(infoMsgModel);
                    }
                    else
                    {
                        infoMsgModel.InfoMsg = userToModifyTemp.UserException;
                        return JsonConvert.SerializeObject(infoMsgModel);
                    }
                }
                else
                {
                    infoMsgModel.InfoMsg = "Nie znaleziono podanego użytkownika";
                    return JsonConvert.SerializeObject(infoMsgModel);
                }
            }
            catch
            {
                infoMsgModel.InfoMsg = "Błąd zmiany hasła";
                return JsonConvert.SerializeObject(infoMsgModel);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("delete-user")]
        public string DeleteUser([FromBody]UserModel userToRemove)
        {
            _mySqManager = HttpContext.RequestServices.GetService(typeof(MySqManager)) as MySqManager;
            try
            {
                UserModel userToRemoveTemp = _mySqManager.GetSpecificUserWithNoPassword(userToRemove);         
                if (userToRemoveTemp.UserId != 0)
                {
                    _mySqManager.DeleteUser(userToRemoveTemp);
                    if (userToRemoveTemp.UserException == null)
                    {
                        infoMsgModel.InfoMsg = "Pomyślnie usunięto użytkownika";
                        return JsonConvert.SerializeObject(infoMsgModel);
                    }
                    else
                    {
                        infoMsgModel.InfoMsg = userToRemoveTemp.UserException;
                        return JsonConvert.SerializeObject(infoMsgModel);
                    }
                }
                else
                {
                    infoMsgModel.InfoMsg = "Nie znaleziono użytkownika do usunięcia";
                    return JsonConvert.SerializeObject(infoMsgModel);
                }
            }
            catch
            {
                infoMsgModel.InfoMsg = "Błąd usówania użytkownika";
                return JsonConvert.SerializeObject(infoMsgModel);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("delete-message")]
        public string DeleteMessage([FromBody]MessageModel messageToRemove)
        {
            _mySqManager = HttpContext.RequestServices.GetService(typeof(MySqManager)) as MySqManager;
            try
            {
                List<MessageModel> messageToRemoveTempList = new List<MessageModel>();
                MessageModel messageToRemoveTemp = _mySqManager.GetMessage(messageToRemove);
                messageToRemoveTempList.Add(messageToRemoveTemp);
                if (messageToRemoveTempList[0].MessageId != 0)
                {
                    _mySqManager.DeleteMessages(messageToRemoveTempList);                             
                    infoMsgModel.InfoMsg = "Pomyślnie usunięto wiadomość";
                    return JsonConvert.SerializeObject(infoMsgModel);
                }
                else
                {
                    infoMsgModel.InfoMsg = "Nie udało się znaleźć wiadomości do usunięcia";
                    return JsonConvert.SerializeObject(infoMsgModel);
                }
            }
            catch
            {
                infoMsgModel.InfoMsg = "Błąd usówania wiadomości";
                return JsonConvert.SerializeObject(infoMsgModel);
            }
        }
    }
}
