using CustomOidcClaims.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Security.Claims;

namespace CustomOidcClaims
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<UserDbContext>(o => o.UseSqlServer(connectionString));

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                o.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(o =>
            {
                Configuration.GetSection("Authentication").Bind(o);

                o.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = async ctx =>
                    {
                        //Get the user's unique identifier
                        string oid = ctx.Principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
                        //Get the Azure AD tenant identifier
                        string tid = ctx.Principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");
                        string userPrincipalName = ctx.Principal.FindFirstValue(ClaimTypes.Name);

                        var db = ctx.HttpContext.RequestServices.GetRequiredService<UserDbContext>();
                        User user = await db.Users.SingleOrDefaultAsync(u => u.ObjectId == oid && u.TenantId == tid);

                        if(user == null)
                        {
                            //Add user to database
                            user = new User
                            {
                                IsAdmin = false,
                                ObjectId = oid,
                                TenantId = tid,
                                UserPrincipalName = userPrincipalName
                            };
                            await db.Users.AddAsync(user);
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            // UPN can change, so update it
                            user.UserPrincipalName = userPrincipalName;
                            await db.SaveChangesAsync();
                        }
                        
                        if (user.IsAdmin)
                        {
                            //If the user is an admin, add them the role
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Role, "admin")
                            };
                            var appIdentity = new ClaimsIdentity(claims);

                            ctx.Principal.AddIdentity(appIdentity);
                        }
                    }
                };
            });
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
