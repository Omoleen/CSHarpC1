using Microsoft.AspNetCore.Mvc;
using CSHarpC1.Models;
namespace CSHarpC1.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly TeacherAPIController _apiController;

        public TeacherPageController(TeacherAPIController apiController)
        {
            _apiController = apiController;
        }

        // GET: /TeacherPage/List
        [HttpGet]
        public IActionResult List()
        {
            // Call the API controller directly
            var apiResult = _apiController.GetAllTeachers();
            
            // Check if the result is successful
            if (apiResult is OkObjectResult okResult)
            {
                // Extract the teachers from the API result
                var teachers = okResult.Value as List<Teacher>;
                return View("~/Views/Teacher/List.cshtml", teachers);
            }
            
            // Handle error case
            return StatusCode(500, "Error retrieving teacher list");
        }

        // GET: /TeacherPage/Show/5
        [HttpGet]
        public IActionResult Show(int id)
        {
            // Call the API controller directly
            var apiResult = _apiController.GetTeacher(id);
            
            // Check if the result is successful
            if (apiResult is OkObjectResult okResult)
            {
                // Extract the teacher from the API result
                var teacher = okResult.Value as Teacher;
                return View("~/Views/Teacher/Show.cshtml", teacher);
            }
            
            // Handle not found case
            if (apiResult is NotFoundObjectResult)
            {
                return NotFound($"Teacher with ID {id} not found.");
            }
            
            // Handle other errors
            return StatusCode(500, "Error retrieving teacher details");
        }

        // GET: /TeacherPage or /TeacherPage/Index
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }
    }
}
