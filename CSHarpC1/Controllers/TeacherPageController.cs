using Microsoft.AspNetCore.Mvc;
using CSHarpC1.Models;
using System;
using System.Linq;

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
        /// <summary>
        /// Displays a list of teachers, optionally filtered by a search string.
        /// </summary>
        /// <param name="SearchKey">The string to search for in teacher first names or last names.</param>
        [HttpGet]
        public IActionResult List(string SearchKey = null) // optional search parameter
        {
            List<Teacher> teachers;
            try
            {
                // Fetch all teachers first
                var apiResult = _apiController.GetAllTeachers();

                if (apiResult is OkObjectResult okResult && okResult.Value is List<Teacher> allTeachers)
                {
                    // Filter if a search key is provided
                    if (!String.IsNullOrEmpty(SearchKey))
                    {
                        string lowerSearchKey = SearchKey.ToLower();
                        teachers = allTeachers.Where(t => 
                                        (t.TeacherFName != null && t.TeacherFName.ToLower().Contains(lowerSearchKey)) ||
                                        (t.TeacherLName != null && t.TeacherLName.ToLower().Contains(lowerSearchKey)) ||
                                        (t.EmployeeNumber != null && t.EmployeeNumber.ToLower().Contains(lowerSearchKey))
                                       ).ToList();
                         ViewData["SearchKey"] = SearchKey; // Pass search key back to view
                         ViewData["TeacherCount"] = teachers.Count; // Show count of results
                    }
                    else
                    {
                        teachers = allTeachers; // No search key, show all
                         ViewData["TeacherCount"] = teachers.Count;
                    }

                    return View("~/Views/Teacher/List.cshtml", teachers);
                }
                else
                {
                    // Handle API error more gracefully for the view
                     teachers = new List<Teacher>(); // Return empty list on error
                     ViewData["ErrorMessage"] = "Error retrieving teacher list from API.";
                     ViewData["TeacherCount"] = 0;
                }
            }
            catch (Exception ex)
            {
                 // Log exception
                 teachers = new List<Teacher>();
                 ViewData["ErrorMessage"] = "An unexpected error occurred.";
                 ViewData["TeacherCount"] = 0;
            }

            return View("~/Views/Teacher/List.cshtml", teachers);
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

        // GET: /TeacherPage/Edit/5
        /// <summary>
        /// Displays the form to edit an existing teacher.
        /// Fetches teacher data using the API.
        /// </summary>
        /// <param name="id">The ID of the teacher to edit.</param>
        /// <returns>The Edit view pre-populated with the teacher's data, or NotFound if the teacher doesn't exist.</returns>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Fetch the teacher data from the API
            var apiResult = _apiController.GetTeacher(id);

            if (apiResult is OkObjectResult okResult)
            {
                var teacher = okResult.Value as Teacher;
                if (teacher != null)
                {
                    // Pass the teacher model to the Edit view
                    return View("~/Views/Teacher/Edit.cshtml", teacher);
                }
            }

            // Handle case where teacher is not found by the API
            if (apiResult is NotFoundObjectResult)
            {
                TempData["ErrorMessage"] = $"Teacher with ID {id} not found.";
                return RedirectToAction("List");
            }
            
            // Handle potential API errors
            TempData["ErrorMessage"] = "Error retrieving teacher details for editing.";
            return RedirectToAction("List");
        }

        // POST: /TeacherPage/Edit/5
        /// <summary>
        /// Processes the submitted form data to update a teacher.
        /// Performs model validation and calls the Update API endpoint.
        /// Includes Server-side error handling for empty name, future hire date, and negative salary via model validation.
        /// </summary>
        /// <param name="id">The ID of the teacher being updated (from the route).</param>
        /// <param name="teacher">The Teacher object with updated data from the form.</param>
        /// <returns>Redirects to the List view on success, otherwise returns the Edit view with validation errors.</returns>
        [HttpPost]
        public IActionResult Edit(int id, Teacher teacher)
        {
            // Ensure the ID from the route matches the model's ID
            if (id != teacher.TeacherId)
            {
                 ModelState.AddModelError("", "ID mismatch occurred.");
                 // Return the view with the original data fetched, not the potentially incorrect submitted data
                 var originalApiResult = _apiController.GetTeacher(id);
                 if (originalApiResult is OkObjectResult okResult) {
                     return View("~/Views/Teacher/Edit.cshtml", okResult.Value as Teacher);
                 }
                 // Fallback if original fetch fails too
                 TempData["ErrorMessage"] = "Could not reload teacher data after ID mismatch.";
                 return RedirectToAction("List");
            }
            
            // Initiative: Server Error Handling (Empty Name, Future Date, Negative Salary) via ModelState
            if (ModelState.IsValid)
            {
                // Call the API to update the teacher
                var apiResult = _apiController.UpdateTeacher(id, teacher);

                if (apiResult is NoContentResult)
                {
                    TempData["SuccessMessage"] = "Teacher updated successfully.";
                    return RedirectToAction("List"); // Redirect on success
                }
                else
                {
                    // Handle API errors (e.g., NotFound, BadRequest for duplicate employee number)
                    if (apiResult is NotFoundObjectResult notFoundResult)
                    {
                        var errorObj = notFoundResult.Value as dynamic;
                        ModelState.AddModelError("", errorObj?.message ?? $"Teacher with ID {id} not found by API.");
                    }
                    else if (apiResult is BadRequestObjectResult badRequestResult)
                    {
                         var errorObj = badRequestResult.Value as dynamic;
                         var errorMessage = errorObj?.message ?? "Failed to update teacher. Please check your input.";
                         ModelState.AddModelError("", errorMessage);
                          if (errorMessage.Contains("Employee number is already taken"))
                          {
                               ModelState.AddModelError("EmployeeNumber", "This employee number is already in use by another teacher.");
                          }
                    }
                     else if (apiResult is ObjectResult errorResult && errorResult.StatusCode == 500) {
                         var errorObj = errorResult.Value as dynamic;
                         ModelState.AddModelError("", errorObj?.message ?? "An unexpected server error occurred.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "An unknown error occurred while updating the teacher via API.");
                    }
                }
            }
            else
            {
                // Model state is invalid, add a general error message if needed
                ModelState.AddModelError("", "Please correct the validation errors.");
            }

            // If model state is invalid or API update failed, return the view with the submitted data and errors
            return View("~/Views/Teacher/Edit.cshtml", teacher);
        }

        // GET: /TeacherPage/EditAjax/5
        /// <summary>
        /// Displays the AJAX form to edit an existing teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to edit.</param>
        [HttpGet]
        public IActionResult EditAjax(int id)
        {
            // Fetch teacher data using the API to pre-populate the form
            var apiResult = _apiController.GetTeacher(id);

            if (apiResult is OkObjectResult okResult)
            {
                var teacher = okResult.Value as Teacher;
                if (teacher != null)
                {
                    return View("~/Views/Teacher/EditAjax.cshtml", teacher);
                }
            }
            
            // Handle errors similar to the non-AJAX Edit GET action
            TempData["ErrorMessage"] = $"Could not load teacher with ID {id} for editing.";
             return RedirectToAction("List"); // Or return a specific error view
        }
    }
}
