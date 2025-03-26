using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using CSHarpC1.Models;

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
        /// GET: api/TeacherAPI
        /// Returns a list of all teachers in the database.
        /// </summary>
        [HttpGet]
        public IActionResult GetAllTeachers()
        {
            using (var conn = _dbContext.AccessDatabase())
            {
                conn.Open();
                
                // Updated query to match actual database column names
                string query = "SELECT teacherid, teacherfname, teacherlname, employeenumber, hiredate, salary FROM teachers";
                
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
                                Salary = reader["salary"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["salary"])
                            };
                            teachers.Add(teacher);
                        }

                        return Ok(teachers); 
                    }
                }
            }
        }

        /// <summary>
        /// GET: api/TeacherAPI/{id}
        /// Returns a single teacher by ID.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetTeacher(int id)
        {
            try 
            {
                using (var conn = _dbContext.AccessDatabase())
                {
                    conn.Open();
                    
                    // Updated query to match actual database column names
                    string query = "SELECT teacherid, teacherfname, teacherlname, employeenumber, hiredate, salary FROM teachers WHERE teacherid = @id";
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
                                    Salary = reader["salary"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["salary"])
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
    }
}
