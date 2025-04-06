using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using CSHarpC1.Models;
using System;
using System.Collections.Generic;

namespace CSHarpC1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly SchoolDbContext _dbContext;

        public StudentAPIController(SchoolDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves all students from the database
        /// </summary>
        /// <returns>A list of all students in the database</returns>
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            try
            {
                using (var conn = _dbContext.AccessDatabase())
                {
                    conn.Open();
                    
                    string query = "SELECT studentid, studentfname, studentlname, studentnumber, enroldate FROM students";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            var students = new List<Student>();

                            while (reader.Read())
                            {
                                var student = new Student
                                {
                                    StudentId = Convert.ToInt32(reader["studentid"]),
                                    StudentFName = reader["studentfname"].ToString(),
                                    StudentLName = reader["studentlname"].ToString(),
                                    StudentNumber = reader["studentnumber"].ToString(),
                                    EnrollmentDate = reader["enroldate"] == DBNull.Value ? null : Convert.ToDateTime(reader["enroldate"])
                                };
                                students.Add(student);
                            }

                            return Ok(students);
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
        /// Retrieves a specific student by ID
        /// </summary>
        /// <param name="id">The unique identifier of the student</param>
        /// <returns>The student's information if found, otherwise a 404 error</returns>
        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            try
            {
                using (var conn = _dbContext.AccessDatabase())
                {
                    conn.Open();
                    
                    string query = "SELECT studentid, studentfname, studentlname, studentnumber, enroldate " +
                                  "FROM students WHERE studentid = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var student = new Student
                                {
                                    StudentId = Convert.ToInt32(reader["studentid"]),
                                    StudentFName = reader["studentfname"].ToString(),
                                    StudentLName = reader["studentlname"].ToString(),
                                    StudentNumber = reader["studentnumber"].ToString(),
                                    EnrollmentDate = reader["enroldate"] == DBNull.Value ? null : Convert.ToDateTime(reader["enroldate"])
                                };

                                return Ok(student);
                            }
                            else
                            {
                                return NotFound(new { message = $"Student with ID {id} not found." });
                            }
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
        /// Adds a new student to the database
        /// </summary>
        /// <param name="student">The student object containing information to be added</param>
        /// <returns>The newly created student with ID if successful, otherwise an error</returns>
        [HttpPost]
        public IActionResult AddStudent([FromBody] Student student)
        {
            try
            {
                using (var conn = _dbContext.AccessDatabase())
                {
                    conn.Open();
                    
                    // Check if student number already exists
                    string checkQuery = "SELECT COUNT(*) FROM students WHERE studentnumber = @studentnumber";
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@studentnumber", student.StudentNumber);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        
                        if (count > 0)
                        {
                            return BadRequest(new { message = "Student number already exists" });
                        }
                    }
                    
                    // Fix column name to match database (enroldate instead of enrollmentdate)
                    string query = @"INSERT INTO students 
                                   (studentfname, studentlname, studentnumber, enroldate) 
                                   VALUES 
                                   (@studentfname, @studentlname, @studentnumber, @enroldate);
                                   SELECT LAST_INSERT_ID();";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentfname", student.StudentFName);
                        cmd.Parameters.AddWithValue("@studentlname", student.StudentLName);
                        cmd.Parameters.AddWithValue("@studentnumber", student.StudentNumber);
                        cmd.Parameters.AddWithValue("@enroldate", student.EnrollmentDate ?? (object)DBNull.Value);
                        
                        int newId = Convert.ToInt32(cmd.ExecuteScalar());
                        student.StudentId = newId;
                        
                        return CreatedAtAction(nameof(GetStudent), new { id = newId }, student);
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
        /// Deletes a student from the database
        /// </summary>
        /// <param name="id">The unique identifier of the student to delete</param>
        /// <returns>204 No Content if successful, 404 if student not found, or an error</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            try
            {
                using (var conn = _dbContext.AccessDatabase())
                {
                    conn.Open();
                    
                    // First check if the student exists
                    string checkQuery = "SELECT COUNT(*) FROM students WHERE studentid = @id";
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        
                        if (count == 0)
                        {
                            return NotFound(new { message = $"Student with ID {id} not found." });
                        }
                    }
                    
                    // Delete the student
                    string deleteQuery = "DELETE FROM students WHERE studentid = @id";
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