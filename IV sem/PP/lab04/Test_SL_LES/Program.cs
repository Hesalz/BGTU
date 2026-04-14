using CashCelebrities;
using DAL_LES;
using Microsoft.Extensions.DependencyInjection;
using ServiceLocator;

internal class Program
{
    private static async Task Main()
    {
        TimeSpan cacheDuration = TimeSpan.FromSeconds(3);

        TransientLocator.Register<IRepository>(() => Repository.Create());
        TransientLocator.Register<ICelebrity>(() => Repository.Create());
        TransientLocator.Register<ILifeEvent>(() => Repository.Create());
        TransientLocator.Register<ICommon>(() => Repository.Create());

        CashCelebrities.ScopeLocator.Register<ICashCelebrities>(duration =>
        {
            IRepository repo = TransientLocator.Resolve<IRepository>();
            return new CashCelebrityScope(duration, repo);
        });

        IServiceScopeFactory scopeFactory = CashCelebrities.ScopeLocator.CreateServiceScopeFactory(cacheDuration);

        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            ICashCelebrities cashCeleb = scope.ServiceProvider.GetService<ICashCelebrities>();
            cashCeleb?.GetCelebrities();
            cashCeleb?.PrintCashCelebrities();

            Console.WriteLine("\nДобавляем новых знаменитостей");

            using (IRepository repo = TransientLocator.Resolve<IRepository>())
            {
                foreach (var c in repo.GetAllCelebrities()) repo.DelCelebrity(c.Id);

                Celebrity c1 = new Celebrity { FullName = "Celeb1", Nationality = "Brazil" };
                Celebrity c2 = new Celebrity { FullName = "Celeb2", Nationality = "Belarus" };

                repo.AddCelebrity(c1);
                repo.AddCelebrity(c2);

                Console.WriteLine("Знаменитости добавлены.");

                repo.GetAllCelebrities().ForEach(c => Console.WriteLine($"{c.Id}: {c.FullName} - {c.Nationality}"));
            }

            Console.WriteLine("\n\nICashCelebrities после добавления");
            cashCeleb?.GetCelebrities();
            cashCeleb?.PrintCashCelebrities();

            Console.WriteLine($"\nЖдём {cacheDuration.TotalSeconds} секунд...");
            await Task.Delay(cacheDuration);

            Console.WriteLine("\n\nICashCelebrities после истечения кэша");
            var refreshedCashCeleb = scope.ServiceProvider.GetService<ICashCelebrities>();
            refreshedCashCeleb?.GetCelebrities();
            refreshedCashCeleb?.PrintCashCelebrities();
        }

        Console.WriteLine("\nEnd.");
        Console.ReadLine();
    }
}
