using System.Text.Json;
using System.IO;

namespace DALJSONXG
{
    public class JSONContext : IDisposable
    {
        private readonly string fileName;
        private readonly object fileLock = new object();

        public List<WSRef> WSRefs { get; private set; } = new List<WSRef>();

        public List<Comment> Comments
        {
            get
            {
                var rc = new List<Comment>();
                this.WSRefs.ForEach(wsref =>
                {
                    wsref.Comments?.ForEach(comment => rc.Add(comment));
                });
                return rc;
            }
        }

        private JSONContext(string fileName)
        {
            this.fileName = fileName;
            Load();
        }

        public static JSONContext Create(string fileName)
        {
            return new JSONContext(fileName);
        }

        private void Load()
        {
            lock (fileLock)
            {
                if (!File.Exists(fileName))
                {
                    WSRefs.Clear();
                    return;
                }

                try
                {
                    string json = File.ReadAllText(fileName);
                    var loadedData = JsonSerializer.Deserialize<List<WSRef>>(json);
                    if (loadedData != null)
                    {
                        WSRefs = loadedData;
                    }
                    else
                    {
                        WSRefs.Clear();
                    }
                }
                catch (Exception)
                {
                    WSRefs.Clear();
                }
            }
        }

        public int SaveChanges()
        {
            lock (fileLock)
            {
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(WSRefs, options);
                    File.WriteAllText(fileName, json);
                    return 1;
                }
                catch (Exception ex)
                {
                    throw new Exception("Ошибка сохранения данных", ex);
                }
            }
        }

        private int MaxWSRefsId()
        {
            return WSRefs.Count > 0 ? WSRefs.Max(wsref => wsref.Id) : 0;
        }

        public bool addWSRef(WSRef wsref)
        {
            wsref.Id = MaxWSRefsId() + 1;
            this.WSRefs.Add(wsref);
            return SaveChanges() > 0;
        }

        private int MaxCommentsId()
        {
            return WSRefs.SelectMany(wsref => wsref.Comments).Max(comment => comment.Id) ?? 0;
        }

        public bool addComment(Comment comment)
        {
            var wsref = WSRefs.FirstOrDefault(w => w.Id == comment.WSrefId);
            if (wsref != null)
            {
                comment.Id = MaxCommentsId() + 1;
                wsref.Comments ??= new List<Comment>();
                wsref.Comments.Add(comment);
                return SaveChanges() > 0;
            }
            return false;
        }

        public void Dispose()
        {
        }
    }
}