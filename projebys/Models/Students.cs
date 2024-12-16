namespace projebys.Models
{
    public class Students
    {
        public int StudentID { get; set; } // Öğrenci kimliği
        public string FirstName { get; set; } // Öğrencinin adı
        public string LastName { get; set; } // Öğrencinin soyadı
        public string Email { get; set; } // E-posta adresi
        public int AdvisorID { get; set; } // Danışman kimliği
        public int UserID { get; set; }
        public string Department { get; set; }
        public int ClassID { get; set; }

        public virtual ICollection<Transcripts> Transcripts { get; set; }

        public virtual ICollection<Courses> Courses { get; set; }
        public virtual ICollection<StudentCourseSelections> CourseSelections { get; set; }
        public virtual Users User { get; set; }
        public virtual Classes Class { get; set; }

    }
}
