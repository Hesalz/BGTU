using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL_LES;

public class LifeEvent
{
    public LifeEvent()
    {
        this.Description = "";
    }
    [Key]
    public int Id { get; set; }
    [ForeignKey("Celebrity")]
    public int CelebrityId { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public string? ReqPhotoPath { get; set; }
    public Celebrity Celebrity { get; set; }
}
