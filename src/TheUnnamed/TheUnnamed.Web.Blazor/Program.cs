using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Authentication.WebAssembly.Msal.Models;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using TheUnnamed.Core.Database;
using TheUnnamed.Core.Database.Config;
using TheUnnamed.Core.Storage;
using TheUnnamed.Core.Storage.Config;
using TheUnnamed.Web.Blazor.Jobs;
using TheUnnamed.Web.Blazor.Service;
using IAccessTokenProvider = Microsoft.AspNetCore.Components.WebAssembly.Authentication.IAccessTokenProvider;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

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

            builder.Services.AddGraphClient(builder.Configuration["MicrosoftGraph:BaseUrl"], builder.Configuration["MicrosoftGraph:Scopes"].Split(" ").ToList());

            // Get the configuration
            var dbConfig = builder.Configuration.GetSection(nameof(DatabaseConfiguration)).Get<DatabaseConfiguration>()
                           ?? throw new NullReferenceException("Database configuration cannot be resolved");
            builder.Services.AddSingleton(dbConfig);
            var fsConfig = builder.Configuration.GetSection(nameof(StorageConfiguration)).Get<StorageConfiguration>()
                           ?? throw new NullReferenceException("Storage configuration cannot be resolved");
            builder.Services.AddSingleton(fsConfig);

            // enable logging.
            builder.Services.AddLogging(logging => { logging.AddConsole(); });

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

    internal static class GraphClientExtensions
    {
        public static IServiceCollection AddGraphClient(this IServiceCollection services, string? baseUrl, List<string>? scopes)
        {
            if (string.IsNullOrEmpty(baseUrl) || scopes.IsNullOrEmpty())
            {
                return services;
            }

            services.Configure<RemoteAuthenticationOptions<MsalProviderOptions>>(
                options =>
                {
                    scopes?.ForEach((scope) => { options.ProviderOptions.DefaultAccessTokenScopes.Add(scope); });
                });

            services.AddScoped<IAuthenticationProvider, GraphAuthenticationProvider>();

            services.AddScoped(sp =>
            {
                return new GraphServiceClient(
                    new HttpClient(),
                    sp.GetRequiredService<IAuthenticationProvider>(),
                    baseUrl);
            });

            return services;
        }

        private class GraphAuthenticationProvider : IAuthenticationProvider
        {
            private readonly IConfiguration config;

            public GraphAuthenticationProvider(
                IAccessTokenProvider tokenProvider,
                IConfiguration config)
            {
                TokenProvider = tokenProvider;
                this.config = config;
            }

            public IAccessTokenProvider TokenProvider { get; }

            public async Task AuthenticateRequestAsync(RequestInformation request,
                Dictionary<string, object>? additionalAuthenticationContext = null,
                CancellationToken cancellationToken = default)
            {
                var result = await TokenProvider.RequestAccessToken(
                    new AccessTokenRequestOptions()
                    {
                        Scopes =
                            config.GetSection("MicrosoftGraph:Scopes").Get<string[]>()
                    });

                if (result.TryGetToken(out var token))
                {
                    request.Headers.Add("Authorization",
                        $"{CoreConstants.Headers.Bearer} {token.Value}");
                }
            }
        }

    }
}
