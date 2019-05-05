using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using JWTAuthApi.Models;

namespace JWTAuthApi.Services
{
    public class UserToJson
    {       
        public UserToJson() { }

        public StringBuilder AllUsersToJson(List<UserModel> allUsers)
        {
            UserToJSONModel userToJSONModel;
            StringBuilder jsonAllUsers = new StringBuilder();
            if (allUsers.Count > 1)
            {
                jsonAllUsers.Append("[");
            }
            int i = 1;
            foreach (var user in allUsers)
            {
                userToJSONModel = new UserToJSONModel();
                userToJSONModel.UserLogin = user.UserLogin;
                userToJSONModel.UserRole = user.UserRole;
                jsonAllUsers.Append(JsonConvert.SerializeObject(userToJSONModel));
                if (i < allUsers.Count)
                {
                    jsonAllUsers.Append(",");
                }
                i++;
            }
            if (allUsers.Count > 1)
            {
                jsonAllUsers.Append("]");
            }

            return jsonAllUsers; 
        }
    }
}
