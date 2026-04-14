using DALMSQLXG;
using GREPO;
using Microsoft.EntityFrameworkCore;

namespace DALMSQLXG
{

    public interface IRepository : GREPO.IRepository<WSRef, Comment> { }
    public class Repository : IRepository
    {
        private readonly Context context;

        private Repository()
        {
            this.context = new Context();
        }

        public static IRepository Create()
        {
            return new Repository();
        }

        public List<WSRef> getAllWSRef()
        {
            return context.wSRefs.ToList();
        }
   
        public List<Comment> getAllComment()
        {
            return context.comments?.ToList() ?? new List<Comment>();
        }

        public Comment? GetCommentById(int Id)
        {
            return context.comments?.Find(Id);
        }

        public bool addWSRef(WSRef wsRef)
        {
            context.wSRefs.Add(wsRef);
            return context.SaveChanges() > 0;
        }

        public bool addComment(Comment comment)
        {
            if (context.comments == null)
                return false;

            context.comments.Add(comment);
            return context.SaveChanges() > 0;
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
