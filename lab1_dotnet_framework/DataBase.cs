using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_dotnet_framework
{
    internal class DataBase
    {
        public DataBase(string directory) {
            string connectionString = "Data Source=" + directory;

            var connection = new SqliteConnection(connectionString);

            connection.Open();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "select * from test;";

        }
    }
}
