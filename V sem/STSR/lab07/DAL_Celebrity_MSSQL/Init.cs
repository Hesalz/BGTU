using DAL_Celebrity_MSSQL.Models;
using DAL_Celebrity_MSSQL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL_Celebrity_MSSQL
{
    public class Init
    {
        static string connstring = string.Empty;

        public Init() { }
        public Init(string conn) { connstring = conn; }

        public static void Execute(bool delete = true, bool create = true)
        {
            using (var context = new Context(connstring))
            {
                try
                {
                    if (delete)
                    {
                        Console.WriteLine("Удаление базы данных...");
                        context.Database.EnsureDeleted();
                    }

                    if (create)
                    {
                        Console.WriteLine("Создание базы данных...");
                        context.Database.EnsureCreated();
                    }

                    if (context.Celebrities.Any())
                    {
                        Console.WriteLine("База данных уже содержит данные. Пропускаем заполнение.");
                        return;
                    }

                    Console.WriteLine("Заполнение базы данных...");
                    FillDatabase(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при инициализации БД: {ex.Message}");
                    throw;
                }
            }
        }

        private static void FillDatabase(Context context)
        {
            Func<string, string> puri = (string f) => $"{f}";

            // 1
            var c1 = new Celebrity() { FullName = "Noam Chomsky", Nationality = "US", ReqPhotoPath = puri("Chomsky.jpg") };
            context.Celebrities.Add(c1);
            context.Lifeevents.AddRange(
                new Lifeevent() { CelebrityId = 1, Date = new DateTime(1928, 12, 7), Description = "Дата рождения", ReqPhotoPath = null },
                new Lifeevent() { CelebrityId = 1, Date = new DateTime(1955, 1, 1), Description = "Издание книги \"Логическая структура лингвистической теории\"", ReqPhotoPath = null }
            );

            // 2
            var c2 = new Celebrity() { FullName = "Tim Berners-Lee", Nationality = "UK", ReqPhotoPath = puri("Berners-Lee.jpg") };
            context.Celebrities.Add(c2);
            context.Lifeevents.AddRange(
                new Lifeevent() { CelebrityId = 2, Date = new DateTime(1955, 6, 8), Description = "Дата рождения", ReqPhotoPath = null },
                new Lifeevent() { CelebrityId = 2, Date = new DateTime(1989, 6, 8), Description = "В CERN предложил \"Гиппертекстовый проект\"", ReqPhotoPath = null }
            );

            // 3
            var c3 = new Celebrity() { FullName = "Edgar Codd", Nationality = "US", ReqPhotoPath = puri("Codd.jpg") };
            context.Celebrities.Add(c3);
            context.Lifeevents.AddRange(
                new Lifeevent() { CelebrityId = 3, Date = new DateTime(1923, 8, 23), Description = "Дата рождения", ReqPhotoPath = null },
                new Lifeevent() { CelebrityId = 3, Date = new DateTime(2003, 4, 18), Description = "Дата смерти", ReqPhotoPath = null }
            );

            // 4
            var c4 = new Celebrity() { FullName = "Donald Knuth", Nationality = "US", ReqPhotoPath = puri("Knuth.jpg") };
            context.Celebrities.Add(c4);
            context.Lifeevents.AddRange(
                new Lifeevent() { CelebrityId = 4, Date = new DateTime(1938, 1, 10), Description = "Дата рождения", ReqPhotoPath = null },
                new Lifeevent() { CelebrityId = 4, Date = new DateTime(1974, 1, 1), Description = "Премия Тьюринга", ReqPhotoPath = null }
            );

            // 5
            var c5 = new Celebrity() { FullName = "Linus Torvalds", Nationality = "US", ReqPhotoPath = puri("Linus.jpg") };
            context.Celebrities.Add(c5);
            context.Lifeevents.AddRange(
                new Lifeevent() { CelebrityId = 5, Date = new DateTime(1969, 12, 28), Description = "Дата рождения. Финляндия.", ReqPhotoPath = null },
                new Lifeevent() { CelebrityId = 5, Date = new DateTime(1991, 9, 17), Description = "Выложил исходный код OS Linus (версии 0.01)", ReqPhotoPath = null }
            );

            // 6
            var c6 = new Celebrity() { FullName = "John Neumann", Nationality = "US", ReqPhotoPath = puri("Neumann.jpg") };
            context.Celebrities.Add(c6);
            context.Lifeevents.AddRange(
                new Lifeevent() { CelebrityId = 6, Date = new DateTime(1903, 12, 28), Description = "Дата рождения. Венгрия", ReqPhotoPath = null },
                new Lifeevent() { CelebrityId = 6, Date = new DateTime(1957, 2, 8), Description = "Дата смерти", ReqPhotoPath = null }
            );

            // 7
            var c7 = new Celebrity() { FullName = "Edsger Dijkstra", Nationality = "NL", ReqPhotoPath = puri("Dijkstra.jpg") };
            context.Celebrities.Add(c7);
            context.Lifeevents.AddRange(
                new Lifeevent() { CelebrityId = 7, Date = new DateTime(1930, 12, 28), Description = "Дата рождения", ReqPhotoPath = null },
                new Lifeevent() { CelebrityId = 7, Date = new DateTime(2002, 8, 6), Description = "Дата смерти", ReqPhotoPath = null }
            );

            // 8
            var c8 = new Celebrity() { FullName = "Ada Lovelace", Nationality = "UK", ReqPhotoPath = puri("Lovelace.jpg") };
            context.Celebrities.Add(c8);
            context.Lifeevents.AddRange(
                new Lifeevent() { CelebrityId = 8, Date = new DateTime(1815, 12, 10), Description = "Дата рождения", ReqPhotoPath = null },
                new Lifeevent() { CelebrityId = 8, Date = new DateTime(1852, 11, 27), Description = "Дата смерти", ReqPhotoPath = null }
            );

            // 9
            var c9 = new Celebrity() { FullName = "Charles Babbage", Nationality = "UK", ReqPhotoPath = puri("Babbage.jpg") };
            context.Celebrities.Add(c9);
            context.Lifeevents.AddRange(
                new Lifeevent() { CelebrityId = 9, Date = new DateTime(1791, 12, 26), Description = "Дата рождения", ReqPhotoPath = null },
                new Lifeevent() { CelebrityId = 9, Date = new DateTime(1871, 10, 18), Description = "Дата смерти", ReqPhotoPath = null }
            );

            // 10
            var c10 = new Celebrity() { FullName = "Andrew Tanenbaum", Nationality = "NL", ReqPhotoPath = puri("Tanenbaum.jpg") };
            context.Celebrities.Add(c10);
            context.Lifeevents.AddRange(
                new Lifeevent() { CelebrityId = 10, Date = new DateTime(1944, 3, 16), Description = "Дата рождения", ReqPhotoPath = null },
                new Lifeevent() { CelebrityId = 10, Date = new DateTime(1987, 1, 1), Description = "Cоздал OS MINIX — бесплатную Unix-подобную систему", ReqPhotoPath = null }
            );

            context.SaveChanges();
            Console.WriteLine("База данных успешно заполнена!");
        }
    }
}