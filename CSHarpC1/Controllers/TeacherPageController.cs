using Microsoft.AspNetCore.Mvc;
using CSHarpC1.Models;
using System;

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

        /// <summary>
        /// Displays the form to create a new teacher
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Teacher/Create.cshtml");
        }

        /// <summary>
        /// Processes the form submission to create a new teacher
        /// </summary>
        /// <param name="teacher">The teacher data from the form</param>
        [HttpPost]
        public IActionResult Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                var result = _apiController.AddTeacher(teacher);
                
                if (result is CreatedAtActionResult)
                {
                    TempData["SuccessMessage"] = "Teacher added successfully.";
                    return RedirectToAction("List");
                }
                
                if (result is BadRequestObjectResult badRequestResult)
                {
                    // Extract the error message from the API response
                    var errorObj = badRequestResult.Value as dynamic;
                    var errorMessage = errorObj?.message ?? "Failed to add teacher. Please try again.";
                    
                    ModelState.AddModelError("", errorMessage);
                    
                    if (errorMessage.Contains("Employee number already exists"))
                    {
                        ModelState.AddModelError("EmployeeNumber", "This employee number is already in use.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add teacher. Please try again.");
                }
            }
            
            return View("~/Views/Teacher/Create.cshtml", teacher);
        }

        /// <summary>
        /// Displays the confirmation page for deleting a teacher
        /// </summary>
        /// <param name="id">The ID of the teacher to delete</param>
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var apiResult = _apiController.GetTeacher(id);
            
            if (apiResult is OkObjectResult okResult)
            {
                var teacher = okResult.Value as Teacher;
                return View("~/Views/Teacher/Delete.cshtml", teacher);
            }
            
            return NotFound($"Teacher with ID {id} not found.");
        }

        /// <summary>
        /// Processes the teacher deletion after confirmation
        /// </summary>
        /// <param name="id">The ID of the teacher to delete</param>
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var apiResult = _apiController.DeleteTeacher(id);
            
            if (apiResult is NoContentResult)
            {
                TempData["SuccessMessage"] = "Teacher successfully deleted.";
                return RedirectToAction("List");
            }
            
            if (apiResult is NotFoundObjectResult notFoundResult)
            {
                // Extract the error message from the API response
                var errorObj = notFoundResult.Value as dynamic;
                var errorMessage = errorObj?.message ?? $"Teacher with ID {id} not found.";
                
                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction("List");
            }
            
            // Handle other errors
            TempData["ErrorMessage"] = "An error occurred while deleting the teacher.";
            return RedirectToAction("List");
        }

        // GET: /TeacherPage/AddAjax
        [HttpGet]
        public IActionResult AddAjax()
        {
            return View("~/Views/Teacher/AddAjax.cshtml");
        }

        // GET: /TeacherPage/DeleteAjax/5
        [HttpGet]
        public IActionResult DeleteAjax(int id)
        {
            var apiResult = _apiController.GetTeacher(id);
            
            if (apiResult is OkObjectResult okResult)
            {
                var teacher = okResult.Value as Teacher;
                return View("~/Views/Teacher/DeleteAjax.cshtml", teacher);
            }
            
            return NotFound($"Teacher with ID {id} not found.");
        }
    }
}
