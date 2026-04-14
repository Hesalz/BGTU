using DAL_Celebrity_MSSQL;

internal class Program
{
    private static void Main(string[] args)
    {
        string CS = @"Data Source=PC;Initial Catalog=Celebrity;Integrated Security=True;Trust Server Certificate=True";
        Init init = new Init(CS);
        Init.Execute(delete: true, create: true);

        Func<Celebrity, string> printC = (c) => $"Id = {c.Id}, FullName = {c.FullName}, Nationality = {c.Nationality}, ReqPhotoPath = {c.ReqPhotoPath}";
        Func<Lifeevent, string> printL = (l) => $"Id = {l.Id}, CelebrityId = {l.CelebrityId}, Date = {l.Date}, Description = {l.Description}, ReqPhotoPath = {l.ReqPhotoPath}";
        Func<string, string> puri = (string f) => $"{f}";


        using (IRepository repo = Repository.Create(CS))
        {
            {
                Console.WriteLine("------ GetAllCelebrities() ------------");
                repo.GetAllCelebrities().ForEach(celebrity => Console.WriteLine(printC(celebrity)));
            }

            {
                Console.WriteLine("------ GetAllLifeevents() ------------");
                repo.GetAllLifeevents().ForEach(life => Console.WriteLine(printL(life)));
            }

            {
                Console.WriteLine("------ AddCelebrity() ------------");
                Celebrity c = new Celebrity { FullName = "Albert Einstein", Nationality = "DE", ReqPhotoPath = puri("Einstein.jpg")};
                if (repo.AddCelebrity(c)) Console.WriteLine($"OK: AddCelebrity: {printC(c)}");
                else Console.WriteLine($"ERROR:AddCelebrity: {printC(c)}");
            }

            {
                Console.WriteLine("------ AddCelebrity() ------------");
                Celebrity c = new Celebrity { FullName = "Samuel Huntington", Nationality = "US", ReqPhotoPath = puri("Huntington.jpg") };
                if (repo.AddCelebrity(c)) Console.WriteLine($"OK: AddCelebrity: {printC(c)}");
                else Console.WriteLine($"ERROR:AddCelebrity: {printC(c)}");
            }

            {
                Console.WriteLine("------ DelCelebrity() ------------");
                int id = 0;
                if ((id = repo.GetCelebrityIdByName("Einstein")) > 0) { 
                    if (repo.DelCelebrity(id)) Console.WriteLine($"OK: DelCelebrity: {id}"); 
                    else Console.WriteLine($"ERROR: DelCelebrity: {id}");
                }
                else Console.WriteLine($"ERROR: GetCelebrityIdByName");
            }


            {
                Console.WriteLine("------ UpdCelebrity() ------------");
                int id = 0;
                if ((id = repo.GetCelebrityIdByName("Huntington")) > 0) { 
                    Celebrity? c = repo.GetCelebrityById(id);
                    if(c != null)
                    {
                        c.FullName = "Samuel Phillips Huntington";
                        if (repo.UpdCelebrity(c.Id, c) != null) Console.WriteLine($"UpdCelebrity: {id}, {printC(c)}");
                        Console.WriteLine($"GetCelebrityById, {printC(repo.GetCelebrityById(c.Id))}");
                    }
                    else Console.WriteLine($"ERROR: GetCelebrityById: {id}");
                }
                else Console.WriteLine($"ERROR: GetCelebrityIdByName");
            }

            {
                Console.WriteLine("------ AddLifeevent() ------------");
                int id = 0;
                if ((id = repo.GetCelebrityIdByName("Huntington")) > 0)
                {
                    Lifeevent l = new Lifeevent { CelebrityId = 12, Date = new DateTime(1927, 4, 18), Description = "Дата рождения", ReqPhotoPath = null};
                    Lifeevent l2 = new Lifeevent { CelebrityId = 12, Date = new DateTime(1927, 4, 18), Description = "Дата рождения", ReqPhotoPath = null };
                    Lifeevent l3 = new Lifeevent { CelebrityId = 12, Date = new DateTime(2008, 12, 24), Description = "Дата рождения", ReqPhotoPath = null };
                    if (repo.AddLifeevent(l)) Console.WriteLine($"AddLifeevent, {printL(l)}");
                    if (repo.AddLifeevent(l2)) Console.WriteLine($"AddLifeevent, {printL(l2)}");
                    if (repo.AddLifeevent(l3)) Console.WriteLine($"AddLifeevent, {printL(l3)}");
                }
                else Console.WriteLine($"ERROR: GetCelebrityIdByName");
            }

            {
                Console.WriteLine("------ DelLifeevent() ------------");
                int id = 22;
                if (repo.DelLifeevent(id)) Console.WriteLine($"OK: DelLifeevent: {id}");
                else Console.WriteLine($"ERROR: DelLifeevent: {id}");
            }

            {
                Console.WriteLine("------ UpdLifeevent() ------------");
                int id = 23;
                Lifeevent? l1;
                if ((l1 = repo.GetLifeeventById(id)) != null)
                {
                    l1.Description = "Дата смерти";
                    if (repo.UpdLifeevent(id, l1)) Console.WriteLine($"OK:UpdLifeevent {id}, {printL(l1)}");
                    else Console.WriteLine($"ERROR:UpdLifeevent {id}, {printL(l1)}");
                }
            }

            {
                Console.WriteLine("------ GetLifeeventSByCelebrityId() ------------");
                int id = 0;
                if((id = repo.GetCelebrityIdByName("Huntington")) > 0)
                {
                    Celebrity? c = repo.GetCelebrityById(id);
                    if (c != null) repo.GetLifeeventsByCelebrityId(c.Id).ForEach(l => Console.WriteLine($"OK: GetLifeeventsByCelebrityId, {id}, {printL(l)}"));
                    else Console.WriteLine($"ERROR: GetLifeeventsByCelebrityId: {id}");
                }
                else Console.WriteLine($"ERROR: GetCelebrityIdByName");
            }

            {
                Console.WriteLine("------ GetCelebrityByLifeeventId() ------------");
                int id = 23;
                Celebrity? c;
                if ((c = repo.GetCelebrityByLifeeventId(id)) != null) Console.WriteLine($"OK: {printC(c)}");
                else Console.WriteLine($"ERROR: GetCelebrityByLifeeventId, {id}");
            }

        }
        Console.WriteLine("----------->"); Console.ReadKey();

    }
}