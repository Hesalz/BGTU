using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace REPO
{
    public class REPO
    {
        public interface IRepository : IDisposable   
        {
            List<WSRef> getAllWSRef();                
            List<Comment> getAllComment();         
            bool addWSRef(WSRef wsRef);
            public bool addComment(Comment comment);
        }

        public class WSRef
        {
            [Key]
            public int? Id { get; set; }
            public string? Url { get; set; }
            public string? Description { get; set; }
            public int? Plus { get; set; }
            public int? Minus { get; set; }
            public List<Comment>? comments { get; set; }
        }

        public class Comment
        {
            [Key]
            public int? Id { get; set; }
            [ForeignKey("WSRef")]
            public int? WSrefID { get; set; }
            public DateTime? Stamp { get; set; }
            public string? Commtext { get; set; }
        }
    }
}
