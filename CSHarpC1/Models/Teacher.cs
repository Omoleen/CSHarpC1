using System;
using System.ComponentModel.DataAnnotations;
using CSHarpC1.Models.Validation;

namespace CSHarpC1.Models;

/// <summary>
/// Represents a teacher in the school system.
/// Contains properties and validation rules for teacher data.
/// </summary>
public class Teacher
{
    /// <summary>
    /// The unique identifier for the teacher (Primary Key).
    /// </summary>
    public int TeacherId { get; set; }
    
    /// <summary>
    /// The teacher's first name. Required field.
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(255, ErrorMessage = "First name cannot exceed 255 characters")]
    public string? TeacherFName { get; set; }
    
    /// <summary>
    /// The teacher's last name. Required field.
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(255, ErrorMessage = "Last name cannot exceed 255 characters")]
    public string? TeacherLName { get; set; }
    
    /// <summary>
    /// The employee number assigned to the teacher. Required field.
    /// Must follow the format 'T' followed by one or more digits (e.g., T123).
    /// </summary>
    [Required(ErrorMessage = "Employee number is required")]
    [RegularExpression(@"^T\d+$", ErrorMessage = "Employee number must be 'T' followed by digits")]
    public string? EmployeeNumber { get; set; }
    
    /// <summary>
    /// The date when the teacher was hired. Required field.
    /// Cannot be a date in the future.
    /// </summary>
    [Required(ErrorMessage = "Hire date is required")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    [FutureDate(ErrorMessage = "Hire date cannot be in the future")]
    public DateTime? HireDate { get; set; }
    
    /// <summary>
    /// The teacher's annual salary. Required field.
    /// Must be a non-negative value.
    /// </summary>
    [Required(ErrorMessage = "Salary is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Salary must be greater than or equal to 0")]
    public decimal Salary { get; set; }
    
    /// <summary>
    /// The teacher's work phone number (optional).
    /// Must be a valid phone number format if provided.
    /// </summary>
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? TeacherWorkPhone { get; set; }
}