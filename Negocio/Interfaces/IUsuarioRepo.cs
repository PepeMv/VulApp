using Entidades;

namespace Negocio.Interfaces
{
    public interface IUsuarioRepo
    {
        public Task<IEnumerable<Usuario>> DameTodosUsuarios();
        public Task<Usuario> DameUsuarioPorId(int id);

        public Task<Usuario> Logea(string usuario, string contrasenia);
    }
}
