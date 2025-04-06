using System;
using System.ComponentModel.DataAnnotations;

namespace CSHarpC1.Models
{
    /// <summary>
    /// Represents a course in the school system
    /// </summary>
    public class Course
    {
        /// <summary>
        /// The unique identifier for the course
        /// </summary>
        public int CourseId { get; set; }
        
        /// <summary>
        /// The course code
        /// </summary>
        [Required(ErrorMessage = "Course code is required")]
        public string? CourseCode { get; set; }
        
        /// <summary>
        /// The course name
        /// </summary>
        [Required(ErrorMessage = "Course name is required")]
        public string? CourseName { get; set; }
        
        /// <summary>
        /// The course description
        /// </summary>
        public string? CourseDescription { get; set; }
        
        /// <summary>
        /// The term when the course is offered
        /// </summary>
        public string? Term { get; set; }
    }
} 