using DAL;
internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("------>Start \n");
        Init.Execute();
        Console.WriteLine("\n------>AfterExecute");


        using (IRepository repo = Repository.Create())
        {
            repo.getAllComment().ForEach(comment => {
                Console.WriteLine($"{comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSrefID} ");
            });

            repo.getAllWSRef().ForEach(wsRef => {
                Console.WriteLine($"{wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
            });
            repo.addWSRef(new WSRef() { Url = "https://www.belstu.by/", Description = "БГТУ", Minus = 0, Plus = 1 });
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
}