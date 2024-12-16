namespace projebys.Models
{
    public class Classes
    {
        public int ClassID { get; set; } // Sınıf kimliği
        public string ClassName { get; set; } // Sınıf adı
        public string Department { get; set; } // Bölüm adı

        // Bir sınıfın birden fazla öğrencisi olabilir
        public virtual ICollection<Students> Students { get; set; }

        public virtual ICollection<ClassCourseMappings> ClassCourseMappings { get; set; }
    }
}
