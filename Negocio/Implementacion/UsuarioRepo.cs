using Conexion;
using Dapper;
using Entidades;
using Negocio.Interfaces;

namespace Negocio.Implementacion
{
    public class UsuarioRepo : IUsuarioRepo
    {
        private readonly DapperContext _dapperContext;
        public UsuarioRepo(DapperContext context)
        {
            _dapperContext = context;
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

        public async Task<Usuario> Logea(string codigo, string contrasenia)
        {

            var queryUsuario = "SELECT * FROM `itp_accesos.usuario` WHERE Codigo = '" + codigo + "' ";
            using var conexion = _dapperContext.CreateConnection();

            var usuario = await conexion.QueryFirstOrDefaultAsync<Usuario>(queryUsuario);

            if (usuario == null)
                throw new Exception("Usuario o contrasenia incorrecto.");

            var queryUsuarioInfo = "SELECT * FROM `itp_accesos.informacion_usuario` WHERE idUsuario = '" + usuario.Id + "' and contrasenia = '" + contrasenia + "'";

            var usuarioInformacion = await conexion.QueryFirstOrDefaultAsync<InformacionUsuario>(queryUsuarioInfo);

            if (usuarioInformacion != null)
                return usuario;


            throw new Exception("Usuario o contrasenia incorrecto.");
        }
    }
}
