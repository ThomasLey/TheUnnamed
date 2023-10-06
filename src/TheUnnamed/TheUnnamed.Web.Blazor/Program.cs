using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using TheUnnamed.Core.Database;
using TheUnnamed.Core.Database.Config;
using TheUnnamed.Core.Storage;
using TheUnnamed.Core.Storage.Config;
using TheUnnamed.Web.Blazor.Jobs;
using TheUnnamed.Web.Blazor.Service;

namespace TheUnnamed.Web.Blazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // todo: tley: add for environmane name
            builder.Configuration
                //.AddJsonFile($"appsettings.{CurrentEnvironment.EnvironmentName}.json", optional: true) //load environment settings
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddEnvironmentVariables(prefix: "tup_");

            var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ')
                ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');

            // Get the configuration
            var dbConfig = builder.Configuration.GetSection(nameof(DatabaseConfiguration)).Get<DatabaseConfiguration>()
                ?? throw new NullReferenceException("Database configuration cannot be resolved");
            builder.Services.AddSingleton(dbConfig);
            var fsConfig = builder.Configuration.GetSection(nameof(StorageConfiguration)).Get<StorageConfiguration>()
                           ?? throw new NullReferenceException("Storage configuration cannot be resolved");
            builder.Services.AddSingleton(fsConfig);

            // enable logging.
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
            });

            // Add services to the container.
            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
            builder.Services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();

            builder.Services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;
            });

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
            builder.Services.AddControllers();

            // Adding my services for razor pages (Controller)
            builder.Services.AddTransient<IndexService>();
            builder.Services.AddTransient<LoginService>();
            builder.Services.AddTransient<ListMyDocumentsService>();

            // Adding my services with persistent store
            builder.Services.AddUnnamedDatabase(dbConfig);
            builder.Services.AddUnnamedStorage(fsConfig);

            builder.Services.AddHostedService<RemoveDocumentsWithoutStream>();

            var app = builder.Build();

            // app.UseUnnamedDatabase() to create DB
            // app.UseUnnamedStorage() to create buckets, Storage and Tables

            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetService<DocumentsContext>();
            db.Database.EnsureCreated();
            scope.Dispose();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}