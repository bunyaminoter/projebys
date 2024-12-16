namespace projebys.Models
{
    public class ClassCourseMappings
    {
        public int MappingID { get; set; } // Harita kimliği (Primary Key)
        public int ClassID { get; set; } // Sınıf kimliği (Foreign Key)
        public int CourseID { get; set; } // Ders kimliği (Foreign Key)
        public bool IsMandatory { get; set; } // Zorunlu ders olup olmadığını belirtir

        // İlişkiler
        public virtual Classes Class { get; set; } // Class ile ilişki
        public virtual Courses Course { get; set; } // Courses ile ilişki
    }
}
