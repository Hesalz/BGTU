using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL
{
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
