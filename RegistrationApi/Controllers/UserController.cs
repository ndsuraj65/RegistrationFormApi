using Microsoft.AspNetCore.Mvc;
using RegistrationApi.Business;

using RegistrationApi.Models;

namespace RegistrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] UserModel user)
        {
            try
            {
                _service.CreateUser(user);
                return Ok(new { message = "User created successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "You are entered duplicate Email Id-Please enter Unique.", detail = ex.Message });
            }
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            try
            {
                var users = _service.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Users Not found .", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var user = _service.GetUserById(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "User not found.", detail = ex.Message });
            }
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] UserModel user)
        {
            try
            {
                _service.UpdateUser(user);
                return Ok(new { message = "User updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "User not exist.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _service.DeleteUser(id);
                return Ok(new { message = "User deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "User not exist.", detail = ex.Message });
            }
        }
    }
}
