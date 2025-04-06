using System;
using System.ComponentModel.DataAnnotations;
using CSHarpC1.Models.Validation;

namespace CSHarpC1.Models
{
    /// <summary>
    /// Represents a student in the school system
    /// </summary>
    public class Student
    {
        /// <summary>
        /// The unique identifier for the student
        /// </summary>
        public int StudentId { get; set; }
        
        /// <summary>
        /// The student's first name
        /// </summary>
        [Required(ErrorMessage = "First name is required")]
        public string? StudentFName { get; set; }
        
        /// <summary>
        /// The student's last name
        /// </summary>
        [Required(ErrorMessage = "Last name is required")]
        public string? StudentLName { get; set; }
        
        /// <summary>
        /// The student's number
        /// </summary>
        [Required(ErrorMessage = "Student number is required")]
        public string? StudentNumber { get; set; }
        
        /// <summary>
        /// The student's enrollment date
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [FutureDate(ErrorMessage = "Enrollment date cannot be in the future")]
        public DateTime? EnrollmentDate { get; set; }
    }
} 