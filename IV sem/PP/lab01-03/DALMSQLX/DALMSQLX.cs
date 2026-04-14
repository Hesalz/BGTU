using Microsoft.EntityFrameworkCore;
using static REPO.REPO;

namespace DALMSQLX
{
    public class DALLMSQLX
    {
        public class Repository : IRepository
        {
            Context context;
            private Repository()
            {
                this.context = new Context();
            }
            public static IRepository Create() { return new Repository(); }
            public List<Comment> getAllComment()
            {
                List<Comment> rc = new List<Comment>();
                if (this.context.comments != null)
                {
                    rc.AddRange(this.context.comments);
                }
                return rc;
            }
            public List<WSRef> getAllWSRef()
            {
                List<WSRef> rc = new List<WSRef>();
                if (this.context.wSRefs != null)
                {
                    rc.AddRange(this.context.wSRefs);
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

            public void Dispose()
            {
                this.context.Dispose();
            }
        }

        public class Context : DbContext
        {
            public Context() : base()
            {
                Database.EnsureCreated();
            }
            public DbSet<WSRef> wSRefs { get; set; }
            public DbSet<Comment>? comments { get; set; }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(@"Data Source=PC; Initial Catalog=SSSS; TrustServerCertificate=True; Integrated Security=True;");

            }
        }
    }
}
