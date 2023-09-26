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
        private SqliteConnection connection;

        public DataBase(string directory)
        {
            string connectionString = "Data Source=" + Directory.GetCurrentDirectory() + directory;

            try
            {
                connection = new SqliteConnection(connectionString);
            }
            catch
            {
                throw new Exception("База данных не найдена");
            }

            connection.Open();
        }

        ~DataBase()
        {
            connection.Close();
        }

        List<List<string>> GetAllStartConditions(string table)
        {
            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "select distinct u0, v0 from " + table + ";";

            SqliteDataReader reader = command.ExecuteReader();

            List<List<string>> allConditions =  new List<List<string>>();

            while (reader.Read()) {
                List<string> currentRow = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    currentRow.Add(reader.GetString(i));
                }

                allConditions.Add(currentRow);
            }

            return allConditions;
        }
        

        public List<List<string>> GetDataForStartCondition(string table, List<string> startCondition)
        {
            SqliteCommand command = connection.CreateCommand();

            if (table == "test") 
            {
                command.CommandText = "select id, x, v, v2, v_v2, loc_prec, h, c1, c2, u, u_v from " + table + " where x0 == " + startCondition[0] + " and u0 == " + startCondition[1] + ";";
            }
            else
            {
                command.CommandText = "select id, x, v, v2, v_v2, loc_prec, h, c1, c2 from " + table + " where x0 == " + startCondition[0] + " and u0 == " + startCondition[1] + ";";
            }

            SqliteDataReader reader =  command.ExecuteReader();

            List<List<string>> data = new List<List<string>>(); 

            while (reader.Read())
            {
                List<string> current  = new List<string>(); 

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    current.Add(reader.GetString(i));   
                }

                data.Add(current);  
            }

            return data;
        }

        public void writeDataForStartCondition(string table, List<string> startCondition, List<List<string>> data)
        {
            SqliteCommand commandRemoving = connection.CreateCommand();

            commandRemoving.CommandText = "delete from " + table + " where x0 = " + startCondition[0] + " and u0 = " + startCondition[1] + ";";

            commandRemoving.ExecuteNonQuery();

            for (int i = 0; i < data.Count; i++)
            {

                SqliteCommand commandAdding = connection.CreateCommand();

                commandAdding.CommandText = "insert into " + table + "(x0, u0, id, x, v, v2, v_v2, loc_prec, h, c1, c2";

                if (table == "test")
                {
                    commandAdding.CommandText += ", u, u_v";
                }

                commandAdding.CommandText += ") values(";

                for (int j = 0; j < data[i].Count; j++)
                {
                    commandAdding.CommandText += data[i][j] + ", ";
                }

                commandAdding.CommandText = commandAdding.CommandText.Substring(0, commandAdding.CommandText.Length - 1);

                commandAdding.CommandText += ");";

                commandAdding.ExecuteNonQuery();
            }
            
        }
    }
}
    
