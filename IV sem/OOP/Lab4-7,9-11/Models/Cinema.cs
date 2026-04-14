using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab4_5.Models
{
    public class Cinema
    {
        [Key]
        public int CinemaId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Range(1, 100)]
        public int HallCount { get; set; } = 1;

        [Range(50, 10000)]
        public int Capacity { get; set; }

        [Range(0.0, 5.0)]
        public double Rating { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public virtual ICollection<CinemaImage> Images { get; set; } = new List<CinemaImage>();

        public override string ToString()
        {
            return $"{CinemaId}. {Name} ({Rating:F1}/5)\nАдрес: {Address}\nКатегория: {Category?.Name}\nЗалы: {HallCount}, Вместимость: {Capacity} мест";
        }

        public Cinema()
        {
            Images = new List<CinemaImage>();
        }
    }


    public class CinemaImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public byte[] ImageData { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(50)]
        public string ContentType { get; set; }

        public int CinemaId { get; set; }

        [ForeignKey("CinemaId")]
        public virtual Cinema Cinema { get; set; }
    }
}