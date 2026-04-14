
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL_LES;

public class Celebrity
{
    public Celebrity()
    {
        this.FullName = "";
        this.Nationality = "XX";
    }
    [Key]
    public int Id { get; set; } 
    public string FullName { get; set; } 
    public string Nationality { get; set; } 
    public string? ReqPhotoPath { get; set; } 

    [InverseProperty("Celebrity")]
    public List<LifeEvent> Lifeevents { get; set; }
}
