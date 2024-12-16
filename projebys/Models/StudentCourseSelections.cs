namespace projebys.Models
{
    public class StudentCourseSelections
    {
        public int SelectionID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public DateTime SelectionDate { get; set; }
        public bool IsApproved { get; set; }

        public virtual Students Student { get; set; }
        public virtual Courses Course { get; set; }
    }
}
