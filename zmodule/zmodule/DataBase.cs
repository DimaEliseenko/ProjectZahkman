using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.OleDb;

namespace zmodule
{
    public static class DBProvider
    {
        public static User GetUser(string ConnectString, string firstName, string Surname) // проверка, есть ли такой пользователь в системе
        {
            User user = null;
            string selectStr = string.Format("SELECT * FROM users WHERE Имя='{0}' AND Фамилия='{1}'", firstName, Surname);
            using (OleDbConnection cnn = new OleDbConnection(ConnectString))
            {
                cnn.Open();
                using (OleDbCommand cmd = new OleDbCommand(selectStr, cnn))
                {
                    using (OleDbDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.HasRows)
                        {
                            return null;
                        }
                        dr.Read();
                        user = new User
                        {
                            FirstName = (string)dr["Имя"],
                            LastName = (string)dr["Фамилия"],
                            Group = (string)dr["Группа"],
                            Facultet = (string)dr["Факультет"],
                        };
                    }
                }
            }
            return user;
        }
    }

    public class User // получение информации о профиле
    {
        public string FirstName { get; set; } // Name
        public string LastName { get; set; } // Surname
        public string Group { get; set; }
        public string Facultet { get; set; }

        public override string ToString()
        {
            return string.Format("First name: {0}\nLast name: {1}\nGroup: {2}", FirstName, LastName, Group);
        }
    }
}
