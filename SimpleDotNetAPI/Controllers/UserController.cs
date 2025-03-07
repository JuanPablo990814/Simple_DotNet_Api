using Microsoft.AspNetCore.Mvc;
using SimpleDotNetAPI.Models;
using SimpleDotNetAPI.Services;


namespace SimpleDotNetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // Obtener todos los usuarios
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        // Obtener un usuario por ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null) 
                return NotFound("Usuario no encontrado");
            return Ok(user);
        }

        // Crear un nuevo usuario
        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            _userService.AddUser(user);
            return Created($"/api/User/{user.Id}", user);
        }

        // Actualizar un usuario existente
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!_userService.UpdateUser(id, updatedUser)) 
                return NotFound("Usuario no encontrado");
            return Ok(updatedUser);
        }

        // Eliminar un usuario
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            if (!_userService.DeleteUser(id)) 
                return NotFound("Usuario no encontrado");
            return NoContent();
        }
    }
}