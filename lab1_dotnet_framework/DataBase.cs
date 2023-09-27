﻿using Microsoft.Data.Sqlite;
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

        private List<string> columnNames = new List<string> { "id", "x", "v", "v2", "v_v2", "loc_prec", "h", "c1", "c2", "u", "u_v" };
        string BaseDirectory { get; }

        public DataBase(string directory)
        {
            BaseDirectory = directory;
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

            string columnNamesTest = "";
            string columnNamesMain = "";

            for (int i = 0; i < columnNames.Count; i++)
            {
                columnNamesTest += columnNames[i];

                if (i != columnNames.Count - 1) 
                    columnNamesTest += ", ";
            }

            for (int i = 0; i < columnNames.Count - 2; i++)
            {
                columnNamesMain += columnNames[i];

                if (i != columnNames.Count - 3)
                    columnNamesMain += ", ";
            }

            if (table == "test") 
            {
                command.CommandText = "select " + columnNamesTest + " from " + table + " where x0 = " + startCondition[0] + " and u0 = " + startCondition[1] + ";";
            }
            else
            {
                command.CommandText = "select " + columnNamesMain + " from " + table + " where x0 = " + startCondition[0] + " and u0 = " + startCondition[1] + ";";
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

            connection.Close();

            return data;
        }

        public void writeDataForStartCondition(string table, List<string> startCondition, List<List<string>> data)
        {
            SqliteConnection connection = GetConnection();

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

                commandAdding.CommandText += startCondition[0] + ", " + startCondition[1] + ", ";

                for (int j = 0; j < data[i].Count; j++)
                {
                    commandAdding.CommandText += data[i][j] + ", ";
                }

                commandAdding.CommandText = commandAdding.CommandText.Substring(0, commandAdding.CommandText.Length - 1);

                commandAdding.CommandText += ");";

                commandAdding.ExecuteNonQuery();
            }
            
            connection.Close();
        }
    }
}
    
