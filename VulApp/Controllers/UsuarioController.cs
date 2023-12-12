using MediatR;
using Microsoft.AspNetCore.Mvc;
using Negocio.Interfaces;

namespace VulApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly IUsuarioRepo _usuarioRepo;
        public UsuarioController(ILogger<UsuarioController> logger, IUsuarioRepo usuarioRepo)
        {
            _logger = logger;
            _usuarioRepo = usuarioRepo;
        }


        [HttpGet(Name = "DameTodosUsuarios")]
        [ProducesResponseType(typeof(Unit), StatusCodes.Status200OK)]
        public async Task<IActionResult> DameTodosUsuarios()
        {
            var result = await _usuarioRepo.DameTodosUsuarios();
            return Ok(result);
        }

        [HttpGet("{id}",Name = "DameUsuarioPorId")]
        [ProducesResponseType(typeof(Unit), StatusCodes.Status200OK)]
        public async Task<IActionResult> DameUsuarioPorId(int id)
        {
            var result = await _usuarioRepo.DameUsuarioPorId(id);
            return Ok(result);
        }

        [HttpGet("{codigo}/{contrasenia}", Name = "Logea")]
        [ProducesResponseType(typeof(Unit), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logea(string codigo, string contrasenia)
        {
            var result = await _usuarioRepo.Logea(codigo,contrasenia);
            return Ok(result);
        }


    }
}
