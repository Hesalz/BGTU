
namespace DAL
{
    public interface IRepository: IDisposable
    {
        List<WSRef>    getAllWSRef();
        List<Comment>  getAllComment();
        bool           addWSRef(WSRef wsRef);
    }
    public class Repository : IRepository
    {
        Context context;    
        private Repository() 
        {
          this.context = new Context();     
        }   
        public static IRepository Create() {return new Repository();}
        public List<Comment> getAllComment()
        {
            List<Comment> rc = new List<Comment>(); 
            if (this.context.comments != null)
            { 
                rc.AddRange(this.context.comments); 
            }
            return  rc;
        }
        public List<WSRef> getAllWSRef()
        {
            List<WSRef> rc = new List<WSRef>(); 
            if(this.context.wSRefs != null)
            { 
             rc.AddRange(this.context.wSRefs);    
            }            return rc;
        }
        public bool addWSRef(WSRef wsref)
        {
            bool rc = false;
            if (this.context.wSRefs != null)
            {
                context.Database.BeginTransaction();
                context.wSRefs.Add(wsref);
                rc = (context.SaveChanges() > 0);
                WSRef x = context.wSRefs.OrderByDescending(e => e.Id).First();
                context.Database.CommitTransaction();
            }
            return rc;
        }

        public void Dispose()
        {
            this.context.Dispose(); 
        }
    }


}
