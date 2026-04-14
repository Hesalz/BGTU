using static DALJSONX.DALJSONX;
using static REPO.REPO;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Init");

        Console.WriteLine("Start");
        using (IRepository repo = Repository.Create())
        {
            Console.WriteLine("All WSRef:");
            repo.getAllWSRef().ForEach(wsRef => {
                Console.WriteLine($"WSRef: {wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
            });

            Console.WriteLine("All Comments:");
            repo.getAllComment().ForEach(comment => {
                Console.WriteLine($"Comment {comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSrefID}");
            });

            var newWsRef = new WSRef() { Url = "https://www.example.com/", Description = "Example Website", Minus = 0, Plus = 0 };
            if (repo.addWSRef(newWsRef))
                Console.WriteLine("WSRef: Added");
            else
                Console.WriteLine("WSRef: Error Adding");

            var newComment = new Comment() { WSrefID = 1, Commtext = "Test comment", Stamp = DateTime.Now };
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
