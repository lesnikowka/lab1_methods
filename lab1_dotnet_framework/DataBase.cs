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

        private List<string> columnNamesTest = new List<string> {"id", "x", "v", "v2", "v_v2", "loc_prec", "h", "c1", "c2", "u", "u_v" };
        private List<string> columnNamesMain1 = new List<string> { "id", "x", "v", "v2", "v_v2", "loc_prec", "h", "c1", "c2" };
        private List<string> columnNamesMain2 = new List<string> { "u2", "id", "x", "v", "v2", "v_v2", "loc_prec", "h", "c1", "c2" };
        private string columnNamesTestString = "";
        private string columnNamesMain1String = "";
        private string columnNamesMain2String = "";
        
        string BaseDirectory { get; }

        public DataBase(string directory)
        {
            BaseDirectory = directory;

            for (int i = 0; i < columnNamesTest.Count; i++)
            {
                columnNamesTestString += columnNamesTest[i];

                if (i != columnNamesTest.Count - 1)
                    columnNamesTestString += ", ";
            }

            for (int i = 0; i < columnNamesMain1.Count; i++)
            {
                columnNamesMain1String += columnNamesMain1[i];

                if (i != columnNamesMain1.Count - 1)
                    columnNamesMain1String += ", ";
            }

            for (int i = 0; i < columnNamesMain2.Count; i++)
            {
                columnNamesMain2String += columnNamesMain2[i];

                if (i != columnNamesMain2.Count - 1)
                    columnNamesMain2String += ", ";
            }

        }

        private SqliteConnection GetConnection()
        {
            string connectionString = "Data Source=" + Directory.GetCurrentDirectory() + BaseDirectory;

            SqliteConnection connection = null;

            try
            {
                connection = new SqliteConnection(connectionString);

                connection.Open();
            }
            catch
            {
                throw new Exception("База данных не найдена");
            }

            return connection;
        }

        public List<List<string>> GetAllStartConditions(string table)
        {
            SqliteConnection connection = GetConnection();

            SqliteCommand command = connection.CreateCommand();

            command.CommandText = "select distinct x0, u0 from " + table + ";";

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

            connection.Close();

            return allConditions;
        }
        

        public List<List<string>> GetDataForStartCondition(string table, List<string> startCondition)
        {
            SqliteConnection connection = GetConnection();

            SqliteCommand command = connection.CreateCommand();

            string columnNamesString;

            if (table == "test") 
            {
                columnNamesString = columnNamesTestString;
            }
            else if(table == "main1")
            {
                columnNamesString = columnNamesMain1String;
            }
            else { 
                columnNamesString = columnNamesMain2String; 
            }

            command.CommandText = "select " + columnNamesString + " from " + table + " where x0 = " + startCondition[0].Replace(",", ".") + " and u0 = " + startCondition[1].Replace(",", ".") + ";";

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

            connection.Close();

            return data;
        }

        //public void writeDataForStartCondition(string table, List<string> startCondition, List<List<string>> data)
        //{
        //    SqliteConnection connection = GetConnection();
        //
        //    SqliteCommand commandRemoving = connection.CreateCommand();
        //
        //    commandRemoving.CommandText = "delete from " + table + " where x0 = " + startCondition[0] + " and u0 = " + startCondition[1] + ";";
        //
        //    commandRemoving.ExecuteNonQuery();
        //
        //    for (int i = 0; i < data.Count; i++)
        //    {
        //
        //        SqliteCommand commandAdding = connection.CreateCommand();
        //
        //        commandAdding.CommandText = "insert into " + table + "(";
        //
        //        if (table == "test2")
        //        {
        //            commandAdding.CommandText += "u2, ";
        //        }
        //
        //
        //
        //        commandAdding.CommandText += "x0, u0, id, x, v, v2, v_v2, loc_prec, h, c1, c2";
        //
        //        if (table == "test")
        //        {
        //            commandAdding.CommandText += ", u, u_v";
        //        }
        //
        //        commandAdding.CommandText += ") values(";
        //
        //        commandAdding.CommandText += startCondition[0] + ", " + startCondition[1] + ", ";
        //
        //        for (int j = 0; j < data[i].Count; j++)
        //        {
        //            commandAdding.CommandText += data[i][j] + ", ";
        //        }
        //
        //        commandAdding.CommandText = commandAdding.CommandText.Substring(0, commandAdding.CommandText.Length - 1);
        //
        //        commandAdding.CommandText += ");";
        //
        //        commandAdding.ExecuteNonQuery();
        //    }
        //    
        //    connection.Close();
        //}
    }
}
    
