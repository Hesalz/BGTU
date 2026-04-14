using DAL_LES;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Repository INIT");
        Repository repo = Repository.Create();

        Console.WriteLine("\nAdd Celebrities");
        Celebrity celeb1 = new Celebrity { FullName = "Isaac Newton", Nationality = "UK" };
        Celebrity celeb2 = new Celebrity { FullName = "Nikola Tesla", Nationality = "RS" };
        Celebrity celeb3 = new Celebrity { FullName = "Stephen Hawking", Nationality = "UK" };

        Console.WriteLine($"Добавлен: {celeb1.FullName} -> {repo.AddCelebrity(celeb1)}");
        Console.WriteLine($"Добавлен: {celeb2.FullName} -> {repo.AddCelebrity(celeb2)}");
        Console.WriteLine($"Добавлен: {celeb3.FullName} -> {repo.AddCelebrity(celeb3)}");


        Console.WriteLine("\nGetAllCelebrities");
        List<Celebrity> celebrities = repo.GetAllCelebrities();
        celebrities.ForEach(c => Console.WriteLine($"{c.Id}: {c.FullName} ({c.Nationality})"));


        Console.WriteLine("\nAdd LifeEvents");
        LifeEvent event1 = new LifeEvent
        {
            CelebrityId = celeb1.Id,
            Date = new DateTime(1687, 7, 5),
            Description = "Открытие закона всемирного тяготения"
        };
        LifeEvent event2 = new LifeEvent
        {
            CelebrityId = celeb2.Id,
            Date = new DateTime(1891, 5, 20),
            Description = "Изобретение катушки Тесла"
        };
        LifeEvent event3 = new LifeEvent
        {
            CelebrityId = celeb3.Id,
            Date = new DateTime(1988, 4, 1),
            Description = "Публикация книги 'Краткая история времени'"
        };

        Console.WriteLine($"Событие: {event1.Description} -> {repo.AddLifeEvent(event1)}");
        Console.WriteLine($"Событие: {event2.Description} -> {repo.AddLifeEvent(event2)}");
        Console.WriteLine($"Событие: {event3.Description} -> {repo.AddLifeEvent(event3)}");



        Console.WriteLine("\nGetAllLifeEvents");
        List<LifeEvent> lifeEvents = repo.GetAllLifeEvents();
        lifeEvents.ForEach(l => Console.WriteLine($"{l.Id}: {l.Description} ({l.Date.ToShortDateString()})"));


        Console.WriteLine("\nGetCelebrityById");
        Celebrity? fetchedCeleb = repo.GetCelebrityById(celebrities[0].Id);
        Console.WriteLine($"Найден: {fetchedCeleb?.FullName}");


        Console.WriteLine("\nGetLifeEventById");
        LifeEvent? fetchedEvent = repo.GetLifeEventById(lifeEvents[0].Id);
        Console.WriteLine($"Событие: {fetchedEvent?.Description}");


        Console.WriteLine("\nGetCelebrityByLifeEventId");
        Celebrity celebByEvent = repo.GetCelebrityByLifeEventId(lifeEvents[0].Id);
        Console.WriteLine($"Связанная знаменитость: {celebByEvent.FullName}");


        Console.WriteLine("\nGetLifeEventsByCelebrityId");
        foreach (LifeEvent e in repo.GetLifeEventsByCelebrityId(celebrities[0].Id))
            Console.WriteLine($"Событие: {e.Description}");


        Console.WriteLine("\nUpdCelebrity");
        celeb1.FullName = "Isaac Newton Updated";
        bool updCeleb = repo.UpdCelebrity(celebrities[0].Id, celeb1);
        Console.WriteLine($"Обновлён? {updCeleb}");

        Console.WriteLine("\nUpdLifeEvent");
        event1.Description = "Обновлённое событие Ньютона";
        bool updEvent = repo.UpdLifeEvent(lifeEvents[0].Id, event1);
        Console.WriteLine($"Обновлено? {updEvent}");

        Console.WriteLine("\nDelLifeEvent");
        bool delEvent = repo.DelLifeEvent(lifeEvents[1].Id);
        Console.WriteLine($"Удалено событие? {delEvent}");

        Console.WriteLine("\nDelCelebrity");
        bool delCeleb = repo.DelCelebrity(celebrities[1].Id);
        Console.WriteLine($"Удалена знаменитость? {delCeleb}");

        Console.WriteLine("\nEnd");
        repo.Dispose();
    }
}