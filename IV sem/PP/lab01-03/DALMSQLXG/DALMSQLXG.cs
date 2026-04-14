using GREPO;
using Microsoft.EntityFrameworkCore;
using static REPO.REPO;

namespace DALMSQLXG
{
    public class DALLMSQLXG
    {
        public class Repository : GREPO.IRepository <WSRef, Comment>
        {
            Context context;
            private Repository()
            {
                this.context = new Context();
            }
            public static IRepository<WSRef, Comment> Create() { return new Repository(); }
            public List<Comment> getAllComment()
            {
                List<Comment> rc = new List<Comment>();
                if (this.context.comments != null)
                {
                    rc.AddRange(this.context.comments
                        .OrderBy(c => c.Stamp)
                        .ThenBy(c => c.Id));
                }
                return rc;
            }

            public List<WSRef> getAllWSRef()
            {
                List<WSRef> rc = new List<WSRef>();
                if (this.context.wSRefs != null)
                {
                    rc.AddRange(this.context.wSRefs.OrderBy(w => w.Id));
                }
                return rc;
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

            public bool addComment(Comment comment)
            {
                bool rc = false;
                if (context.comments != null)
                {
                    context.Add(comment);
                    rc = (context.SaveChanges() > 0);
                }
                return rc;
            }

            public Comment? GetCommentById(int id)
            {
                if (this.context.comments == null)
                {
                    return null;
                }
                var comment = this.context.comments.FirstOrDefault(c => c.Id == id);
                return comment;
            }

            public void Dispose()
            {
                this.context.Dispose();
            }
        }
    }
}
