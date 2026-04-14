using GREPO;
using Microsoft.EntityFrameworkCore;

namespace DALJSONXG
{
    public interface IRepository : GREPO.IRepository<WSRef, Comment> { }

    public class Repository : IRepository
    {
        JSONContext context;
        private Repository(string jsonFilePath)
        {
            this.context = JSONContext.Create(jsonFilePath);
        }

        public static IRepository Create(string jsonFilePath = "WSRef.json")
        {
            return new Repository(jsonFilePath);
        }

        public List<Comment> getAllComment()
        {
            return this.context.Comments == null ? new List<Comment>() : this.context.Comments;
        }

        public List<WSRef> getAllWSRef()
        {
            return this.context.WSRefs == null ? new List<WSRef>() : this.context.WSRefs;
        }

        public bool addWSRef(WSRef wsref)
        {
            bool rc = false;
            if (this.context.addWSRef(wsref)) rc = (this.context.SaveChanges() > 0);
            return rc;
        }

        public bool addComment(Comment comment)
        {
            bool rc = false;
            if (this.context.addComment(comment)) rc = (context.SaveChanges() > 0);
            return rc;
        }

        public Comment? GetCommentById(int id)
        {
            return context.Comments.SingleOrDefault(c => c.Id == id);
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}