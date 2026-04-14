using GREPO;
using DALMSQLXG;
using DALJSONXG;

namespace Test_XXG
{
    public class Test_XXG
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Init");  
            DALMSQLXG.Init.Execute(); 

            Console.WriteLine("Start");  
            Console.WriteLine("");  

            Console.WriteLine("\nTest DALMSQLX");  
            using (DALMSQLXG.IRepository repo = DALMSQLXG.Repository.Create())  
            {
                Console.WriteLine("------>Start \n");  
                Console.WriteLine("\n------>AfterExecute");  

                repo.getAllComment().ForEach(comment => {  
                    Console.WriteLine($"{comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSrefID} "); 
                });

                repo.getAllWSRef().ForEach(wsRef => {  
                    Console.WriteLine($"{wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}"); 
                });

                repo.addWSRef(new DALMSQLXG.WSRef { Url = "google.com", Minus = 0, Plus = 0 });
                repo.addComment(new DALMSQLXG.Comment { WSrefID = 4, Stamp = DateTime.UtcNow, Commtext = "Текст изменен" });
                Console.WriteLine("-----------------------------------------------------------------"); 


                repo.getAllComment().ForEach(comment => { 
                    Console.WriteLine($"{comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSrefID} "); 
                });

                repo.getAllWSRef().ForEach(wsRef => { 
                    Console.WriteLine($"{wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");
                });
            }

            Console.WriteLine("");
            Console.WriteLine(""); 
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine(""); 
            Console.WriteLine("");  

            Console.WriteLine("Test DALJSONX");
            DALJSONXG.Init.Execute();
            using (DALJSONXG.IRepository repo = DALJSONXG.Repository.Create("WSRef.json")) 
            {
                Console.WriteLine("All WSRef:");
                repo.getAllWSRef().ForEach(wsRef => { 
                    Console.WriteLine($"WSRef: {wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}");  
                });

                Console.WriteLine("All Comments:");
                repo.getAllComment().ForEach(comment => {
                    Console.WriteLine($"Comment {comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSrefId}");
                });

                repo.addWSRef(new DALJSONXG.WSRef { Url = "belstu.by", Minus = 0, Plus = 0 });
                repo.addComment(new DALJSONXG.Comment { WSrefId = 6, Stamp = DateTime.UtcNow, Commtext = "Текст изменен" }); 

                Console.WriteLine("");
                Console.WriteLine(""); 
                Console.WriteLine("After Additions:"); 
                Console.WriteLine(""); 
                Console.WriteLine("");

                repo.getAllWSRef().ForEach(wsRef => {
                    Console.WriteLine($"WSRef: {wsRef.Id}: {wsRef.Url}, {wsRef.Description}, {wsRef.Minus}, {wsRef.Plus}"); 
                });

                repo.getAllComment().ForEach(comment => { 
                    Console.WriteLine($"Comment {comment.Id}: {comment.Commtext}, {comment.Stamp}, {comment.WSrefId}");  
                });
            }

            Console.WriteLine("Finish");
            Console.ReadLine();
        }
    }
}