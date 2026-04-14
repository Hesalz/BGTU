namespace DAL12_JSON
{
    public interface IRepository: IDisposable             //  наследует  IDisposable
    {
        List<WSRef>    getAllWSRef();                      // получить весь перечень Интернет-ресурсов 
        List<Comment>  getAllComment();                    // получить все комментарии  
        bool           addWSRef(WSRef wsRef);              // добавить новый  Интернет-ресурс  
        bool           addComment(Comment comment);        // добавить комментарий   
    }
    


    public class Repository : IRepository
    {
        JSONContext  context;    
        private Repository()
        {
            this.context = JSONContext.Create("WSRef.json");
        }   
        public static IRepository Create()
        {
           return new Repository();
        }
        public List<Comment> getAllComment()
        {
            return  this.context.Comments ==  null? new List<Comment>(): this.context.Comments;
        }
        public List<WSRef> getAllWSRef()
        {            
            return this.context.WSRefs == null? new List<WSRef>() : this.context.WSRefs;
        }
        public bool addWSRef(WSRef wsref)
        {
            bool rc = false;
            if (this.context.addWSRef(wsref))  rc = (this.context.SaveChanges() > 0);
            return rc;
        }
        public bool addComment(Comment comment)
        {
            bool rc = false;
            if (this.context.addComment(comment))  rc = (context.SaveChanges() > 0);
            return rc;
        }
        public void Dispose()
        {
            this.context.Dispose(); 
        }
    }
}

