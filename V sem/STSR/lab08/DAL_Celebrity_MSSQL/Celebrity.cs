using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_Celebrity_MSSQL
{
    public class Celebrity  // Знаменитость 
    {
        public Celebrity() { this.FullName = string.Empty; this.Nationality = string.Empty; }
        public int Id { get; set; }                 // Id знаменитости 
        public string FullName { get; set; }        // полное имя знаменитости
        public string Nationality { get; set; }     // гражданство знаменитости (2 символа ISO)
        public string? ReqPhotoPath { get; set; }   // reguest path  фотографии 

        //public  virtual bool Update(Celebrity celebrity); // --вспомогательный метод
    }
}
