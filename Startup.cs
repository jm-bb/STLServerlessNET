using MySql.Data.MySqlClient;
using System.Text.Json.Serialization;

namespace STLServerlessNET;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        string serviceDbConnString = Configuration.GetConnectionString("ServiceConnection")!;
        string webDbConnString = Configuration.GetConnectionString("WebConnection")!;

        services.AddCors();
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        services.AddSingleton<MySqlConnection>(provider =>
        {
            return new MySqlConnection(serviceDbConnString);
        });

        services.AddSingleton(provider =>
        {
            return new MySqlConnection(webDbConnString);
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "GetCarriers",
                pattern: "/service/carriers",
                defaults: new { controller = "Carrier", action = "GetCarriers" });

            endpoints.MapControllerRoute(
                name: "GetOrderDetails",
                pattern: "/web/order/{id:int}",
                defaults: new { controller = "Order", action = "GetOrderDetails" });
        });
    }
}