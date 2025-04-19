using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using CSHarpC1.Models;
using System;
using System.Collections.Generic;

namespace CSHarpC1.Controllers
{
    /// <summary>
    /// Web API Controller to access teacher information.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _dbContext;

        public TeacherAPIController(SchoolDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves all teachers from the database
        /// </summary>
        /// <returns>A list of all teachers in the database</returns>
        [HttpGet]
        public IActionResult GetAllTeachers()
        {
            try
            {
                using (var conn = _dbContext.AccessDatabase())
                {
                    conn.Open();
                    
                    // Added teacherworkphone to the SELECT query
                    string query = "SELECT teacherid, teacherfname, teacherlname, employeenumber, hiredate, salary, teacherworkphone FROM teachers";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var teachers = new List<Teacher>();

                            while (reader.Read())
                            {
                                var teacher = new Teacher
                                {
                                    TeacherId = Convert.ToInt32(reader["teacherid"]),
                                    TeacherFName = reader["teacherfname"].ToString(),
                                    TeacherLName = reader["teacherlname"].ToString(),
                                    EmployeeNumber = reader["employeenumber"].ToString(),
                                    HireDate = reader["hiredate"] == DBNull.Value ? null : Convert.ToDateTime(reader["hiredate"]),
                                    Salary = reader["salary"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["salary"]),
                                    TeacherWorkPhone = reader["teacherworkphone"] == DBNull.Value ? null : reader["teacherworkphone"].ToString()
                                };
                                teachers.Add(teacher);
                            }

                            return Ok(teachers);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { message = "Database error", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific teacher by their ID
        /// </summary>
        /// <param name="id">The unique identifier of the teacher</param>
        /// <returns>The teacher's information if found, otherwise a 404 error</returns>
        [HttpGet("{id}")]
        public IActionResult GetTeacher(int id)
        {
            try
            {
                using (var conn = _dbContext.AccessDatabase())
                {
                    conn.Open();
                    
                    // Add teacherworkphone to the SELECT query
                    string query = "SELECT teacherid, teacherfname, teacherlname, employeenumber, hiredate, salary, teacherworkphone FROM teachers WHERE teacherid = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var teacher = new Teacher
                                {
                                    TeacherId = Convert.ToInt32(reader["teacherid"]),
                                    TeacherFName = reader["teacherfname"].ToString(),
                                    TeacherLName = reader["teacherlname"].ToString(),
                                    EmployeeNumber = reader["employeenumber"].ToString(),
                                    HireDate = reader["hiredate"] == DBNull.Value ? null : Convert.ToDateTime(reader["hiredate"]),
                                    Salary = reader["salary"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["salary"]),
                                    TeacherWorkPhone = reader["teacherworkphone"] == DBNull.Value ? null : reader["teacherworkphone"].ToString()
                                };

                                return Ok(teacher);
                            }
                            else
                            {
                                // Return 404 if teacher not found
                                return NotFound(new { message = $"Teacher with ID {id} not found." });
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { message = "Database connection error", details = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new teacher to the database
        /// </summary>
        /// <param name="teacher">The teacher object containing information to be added</param>
        /// <returns>The newly created teacher with ID if successful, otherwise an error</returns>
        [HttpPost]
        public IActionResult AddTeacher([FromBody] Teacher teacher)
        {
            try
            {
                using (var conn = _dbContext.AccessDatabase())
                {
                    conn.Open();
                    
                    // Check if employee number already exists
                    string checkQuery = "SELECT COUNT(*) FROM teachers WHERE employeenumber = @employeenumber";
                    if (teacher.TeacherId > 0)
                    {
                        checkQuery += " AND teacherid != @teacherid";
                    }
                    
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@employeenumber", teacher.EmployeeNumber);
                        if (teacher.TeacherId > 0)
                        {
                            checkCmd.Parameters.AddWithValue("@teacherid", teacher.TeacherId);
                        }
                        
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            return BadRequest(new { message = "Employee number already exists" });
                        }
                    }
                    
                    // Add teacherworkphone to the insert query
                    string query = @"INSERT INTO teachers 
                                    (teacherfname, teacherlname, employeenumber, hiredate, salary, teacherworkphone) 
                                    VALUES 
                                    (@teacherfname, @teacherlname, @employeenumber, @hiredate, @salary, @teacherworkphone);
                                    SELECT LAST_INSERT_ID();";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@teacherfname", teacher.TeacherFName);
                        cmd.Parameters.AddWithValue("@teacherlname", teacher.TeacherLName);
                        cmd.Parameters.AddWithValue("@employeenumber", teacher.EmployeeNumber);
                        cmd.Parameters.AddWithValue("@hiredate", teacher.HireDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@salary", teacher.Salary);
                        cmd.Parameters.AddWithValue("@teacherworkphone", teacher.TeacherWorkPhone ?? (object)DBNull.Value);
                        
                        // Execute query and get the newly created ID
                        int newId = Convert.ToInt32(cmd.ExecuteScalar());
                        teacher.TeacherId = newId;
                        
                        return CreatedAtAction(nameof(GetTeacher), new { id = newId }, teacher);
                    }
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { message = "Database error", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing teacher in the database.
        /// Corresponds to PUT /api/TeacherAPI/{id}
        /// </summary>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="teacher">A Teacher object containing the updated information.</param>
        /// <returns>
        /// 204 No Content if the update is successful.
        /// 400 Bad Request if the provided ID does not match the ID in the request body.
        /// 400 Bad Request if the employee number is already taken by another teacher.
        /// 404 Not Found if the teacher with the specified ID does not exist.
        /// 500 Internal Server Error if a database error occurs.
        /// </returns>
        /// <example>
        /// PUT /api/TeacherAPI/5
        /// Request Body: { "teacherId": 5, "teacherFName": "Jessica", "teacherLName": "Morris Updated", ... }
        /// </example>
        [HttpPut("{id}")]
        public IActionResult UpdateTeacher(int id, [FromBody] Teacher teacher)
        {
            // Check if the ID in the URL matches the ID in the request body
            if (id != teacher.TeacherId)
            {
                return BadRequest(new { message = "ID mismatch between URL and request body." });
            }

            try
            {
                using (var conn = _dbContext.AccessDatabase())
                {
                    conn.Open();

                    // 1. Check if the teacher exists
                    string checkExistQuery = "SELECT COUNT(*) FROM teachers WHERE teacherid = @id";
                    using (var checkExistCmd = new MySqlCommand(checkExistQuery, conn))
                    {
                        checkExistCmd.Parameters.AddWithValue("@id", id);
                        int existCount = Convert.ToInt32(checkExistCmd.ExecuteScalar());
                        if (existCount == 0)
                        {
                            // Initiative: Server Error Handling on Update (Non-Existent)
                            return NotFound(new { message = $"Teacher with ID {id} not found." });
                        }
                    }

                    // 2. Check if the employee number is already taken by another teacher
                    string checkDuplicateQuery = "SELECT COUNT(*) FROM teachers WHERE employeenumber = @employeenumber AND teacherid != @id";
                    using (var checkDuplicateCmd = new MySqlCommand(checkDuplicateQuery, conn))
                    {
                        checkDuplicateCmd.Parameters.AddWithValue("@employeenumber", teacher.EmployeeNumber);
                        checkDuplicateCmd.Parameters.AddWithValue("@id", id);
                        int duplicateCount = Convert.ToInt32(checkDuplicateCmd.ExecuteScalar());
                        if (duplicateCount > 0)
                        {
                            return BadRequest(new { message = "Employee number is already taken by another teacher." });
                        }
                    }

                    // 3. Perform the update
                    string query = @"UPDATE teachers SET 
                                    teacherfname = @teacherfname, 
                                    teacherlname = @teacherlname, 
                                    employeenumber = @employeenumber, 
                                    hiredate = @hiredate, 
                                    salary = @salary, 
                                    teacherworkphone = @teacherworkphone 
                                WHERE teacherid = @id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@teacherfname", teacher.TeacherFName);
                        cmd.Parameters.AddWithValue("@teacherlname", teacher.TeacherLName);
                        cmd.Parameters.AddWithValue("@employeenumber", teacher.EmployeeNumber);
                        cmd.Parameters.AddWithValue("@hiredate", teacher.HireDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@salary", teacher.Salary);
                        cmd.Parameters.AddWithValue("@teacherworkphone", teacher.TeacherWorkPhone ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", id); // Use the ID from the URL parameter

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return NoContent(); // Standard successful PUT response
                        }
                        else
                        {
                            // Should ideally be caught by the existence check, but good to have a fallback
                            return NotFound(new { message = $"Teacher with ID {id} could not be updated (possibly already deleted)." });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Log the exception details (omitted for brevity)
                return StatusCode(500, new { message = "Database error during update.", details = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception details (omitted for brevity)
                return StatusCode(500, new { message = "An error occurred during update.", details = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a teacher from the database by their ID
        /// </summary>
        /// <param name="id">The unique identifier of the teacher to delete</param>
        /// <returns>204 No Content if successful, 404 if teacher not found, or an error</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteTeacher(int id)
        {
            try
            {
                using (var conn = _dbContext.AccessDatabase())
                {
                    conn.Open();
                    
                    // First check if the teacher exists
                    string checkQuery = "SELECT COUNT(*) FROM teachers WHERE teacherid = @id";
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        
                        if (count == 0)
                        {
                            return NotFound(new { message = $"Teacher with ID {id} not found." });
                        }
                    }
                    
                    // Delete the teacher
                    string deleteQuery = "DELETE FROM teachers WHERE teacherid = @id";
                    using (var deleteCmd = new MySqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@id", id);
                        deleteCmd.ExecuteNonQuery();
                        
                        return NoContent(); // 204 No Content is the standard response for successful DELETE
                    }
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, new { message = "Database error", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }
    }
}
