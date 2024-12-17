using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace projebys.Pages.Personnel
{
    public class UpdateCourseModel : PageModel
    {
        public int CourseId { get; set; }

        public void OnGet(int courseId)
        {
            // courseId, URL'den al�nan parametreyi temsil eder
            CourseId = courseId;
        }
    }
}
