using Conexion;
using Dapper;
using Entidades;
using MensajesExternos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Negocio.Clases;
using Negocio.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Negocio.Implementacion
{
    public class UsuarioRepo : IUsuarioRepo
    {
        private readonly DapperContext _dapperContext;
        private readonly IConfiguration _config;
        public UsuarioRepo(DapperContext context, IConfiguration config)
        {
            _dapperContext = context;
            _config = config;
        }

        public async Task<IEnumerable<Usuario>> DameTodosUsuarios()
        {
            var query = "SELECT * FROM `itp_accesos.usuario`";
            using var conexion = _dapperContext.CreateConnection();

            return await conexion.QueryAsync<Usuario>(query);
        }

        public async Task<Usuario> DameUsuarioPorId(int id)
        {
            var query = $"SELECT * FROM `itp_accesos.usuario` WHERE Id = {id}";

            using var conexion = _dapperContext.CreateConnection();

            return await conexion.QuerySingleOrDefaultAsync<Usuario>(query) ?? null;
        }

        public async Task<IActionResult> Actualizausuario(ActualizaUsuarioEntrada entrada)
        {
            var query = $"UPDATE `itp_accesos.usuario` SET Codigo={entrada.Codigo}, Nombre={entrada.Nombre}, Celular={entrada.Celular}, EstaActivo={entrada.EstaActivo} WHERE Id={entrada.Id};";
            
            using var conexion = _dapperContext.CreateConnection();

            var xx = await conexion.ExecuteAsync(query);

            return (IActionResult)Task.FromResult(0);
        }
        public async Task<LoginResponse> Login(string codigo, string contrasenia)
        {

            var queryUsuario = "SELECT * FROM `itp_accesos.usuario` WHERE Codigo = '" + codigo + "' ";
            using var conexion = _dapperContext.CreateConnection();

            var usuario = await conexion.QueryFirstOrDefaultAsync<Usuario>(queryUsuario);

            if (usuario == null)
                throw new Exception("Usuario o contrasenia incorrecto.");

            var queryUsuarioInfo = "SELECT * FROM `itp_accesos.informacion_usuario` WHERE idUsuario = '" + usuario.Id + "' and contrasenia = '" + contrasenia + "'";

            var usuarioInformacion = await conexion.QueryFirstOrDefaultAsync<InformacionUsuario>(queryUsuarioInfo);

            if (usuarioInformacion == null)
                throw new Exception("Usuario o contrasenia incorrecto.");

            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            //var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, usuario.Nombre),
            //    new Claim(ClaimTypes.Role, usuario.Codigo),
            
            //};

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);


            return new LoginResponse(token,null);

            
        }
    }
}
