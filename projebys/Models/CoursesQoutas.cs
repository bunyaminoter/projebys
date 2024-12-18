using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace projebys.Models
{
    public class CourseQuotas
    {
      
            [Key] // CourseID birincil anahtar
            [ForeignKey("Course")] // Courses tablosuna bağlanıyor
            public int CourseID { get; set; }

            [Required]
            public int Quota { get; set; } // Toplam kontenjan

            [Required]
            public int RemainingQuota { get; set; } // Kalan kontenjan

            // Navigation property (Courses ile ilişki)
            public virtual Courses Course { get; set; }
    }
}
