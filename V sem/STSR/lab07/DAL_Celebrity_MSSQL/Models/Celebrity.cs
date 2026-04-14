
namespace DAL_Celebrity_MSSQL.Models
{
    public class Celebrity  //  Знаменитость  
    {
        public Celebrity() { this.FullName = string.Empty; this.Nationality = string.Empty; }
        public int Id { get; set; }                        // Id Знаменитости        
        public string FullName { get; set; }         // полное имя   Знаменитости
        public string Nationality { get; set; }         // гражданство  Знаменитости ( 2 символа ISO )
        public string? ReqPhotoPath { get; set; }         // reguest path  Фотографии
        
        public virtual bool Update(Celebrity celebrity)   // --вспомогательный метод  
        {
            if (!string.IsNullOrEmpty(celebrity.FullName)) this.FullName = celebrity.FullName;
            if (!string.IsNullOrEmpty(celebrity.Nationality)) this.Nationality = celebrity.Nationality;
            if (!string.IsNullOrEmpty(celebrity.ReqPhotoPath)) this.ReqPhotoPath = celebrity.ReqPhotoPath;
            return true;     //  изменения были ?
        }
    }
}
