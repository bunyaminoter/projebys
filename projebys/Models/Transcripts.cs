using projebys.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Transcripts
{
    [Key]
    public int TranscriptID { get; set; }
    public int StudentID { get; set; }
    public int CourseID { get; set; }
    [Required]
    public string Grade { get; set; }

    [ForeignKey("StudentID")]
    public Students Student { get; set; }

    [ForeignKey("CourseID")]
    public Courses Course { get; set; }

}
