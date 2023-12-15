using Conexion;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Negocio.Implementacion;
using Negocio.Interfaces;
using System.Text;

namespace VulApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            var jwtIssuer = _configuration.GetSection("Jwt:Issuer").Get<string>();
            var jwtKey = _configuration.GetSection("Jwt:Key").Get<string>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = jwtIssuer,
                     ValidAudience = jwtIssuer,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                 };
             });

            services.AddScoped<DapperContext>();
            services.AddScoped<IUsuarioRepo, UsuarioRepo>();

            services.AddControllersWithViews();
            services.AddSwaggerGen();

            CargaEnsambladosPraMediatR(services);

            services.AddMvc(x => x.EnableEndpointRouting = false);


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            //app.UseMiddleware<HandlerExceptionMiddleware>();
            app.UseRouting();
            app.UseAuthorization();

            app
                .UseEndpoints(endpoints =>
                {
                    //endpoints.MapControllers();
                    endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                });

        }

        private void CargaEnsambladosPraMediatR(IServiceCollection services)
        {
            //var assemblyQuery = typeof(DameEmpresaPorIdHandler).Assembly;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            assemblies
            .Where(a => a?.FullName?.StartsWith("ITPBusiness") ?? false)
            .ToList()
            .ForEach(x =>
            {
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(x));
            });

        }
    }
}
