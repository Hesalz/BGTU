using static REPO.REPO;
using System.Text.Json;

namespace DALJSONX
{
    public class DALJSONX
    {
        public class Repository : IRepository
        {
            JSONContext context;
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
            public void Dispose()
            {
                this.context.Dispose();
            }
        }

        public class JSONContext : IDisposable
        {
            FileStream fs;
            public List<WSRef> WSRefs { get; private set; }
            public List<Comment> Comments
            {
                get
                {
                    List<Comment> rc = new List<Comment>();
                    this.WSRefs.ForEach(wsref => { wsref.comments?.ForEach(comment => rc.Add(comment)); });
                    return rc;
                }
            }

            private JSONContext(string FileName)
            {
                if (!File.Exists(FileName))
                {
                    this.fs = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite);
                    JsonSerializer.SerializeAsync<List<WSRef>>(this.fs, this.WSRefs = new List<WSRef>()).Wait();
                }
                else
                {
                    this.fs = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite);
                    this.WSRefs = this.Load();
                }
            }
            public static JSONContext Create(string FileName) { return new JSONContext(FileName); }
            private List<WSRef> Load()
            {
                this.fs.Seek(0, SeekOrigin.Begin);
                List<WSRef>? wsrefs = JsonSerializer.DeserializeAsync<List<WSRef>?>(fs).Result;
                return (wsrefs == null) ? new List<WSRef>() : wsrefs;
            }
            public int SaveChanges()
            {
                this.fs.Seek(0, SeekOrigin.Begin);
                JsonSerializer.SerializeAsync<List<WSRef>>(this.fs, this.WSRefs == null ? new List<WSRef>() : this.WSRefs);
                return 1;
            }
            private int MaxWSRefsId()
            {
                if (this.WSRefs.Count > 0)
                {
                    int? maxId = this.WSRefs.Max(wsref => wsref.Id);
                    if (maxId.HasValue)
                    {
                        return (int)maxId.Value;
                    }
                }
                return 0;
            }

            public bool addWSRef(WSRef wsref)
            {
                wsref.Id = MaxWSRefsId() + 1;
                this.WSRefs.Add(wsref);
                return true;
            }
            private int MaxCommentsId()
            {
                int rc = 0;
                int? m = 0;

                this.WSRefs.ForEach(wsref => {
                if (wsref.comments != null)
                {
                    m = wsref.comments.Max(comment => comment.Id);
                    rc = (m != null && m > rc) ? (int)m : rc;
                }
                });
                return rc;
            }
            public bool addComment(Comment comment)
            {
                bool rc = false;
                int idx = this.WSRefs.FindIndex(wsref => wsref.Id == comment.WSrefID);
                if (idx >= 0)
                {
                    var wsref = this.WSRefs[idx];
                    if (wsref.comments == null)
                    {
                        wsref.comments = new List<Comment>();
                    }
                    comment.Id = this.MaxCommentsId() + 1;
                    wsref.comments.Add(comment);
                    rc = true;
                }
                return rc;
            }

            public void Dispose() { fs.Dispose(); }
        }
    }
}
