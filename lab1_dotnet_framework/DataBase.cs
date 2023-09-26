using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace lab1_dotnet_framework
{
    internal class DataBase
    {
        public DataBase(string directory)
        {
            string connectionString = "Data Source=" + Directory.GetCurrentDirectory() + directory;


            SqliteConnection connection;

            connection = new SqliteConnection(connectionString);


            try
            {
                connection = new SqliteConnection(connectionString);
            }
            catch
            {
                throw new Exception("База данных не найдена");
            }

            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "select * from test;";

            SqliteDataReader reader = command.ExecuteReader();

            List<string> data = new List<string>();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string tmp = reader.GetString(i);
                }
            }



        }
    }
}
    
