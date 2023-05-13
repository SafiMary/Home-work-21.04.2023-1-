using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace Home_work_21._04._2023_1_
{
    internal class Program
    {
        static SQLiteConnection connection;
        static SQLiteCommand command;
        static public bool Connect(string fileName)
        {
            try
            {
                connection = new SQLiteConnection("Data Source=" + fileName + ";Version=3; FailIfMissing=False");
                connection.Open();
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Ошибка доступа к базе данных. Исключение: {ex.Message}");
                return false;
            }
        }
        static void Main(string[] args)
        {
            if (Connect("firstBase.sqlite"))
            {
                Console.WriteLine("Connected");
                command = new SQLiteCommand(connection)
                {
                    CommandText = "CREATE TABLE IF NOT EXISTS [Person]([id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, [name] TEXT, [phone] byte, [email] TEXT);"
                };
                command.ExecuteNonQuery();  
                Console.WriteLine("Таблица создана");
            }
            Regex regexemail = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", RegexOptions.IgnoreCase);
            Regex regexphone = new Regex(@"(?:\+|\d)[\d\-\(\) ]{9,}\d", RegexOptions.IgnoreCase);
            Regex regexname = new Regex(@"^[a-zA-Z][a-zA-Z0]{3,10}$", RegexOptions.IgnoreCase);


            Console.ReadLine();
            foreach (var arg in args)
            {

                Match matchCollection1 = regexemail.Match(arg);
                Match matchCollection2 = regexphone.Match(arg);
                Match matchCollection3 = regexname.Match(arg);


                command.CommandText = "INSERT INTO Person (name, phone, email) VALUES (:name, :family, :age)";
                if (matchCollection3.Success)
                {
                    Console.WriteLine($"Имя {matchCollection3.Value}");
                    command.Parameters.AddWithValue("name", matchCollection3.Value);
                    matchCollection3 = matchCollection2.NextMatch();
                    
                }

                if (matchCollection2.Success)
                {
                    Console.WriteLine($"Номер мобильного телефона {matchCollection2.Value}");
                    command.Parameters.AddWithValue("phone", matchCollection2.Value);
                    matchCollection2 = matchCollection1.NextMatch();
                }

                if (matchCollection1.Success)
                {
                    Console.WriteLine($"Электронная почта {matchCollection1.Value}");
                    command.Parameters.AddWithValue("email", matchCollection1.Value);

                }


                command.ExecuteNonQuery();

                command.CommandText = "SELECT * FROM Person";
                DataTable data = new DataTable();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(data);
                Console.WriteLine($"Прочитано {data.Rows.Count} записей из таблицы БД");
                foreach (DataRow row in data.Rows)
                {
                    Console.WriteLine($"id = {row.Field<float>("id")} name = {row.Field<string>("name")} phone = {row.Field<byte>("phone")} email = {row.Field<string>("email")}");
                }
            }

        }
    }
}
