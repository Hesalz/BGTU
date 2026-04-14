using System;
using System.Reflection.Emit;
using DAL004;

Repository.JSONFileName = "Celebrities.json";
using (IRepository repository = new Repository("Celebrities"))
{

    void print(string label)
    {
        Console.WriteLine("—-- " + label + "----");
        foreach (Celebrity celebrity in repository.getAllCelebrities())
        {
            Console.WriteLine($"Id = {celebrity.Id}, Firstname = {celebrity.Firstname},Surname = {celebrity.Surname}, PhotoPath = {celebrity.PhotoPath}");
        }
    }
    ;
    print("start");
    int? testdel1 = repository.addCelebrity(new Celebrity(0, "TestDel1", "TestDell", "TestDel1.jpg"));
    int? testdel2 = repository.addCelebrity(new Celebrity(0, "TestDel2", "TestDel2", "TestDel2.jpg"));
    int? testupd1 = repository.addCelebrity(new Celebrity(0, "TestUpd1", "TestUpd1", "TestUpd1.jpg"));
    int? testupd2 = repository.addCelebrity(new Celebrity(0, "TestUpd2", "TestUpd2", "TestUpd2.jpg"));
    repository.SaveChanges();
    print("add 4");
    if (testdel1 != null)
        if (repository.delCelebrityById((int)testdel1)) Console.WriteLine($" delete {testdel1} "); else Console.WriteLine($"delete {testdel1} error");
    if (testdel2 != null)
        if (repository.delCelebrityById((int)testdel2)) Console.WriteLine($" delete {testdel2} "); else Console.WriteLine($"delete {testdel2} error");
    if (repository.delCelebrityById(1000)) Console.WriteLine($" delete {1000} "); else Console.WriteLine($"delete {1000} error");
    repository.SaveChanges();
    print("del 2");
    if (testupd1 != null)
        if (repository.updCelebrityById((int)testupd1, new Celebrity(0, "Updated1", "Updated1", "Updated1.jpg")) == null) Console.WriteLine($"update {testupd1} error");
        else Console.WriteLine($" update {testupd1} ");
    if (testupd2 != null)
        if (repository.updCelebrityById((int)testupd2, new Celebrity(0, "Updated2", "Updated2", "Updated2.jpg")) == null) Console.WriteLine($"update {testupd2} error");
        else Console.WriteLine($" update {testupd2} ");
    if (repository.updCelebrityById(1000, new Celebrity(0, "Updated1000", "Updated1000", "Updated1000.jpg")) == null) Console.WriteLine($"update {1000} error");
    else Console.WriteLine($" update {1000} ");
    repository.SaveChanges();
    print("upd 2");
}