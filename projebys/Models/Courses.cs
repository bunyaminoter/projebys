using System.ComponentModel.DataAnnotations.Schema;

namespace projebys.Models
{
    public class Courses
    {

        public int CourseID { get; set; } // Ders kimliği
        public string CourseCode { get; set; }  // Ders kodu
        public string CourseName { get; set; } // Ders adı
        public bool IsMandatory { get; set; } // Zorunlu ders mi?
        public int Credit { get; set; } // Kredi sayısı
        public string Department { get; set; }  // Bölüm

        public virtual ICollection<Students> Students { get; set; }

        public virtual ICollection<Transcripts> Transcripts { get; set; }

        public virtual ICollection<StudentCourseSelections> StudentSelections { get; set; }
    }
}
