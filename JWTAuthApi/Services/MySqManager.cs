using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using JWTAuthApi.Models;

namespace JWTAuthApi.Services
{
    public class MySqManager
    {
        public string ConnectionString { get; set; }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public MySqManager(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public List<UserModel> GetAllUsers()
        {
            List<UserModel> usersList = new List<UserModel>();
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand("select * from users", connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usersList.Add(new UserModel()
                        {
                            UserId = Convert.ToInt32(reader["users_id"]),
                            UserLogin = reader["users_login"].ToString(),
                            UserPassword = reader["users_password"].ToString(),
                            UserRole = reader["users_role"].ToString()
                        });
                    }
                }
                connection.Close();
            }
            return usersList;
        }

        public UserModel GetSpecificUserWithPassword(UserModel oneUser)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand("select * from users where users_login='" + oneUser.UserLogin + "' and users_password='" + oneUser.UserPassword + "'", connection);

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        oneUser = new UserModel()
                        {
                            UserId = Convert.ToInt32(reader["users_id"]),
                            UserLogin = reader["users_login"].ToString(),
                            UserPassword = reader["users_password"].ToString(),
                            UserRole = reader["users_role"].ToString()
                        };
                    }
                }
                catch
                {
                    oneUser.UserException = "Wystąpił błąd wyszukiwania użytkownika!";
                }
                connection.Close();
            }
            if (oneUser.UserId == 0)
            {
                oneUser.UserException = "Nie znaleziono takiego użytkownika!";
            }
            return oneUser;
        }

        public UserModel GetSpecificUserWithNoPassword(UserModel oneUser)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                try
                {
                    MySqlCommand cmd = new MySqlCommand("select * from users where users_login='" + oneUser.UserLogin + "'", connection);

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        oneUser = new UserModel()
                        {
                            UserId = Convert.ToInt32(reader["users_id"]),
                            UserLogin = reader["users_login"].ToString(),
                            UserPassword = reader["users_password"].ToString(),
                            UserRole = reader["users_role"].ToString()
                        };
                    }

                }
                catch
                {
                    oneUser.UserException = "Wystąpił błąd wyszukiwania użytkownika!";
                }
                connection.Close();
            }
            if (oneUser.UserId == 0)
            {
                oneUser.UserException = "Nie znaleziono takiego użytkownika!";
            }

            return oneUser;
        }

        public void AddNewUser(UserModel user)
        {
            if (user.UserLogin != "")
            {
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand("insert into users(users_login, users_password, users_role) values('" + user.UserLogin + "', '" + user.UserPassword + "', '" + user.UserRole + "')", connection);
                    MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                    connection.Close();
                }
            }
            else
            {
                user.UserException = "Użytkownik o takim loginie już istnieje!";
            }
        }

        public void DeleteUser(UserModel user)
        {
            try
            {
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand("delete from users where users_id=" + user.UserId, connection);
                    MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                    connection.Close();
                }
            }
            catch
            {
                user.UserException = "Błąd przetwarzania usuwania użytkownika";
            }
        }

        public void ChangeUserPassword(UserModel user)
        {
            try
            {
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand("update users set users_password='" + user.UserNewPassword + "' where users_id=" + user.UserId, connection);
                    MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                    connection.Close();
                }
            }
            catch
            {
                user.UserException = "Błąd przetwarzania zmiany hasła";
            }
        }

        public List<MessageModel> GetAllMessages()
        {
            List<MessageModel> messageList = new List<MessageModel>();
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand("select * from message", connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messageList.Add(new MessageModel()
                        {
                            MessageId = Convert.ToInt32(reader["message_id"]),
                            MessageAuthor = reader["message_author"].ToString(),
                            MessageContent = reader["message_content"].ToString(),
                            MessagePriority = reader["message_priority"].ToString(),
                            MessageBirth = reader["message_birth"].ToString(),
                            MessageDeath = reader["message_death"].ToString()
                        });
                    }
                }
                connection.Close();
            }
            return messageList;
        }

        public void AddNewMessages(MessageModel message)
        {
            using (MySqlConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand("insert into message(message_content, message_priority, message_author, message_birth, message_death) values('" + message.MessageContent + "', '" + message.MessagePriority + "', '" + message.MessageAuthor + "', '" + message.MessageBirth + "', '" + message.MessageDeath + "')", connection);
                    MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                    connection.Close();
                }
                catch
                {
                    message.MessageException = "Nie udało się dodać nowej wiadomości";
                }
            }
        }

        public void AddNewMessages(List<MessageModel> messageList)
        {
            using (MySqlConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    foreach (MessageModel message in messageList)
                    {
                        MySqlCommand cmd = new MySqlCommand("insert into message(message_content, message_priority, message_author, message_birth, message_death) values('" + message.MessageContent + "', '" + message.MessagePriority + "', '" + message.MessageAuthor + "', '" + message.MessageBirth + "', '" + message.MessageDeath + "')", connection);
                        MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                    }
                    connection.Close();
                }
                catch
                {
                    foreach (MessageModel message in messageList)
                    {
                        message.MessageException = "Nie udało się dodać nowych wiadomości";
                    }
                }
            }
        }

        public MessageModel GetMessage(MessageModel message)
        {
            using (MySqlConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand("select * from message where message_content='" + message.MessageContent + "' and message_author='" + message.MessageAuthor + "'", connection);

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        message = new MessageModel()
                        {
                            MessageId = Convert.ToInt32(reader["message_id"]),
                            MessageContent = reader["message_content"].ToString(),
                            MessageAuthor = reader["message_author"].ToString(),
                            MessagePriority = reader["message_priority"].ToString(),
                            MessageBirth = reader["message_birth"].ToString(),
                            MessageDeath = reader["message_death"].ToString(),
                        };
                    }
                }
                catch
                {
                    message.MessageException = "Błąd przetwarzania pobierania wiadomości";
                }
                connection.Close();
            }
            if(message.MessageId == 0)
            {
                message.MessageException = "Nie udało się pobrać wiadomości";
            }
            return message;
        }

        public void DeleteMessages(List<MessageModel> messageList)
        {
            try
            {
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();
                    foreach (MessageModel message in messageList)
                    {
                        if (message.MessageId != 0)
                        {
                            MySqlCommand cmd = new MySqlCommand("delete from message where message_id=" + message.MessageId, connection);
                            MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                        }
                    }
                    connection.Close();
                }
            }
            catch
            {
                foreach (MessageModel message in messageList)
                {
                    message.MessageException = "Nie udało się usunąć wiadomości";
                }
            }
        }

        public void AddNewToken(TokenModelForStorageModel token)
        {
            try
            {
                using (MySqlConnection connection = GetConnection())
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand("insert into tokens(tokens_token, tokens_user) values('" + token.TokenToken + "', '" + token.TokenUser + "')", connection);
                    MySqlDataReader mySqlDataReader = cmd.ExecuteReader();
                    connection.Close();
                }
            }
            catch
            {
                token.TokenException = "Nie udało się dodać nowego tokena";
            }
        }
    }
}

