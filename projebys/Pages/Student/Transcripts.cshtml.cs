using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using projebys.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace projebys.Pages.Student
{
    public class TranscriptModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TranscriptModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public List<CourseGrade> Courses { get; set; }

        public IActionResult OnGet(int id)
        {
            // ��renciyi ve transkriptini al�yoruz
            var student = _context.Students
                .Include(s => s.Transcripts)  // ��rencinin transkriptlerini dahil et
                .ThenInclude(t => t.Course)   // Ders bilgilerini dahil et
                .FirstOrDefault(s => s.StudentID == id);

            if (student == null)
            {
                return RedirectToPage("/Error");
            }

            // ��renci bilgilerini al�yoruz
            StudentID = student.StudentID;
            StudentName = student.FirstName + " " + student.LastName;

            // Transcripts i�erisinden ders bilgilerini ve notlar� al�yoruz
            Courses = student.Transcripts
                .Select(t => new CourseGrade
                {
                    CourseName = t.Course.CourseName,  // Ders ad� (Course'dan al�n�yor)
                    Credit = t.Course.Credit,          // Kredi (Course'dan al�n�yor)
                    Grade = t.Grade                    // Dersin notu (Transcripts'tan al�n�yor)
                }).ToList();

            return Page();
        }

        public class CourseGrade
        {
            public string CourseName { get; set; }
            public int Credit { get; set; }
            public string Grade { get; set; }
        }
    }
}
