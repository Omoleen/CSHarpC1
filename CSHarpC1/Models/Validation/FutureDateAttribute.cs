using System;
using System.ComponentModel.DataAnnotations;

namespace CSHarpC1.Models.Validation
{
    /// <summary>
    /// Validates that a date is not in the future
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // Allow null values (handle with Required attribute if needed)
            if (value == null)
                return true;

            // Check that the date is not in the future
            return value is DateTime date && date <= DateTime.Today;
        }
    }
} 