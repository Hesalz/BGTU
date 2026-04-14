using REPO;
using DALMSQLX;
using DALJSONX;
internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Init");
        DALMSQLX.Init.Execute();
        DALJSONX.Init.Execute();

        Console.WriteLine("Start");
        Console.WriteLine("");

        Console.WriteLine("\nTest DALMSQLX");
        TestDALMSQLX();

        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("------------------------------------------------");
        Console.WriteLine("");
        Console.WriteLine("");

        Console.WriteLine("Test DALJSONX");
        TestDALJSONX();

        Console.WriteLine("Finish");
        Console.ReadLine();
    }

    private static void TestDALMSQLX()
    {
        Console.WriteLine("------>Start \n");
        DALMSQLX.Init.Execute();
        Console.WriteLine("\n------>AfterExecute");


        using (REPO.REPO.IRepository repo = DALMSQLX.DALLMSQLX.Repository.Create())
        {
            repo.getAllComment().ForEach(comment => {
                Console.WriteLine($"{comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSrefID} ");
            });

            repo.getAllWSRef().ForEach(wsRef => {
                Console.WriteLine($"{wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
            });
            repo.addWSRef(new REPO.REPO.WSRef() { Url = "https://www.belstu.by/", Description = "БГТУ", Minus = 0, Plus = 1 });
            Console.WriteLine("-----------------------------------------------------------------");
            repo.getAllWSRef().ForEach(wsRef => {
                Console.WriteLine($"{wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
            });
            repo.getAllComment().ForEach(comment => {
                Console.WriteLine($"{comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSrefID} ");
            });
        }

        Console.ReadKey();
    }

    private static void TestDALJSONX()
    {
        Console.WriteLine("Init");

        Console.WriteLine("Start");
        using (REPO.REPO.IRepository repo = DALJSONX.DALJSONX.Repository.Create())
        {
            Console.WriteLine("All WSRef:");
            repo.getAllWSRef().ForEach(wsRef => {
                Console.WriteLine($"WSRef: {wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
            });

            Console.WriteLine("All Comments:");
            repo.getAllComment().ForEach(comment => {
                Console.WriteLine($"Comment {comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSrefID}");
            });

            var newWsRef = new REPO.REPO.WSRef() { Url = "https://www.example.com/", Description = "Example Website", Minus = 0, Plus = 0 };
            if (repo.addWSRef(newWsRef))
                Console.WriteLine("WSRef: Added");
            else
                Console.WriteLine("WSRef: Error Adding");

            var newComment = new REPO.REPO.Comment() { WSrefID = 1, Commtext = "Test comment", Stamp = DateTime.Now };
            if (repo.addComment(newComment))
                Console.WriteLine("Comment: Added");
            else
                Console.WriteLine("Comment: Error Adding");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("After Additions:");
            Console.WriteLine("");
            Console.WriteLine("");

            repo.getAllWSRef().ForEach(wsRef => {
                Console.WriteLine($"WSRef: {wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
            });

            repo.getAllComment().ForEach(comment => {
                Console.WriteLine($"Comment {comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSrefID}");
            });

        }
        Console.WriteLine("Finish");
        Console.ReadLine();
    }
}
