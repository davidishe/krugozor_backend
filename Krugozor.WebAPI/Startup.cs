using System.IO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using WebAPI.Middleware;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Krugozor.Infrastructure.Database;
using Core.Models;
using Krugozor.Identity.Database;
using Krugozor.Identity.Extensions;
using Krugozor.Infrastructure;
using Krugozor.Core.Models.Items;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Krugozor.Core.Models;
using Krugozor.Infrastructure.Strapi;
using Krugozor.Email.EmailService;
using Krugozor.Infrastructure.Messenger.Hubs;
using Krugozor.Core.Models.Messages;

namespace WebAPI
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      _config = configuration;
    }

    public IConfiguration _config { get; }


    public void ConfigureDevelopmentServices(IServiceCollection services)
    {
      ConfigureServices(services);
    }

    public void ConfigureProductionServices(IServiceCollection services)
    {
      ConfigureServices(services);
    }

    public void ConfigureServices(IServiceCollection services)
    {

      services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("DefaultConnection")));
      services.AddDbContext<IdentityContext>(options => options.UseSqlServer(_config.GetConnectionString("IdentityConnection")));


      services.AddIdentityServices(_config);
      services.AddApplicationServices();


      services.AddControllers(options =>
      {
        var policy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();

        options.Filters.Add(new AuthorizeFilter(policy));
      })
        .AddNewtonsoftJson(opt =>
        {
          opt.SerializerSettings.ReferenceLoopHandling =
          Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });

      services.AddCors();
      services.AddAutoMapper(typeof(Startup));

      services.AddScoped<IDbRepository<ProposalProfile>, DbRepository<ProposalProfile>>();
      services.AddScoped<IDbRepository<Request>, DbRepository<Request>>();
      services.AddScoped<IDbRepository<Favour>, DbRepository<Favour>>();
      services.AddScoped<IDbRepository<Grade>, DbRepository<Grade>>();
      services.AddScoped<IDbRepository<RequestStatus>, DbRepository<RequestStatus>>();
      services.AddScoped<IDbRepository<ProposalProfileStatus>, DbRepository<ProposalProfileStatus>>();
      services.AddScoped<IDbRepository<Message>, DbRepository<Message>>();
      services.AddScoped<IDbRepository<Correspondent>, DbRepository<Correspondent>>();

      services.AddSignalR();

      // services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
      // services.AddScoped<IUnitOfWork, UnitOfWork>();

      services.AddScoped<IDbRepository<ProposalDescriptionTranaltaion>, DbRepository<ProposalDescriptionTranaltaion>>();
      services.AddScoped<IStrapiService, StrapiService>();
      services.AddScoped<IEmailService, EmailService>();


      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationService", Version = "v1" });
      });
      services.AddDirectoryBrowser();


    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chtole bot v1"));
      }


      app.UseMiddleware<ExceptionMiddleware>();
      app.UseStatusCodePagesWithReExecute("/errors/{0}");
      app.UseRouting();
      app
        .UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
        .WithOrigins(
                      "https://localhost:4200",
                      "https://localhost:4300",
                      "https://localhost:4400",
                      "https://localhost:4500",
                      "https://api.telecost.pro",
                      "https://api.krugozor.davidishe.pro",
                      "https://krugozor.space",
                      "https://krugozor.space",
                      "https://api.krugozor.davidishe.pro",
                      "https://telecost.pro",
                      "https://property.telecost.pro/",
                      "https://property.telecost.pro",
                      "*")
        .AllowCredentials());


      app.UseAuthentication();
      app.UseAuthorization();
      app.UseDefaultFiles();
      // app.UseStaticFiles();
      app.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(
          Path.Combine(Directory.GetCurrentDirectory(), "Assets")
        ),
        RequestPath = "/assets"
      });




      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapHub<ChatHub>("/chathub");
        // endpoints.MapFallbackToController("Index", "Fallback");
      });


      IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en"), //English US
                new CultureInfo("ar"), //Arabic SY
            };
      var localizationOptions = new RequestLocalizationOptions
      {
        DefaultRequestCulture = new RequestCulture("en"), //English US will be the default culture (for new visitors)
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures
      };

      app.UseRequestLocalization(localizationOptions);
    }
  }
}
