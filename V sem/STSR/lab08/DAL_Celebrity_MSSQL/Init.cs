using DAL_Celebrity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_Celebrity_MSSQL
{
    public class Init
    {
        public static void Execute(string connectionString, bool delete = true, bool create = true)
        {
            // Создаем базу данных, если её нет
            if (create)
            {
                EnsureDatabaseExists(connectionString);
            }

            Context context = new Context(connectionString);
            if (delete) //если есть БД то она удаляется
            {
                context.Database.EnsureDeleted();
            }
            if (create)// если нет бд создается
            {
                context.Database.EnsureCreated();
            }
            
            // Проверяем, есть ли уже данные в базе
            if (context.Celebrities.Any())
            {
                return; // Данные уже есть, не добавляем повторно
            }
            
            Func<string, string> puri = (f) => $"{f}";

            {//1
                Celebrity c = new Celebrity() { FullName = "Noam Chomsky", Nationality = "US", ReqPhotoPath = puri("Chomsky.jpg") };
                LifeEvent l1 = new LifeEvent() { CelebrityId = 1, Date = new DateTime(1928, 12, 7), Description = "Дата Рождения", ReqPhotoPath = null };
                LifeEvent l2 = new LifeEvent() { CelebrityId = 1, Date = new DateTime(1955, 1, 1), Description = "Издание книги \"Логическая структура лингвистической теории\"", ReqPhotoPath = null };

                context.Celebrities.Add(c);
                context.LifeEvents.Add(l1);
                context.LifeEvents.Add(l2);
            }
            {//2
                Celebrity c = new Celebrity() { FullName = "Tim Berners-Lee", Nationality = "US", ReqPhotoPath = puri("Berners-Lee.jpg") };
                LifeEvent l1 = new LifeEvent() { CelebrityId = 2, Date = new DateTime(1955, 6, 8), Description = "Дата Рождения", ReqPhotoPath = null };
                LifeEvent l2 = new LifeEvent() { CelebrityId = 2, Date = new DateTime(1989, 6, 8), Description = "В CERN предложил \"Гиппертекстовый проект\"", ReqPhotoPath = null };

                context.Celebrities.Add(c);
                context.LifeEvents.Add(l1);
                context.LifeEvents.Add(l2);
            }
            {//3
                Celebrity c = new Celebrity() { FullName = "Edgar Codd", Nationality = "US", ReqPhotoPath = puri("Codd.jpg") };
                LifeEvent l1 = new LifeEvent() { CelebrityId = 3, Date = new DateTime(1923, 8, 23), Description = "Дата Рождения", ReqPhotoPath = null };
                LifeEvent l2 = new LifeEvent() { CelebrityId = 3, Date = new DateTime(2003, 4, 18), Description = "Дата смерти", ReqPhotoPath = null };

                context.Celebrities.Add(c);
                context.LifeEvents.Add(l1);
                context.LifeEvents.Add(l2);
            }
            {//4
                Celebrity c = new Celebrity() { FullName = "Donlad Knuth", Nationality = "US", ReqPhotoPath = puri("Knuth.jpg") };
                LifeEvent l1 = new LifeEvent() { CelebrityId = 4, Date = new DateTime(1938, 1, 10), Description = "Дата Рождения", ReqPhotoPath = null };
                LifeEvent l2 = new LifeEvent() { CelebrityId = 4, Date = new DateTime(1974, 1, 1), Description = "Премия Тьюринга", ReqPhotoPath = null };

                context.Celebrities.Add(c);
                context.LifeEvents.Add(l1);
                context.LifeEvents.Add(l2);
            }
            {//5
                Celebrity c = new Celebrity() { FullName = "Linus Torvalds", Nationality = "US", ReqPhotoPath = puri("Linus.jpg") };
                LifeEvent l1 = new LifeEvent() { CelebrityId = 5, Date = new DateTime(1969, 12, 7), Description = "Дата Рождения. Finland", ReqPhotoPath = null };
                LifeEvent l2 = new LifeEvent() { CelebrityId = 5, Date = new DateTime(1991, 9, 17), Description = "Выложил исходный код OS Linus (версия 0.01)", ReqPhotoPath = null };

                context.Celebrities.Add(c);
                context.LifeEvents.Add(l1);
                context.LifeEvents.Add(l2);
            }
            {//6
                Celebrity c = new Celebrity() { FullName = "John Neumann", Nationality = "US", ReqPhotoPath = puri("Neumann.jpg") };
                LifeEvent l1 = new LifeEvent() { CelebrityId = 6, Date = new DateTime(1903, 12, 28), Description = "Дата Рождения. Венгрия", ReqPhotoPath = null };
                LifeEvent l2 = new LifeEvent() { CelebrityId = 6, Date = new DateTime(1957, 2, 8), Description = "Дата смерти", ReqPhotoPath = null };

                context.Celebrities.Add(c);
                context.LifeEvents.Add(l1);
                context.LifeEvents.Add(l2);
            }
            {//7
                Celebrity c = new Celebrity() { FullName = "Edsger Dijsktra", Nationality = "NL", ReqPhotoPath = puri("Dijkstra.jpg") };
                LifeEvent l1 = new LifeEvent() { CelebrityId = 7, Date = new DateTime(1930, 5, 11), Description = "Дата Рождения", ReqPhotoPath = null };
                LifeEvent l2 = new LifeEvent() { CelebrityId = 7, Date = new DateTime(2002, 8, 6), Description = "Дата смерти", ReqPhotoPath = null };

                context.Celebrities.Add(c);
                context.LifeEvents.Add(l1);
                context.LifeEvents.Add(l2);
            }
            {//8
                Celebrity c = new Celebrity() { FullName = "Ada Lovelace", Nationality = "UK", ReqPhotoPath = puri("Lovelace.jpg") };
                LifeEvent l1 = new LifeEvent() { CelebrityId = 8, Date = new DateTime(1852, 11, 27), Description = "Дата Рождения", ReqPhotoPath = null };
                LifeEvent l2 = new LifeEvent() { CelebrityId = 8, Date = new DateTime(1915, 12, 10), Description = "Дата смерти", ReqPhotoPath = null };

                context.Celebrities.Add(c);
                context.LifeEvents.Add(l1);
                context.LifeEvents.Add(l2);
            }
            {//9
                Celebrity c = new Celebrity() { FullName = "Charles Babbage", Nationality = "UK", ReqPhotoPath = puri("Babbage.jpg") };
                LifeEvent l1 = new LifeEvent() { CelebrityId = 9, Date = new DateTime(1791, 12, 26), Description = "Дата Рождения", ReqPhotoPath = null };
                LifeEvent l2 = new LifeEvent() { CelebrityId = 9, Date = new DateTime(1871, 10, 18), Description = "Дата смерти", ReqPhotoPath = null };

                context.Celebrities.Add(c);
                context.LifeEvents.Add(l1);
                context.LifeEvents.Add(l2);
            }
            {//10
                Celebrity c = new Celebrity() { FullName = "Andrew Tanenbaum", Nationality = "NL", ReqPhotoPath = puri("Tanenbaum.jpg") };
                LifeEvent l1 = new LifeEvent() { CelebrityId = 10, Date = new DateTime(1944, 3, 16), Description = "Дата Рождения", ReqPhotoPath = null };
                LifeEvent l2 = new LifeEvent() { CelebrityId = 10, Date = new DateTime(1987, 1, 1), Description = "Создал IS MINIX - бесплатную Unix-подобную систему", ReqPhotoPath = null };

                context.Celebrities.Add(c);
                context.LifeEvents.Add(l1);
                context.LifeEvents.Add(l2);
            }

            context.SaveChanges();
        }

        private static void EnsureDatabaseExists(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            string databaseName = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"
                        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
                        BEGIN
                            CREATE DATABASE [{databaseName}]
                        END";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
