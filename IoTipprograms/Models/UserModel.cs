using System.Collections.Generic;

namespace IoTipprograms.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string UserLogin { get; set; }
        public string UserPassword { get; set; }
        public string UserNewPassword { get; set; }
        public string UserRole { get; set; }
        public string UserException { get; set; }
        public List<UserModel> UsersList;
    }
}
