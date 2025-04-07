using FluentMigrator.Runner;
using konnect_player_info.Model.Manager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using System;

using TODO_API_net.Models.basicAuth;
using static TODO_API_net.Models.basicAuth.BasicAuthServices;

namespace konnect_player_info
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IBasicAuthService, BasicAuthService>();

            var db_host = Environment.GetEnvironmentVariable("DB_HOST");
            var db_port = Environment.GetEnvironmentVariable("DB_PORT");
            var db_name = Environment.GetEnvironmentVariable("DB_NAME");
            var db_user = Environment.GetEnvironmentVariable("DB_USER");
            var db_password = Environment.GetEnvironmentVariable("DB_PASS");

            var conn = $"Server={db_host};Port={db_port};Database={db_name};Uid={db_user};" +
                $"Pwd={db_password};Convert Zero Datetime=True";

            BaseManager.ConnectionString = conn; 

            services.AddTransient<MySqlConnection>(_ => new MySqlConnection(conn));

            BasicAuthServices.conn = conn;
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                .AddMySql5()
                .WithGlobalConnectionString(conn)
                .ScanIn(typeof(Startup).Assembly).For.Migrations());

            services.AddHttpContextAccessor();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            using (var scope = app.ApplicationServices.CreateScope())
            {
                Console.WriteLine("\n");
                var dbConnection = scope.ServiceProvider.GetRequiredService<MySqlConnection>();
                try
                {
                    dbConnection.Open();
                    Console.WriteLine("Database connection successful!!!");
                    dbConnection.Close();
                }
                catch (Exception error)
                {
                    Console.WriteLine($"X Database connection failed: {error.Message}");
                }

                try
                {
                    var migrator = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    //migrator.MigrateDown(0);// remove all
                    migrator.MigrateUp();
                    Console.WriteLine("Migrations applied successfully!");
                }
                catch (Exception error)
                {
                    Console.WriteLine($"X Migration failed: {error.Message}");
                }
                Console.WriteLine("\n");


            }


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to TODO API!");
                });
            });
        }
    }
}
