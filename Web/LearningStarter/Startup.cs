using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using LearningStarter.Data;
using LearningStarter.Entities;
using LearningStarter.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LearningStarter;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
       
        // Add CORS services
        services.AddCors(opt =>
        {
            opt.AddPolicy("corsallow", builder =>
            {
                builder.AllowCredentials();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
                builder.WithOrigins(
                  "http://localhost:3001",
                  "http://localhost:3001",
                  "http://localhost:3002",

                  "https://6mdqms63-5001.uks1.devtunnels.ms/"
                );
            });
        });


        services.AddControllers();

        services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.MaxValue;
            options.Preload = true;
            options.IncludeSubDomains = true;
        });

        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddIdentity<User, Role>(
                options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                    options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                    options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                    options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
                })
            .AddEntityFrameworkStores<DataContext>();

        services.AddMvc();

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

        services.AddAuthorization();

        // Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Learning Starter Server",
                Version = "v1",
                Description = "Description for the API goes here.",
            });

            c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);
            c.MapType(typeof(IFormFile), () => new OpenApiSchema { Type = "file", Format = "binary" });
        });

        services.AddSpaStaticFiles(config => { config.RootPath = "learning-starter-web/build"; });

        services.AddHttpContextAccessor();

        // configure DI for application services
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IPostRepository, PostRepository>();

        services.AddSingleton<WebSocketService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext)
    {
       // dataContext.Database.EnsureDeleted();
        dataContext.Database.EnsureCreated();

        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCors("corsallow");
        app.UseWebSockets();
       // app.UseSpaStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // global cors policy

        // Enable middleware to serve generated Swagger as a JSON endpoint.
        app.UseSwagger(options => { options.SerializeAsV2 = true; });

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
        // specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Learning Starter Server API V1"); });

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(x => x.MapControllers());

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "learning-starter-web";
            if (env.IsDevelopment())
            {
                spa.UseProxyToSpaDevelopmentServer("http://localhost:3001");
            }
        });

        // added custom response headers
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
            //context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            await next();
        });



        using var scope = app.ApplicationServices.CreateScope();
        var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetService<RoleManager<Role>>();

        SeedRoles(dataContext, roleManager).Wait();
        SeedUsers(dataContext, userManager).Wait();
        SeedServerTypes(dataContext);
        SeedServer(dataContext);
        SeedClassroom(dataContext);
        SeedChannel(dataContext);
        SeedStudent(dataContext);
        SeedClassroomStudents(dataContext);
        SeedClassroomChannels(dataContext);

        // #BEGINNING CODE

//added underneath comment}
    }
    
    private static void SeedServer(DataContext dataContext)
    {
        if (dataContext.Set<Server>().Any())
        {
            return;
        }

        var seedServer = new Server
        {
            Name = "Southeastern Louisiana University",
            Description = "College"
        };
        
        dataContext.Set<Server>().Add(seedServer);
        dataContext.SaveChanges();
    }
    
    private static void SeedClassroom(DataContext dataContext)
    {
        if (dataContext.Set<Classroom>().Any())
        {
            return;
        }

        var classroomsToSeed = new List<Classroom>()
        {
            new()
            {
                Name = "Mathematics",
                Description = "Numbers and such"
            },

            new()
            {
                Name = "Computer Science",
                Description = "Computers and such"
            }
        };
        
        dataContext.Set<Classroom>().AddRange(classroomsToSeed);
        dataContext.SaveChanges();
    }
    
    private static void SeedChannel(DataContext dataContext)
    {
        if (dataContext.Set<Channel>().Any())
        {
            return;
        }
        
        var channelsToSeed = new List<Channel>()
        {
            new()
            {
                Name = "Probability",
                Description = "Distributions and stuff"
            },

            new()
            {
                Name = "Algebra",
                Description = "Equations and stuff"
            },
            
            new()
            {
                Name = "Geometry",
                Description = "Shapes and stuff"
            },
            
            new()
            {
                Name = "Programming in Java",
                Description = "Code and stuff"
            },
            
            new()
            {
                Name = "Game Design",
                Description = "Games and stuff"
            },
            
            new()
            {
                Name = "Ethical Hacking",
                Description = "Firewalls and stuff"
            }
        };
        
        dataContext.Set<Channel>().AddRange(channelsToSeed);
        dataContext.SaveChanges();
    }
    
    private static void SeedStudent(DataContext dataContext)
    {
        if (dataContext.Set<Student>().Any())
        {
            return;
        }

        var studentsToSeed = new List<Student>()
        {
            new()
            {
                Name = "Tyler",
                StudentEmail = "tyler@email"
            },

            new()
            {
                Name = "Jimmy",
                StudentEmail = "jimmy@email"
            },
            
            new()
            {
                Name = "Kyle",
                StudentEmail = "kyle@email"
            },
            
            new()
            {
                Name = "Tomar",
                StudentEmail = "tomar@email"
            },
            
            new()
            {
                Name = "Weese",
                StudentEmail = "weese@email"
            },
            
            new()
            {
                Name = "Patrick",
                StudentEmail = "patrick@email"
            },
            
            new()
            {
                Name = "Abe",
                StudentEmail = "abe@email"
            },
            
            new()
            {
                Name = "Ashley",
                StudentEmail = "ashley@email"
            },
            
            new()
            {
                Name = "Robert",
                StudentEmail = "robert@email"
            },
            
            new()
            {
                Name = "Edward",
                StudentEmail = "edward@email"
            }
        };
        
        dataContext.Set<Student>().AddRange(studentsToSeed);
        dataContext.SaveChanges();
    }
    
    private static void SeedClassroomStudents(DataContext dataContext)
    {
        if (dataContext.Set<ClassroomStudents>().Any())
        {
            return;
        }

        var seedingClassroomStudents = new List<ClassroomStudents>()
        {
            new()
            {
                Classroom = dataContext.Set<Classroom>().First(),
                Student = dataContext.Set<Student>().First(),
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().First(),
                Student = dataContext.Set<Student>().ElementAt(1),
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().First(),
                Student = dataContext.Set<Student>().ElementAt(2),
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().First(),
                Student = dataContext.Set<Student>().ElementAt(3),
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().First(),
                Student = dataContext.Set<Student>().ElementAt(4),
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().ElementAt(1),
                Student = dataContext.Set<Student>().ElementAt(5),
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().ElementAt(1),
                Student = dataContext.Set<Student>().ElementAt(6),
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().ElementAt(1),
                Student = dataContext.Set<Student>().ElementAt(7),
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().ElementAt(1),
                Student = dataContext.Set<Student>().ElementAt(8),
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().ElementAt(1),
                Student = dataContext.Set<Student>().ElementAt(9),
            }
        };
        
        dataContext.Set<ClassroomStudents>().AddRange(seedingClassroomStudents);
        dataContext.SaveChanges();
    }
    
    private static void SeedClassroomChannels(DataContext dataContext)
    {
        if (dataContext.Set<ClassroomChannels>().Any())
        {
            return;
        }

        var seedingClassroomChannels = new List<ClassroomChannels>()
        {
            new()
            {
                Classroom = dataContext.Set<Classroom>().First(),
                Channel = dataContext.Set<Channel>().First()
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().First(),
                Channel = dataContext.Set<Channel>().ElementAt(1)
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().First(),
                Channel = dataContext.Set<Channel>().ElementAt(2)
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().ElementAt(1),
                Channel = dataContext.Set<Channel>().ElementAt(3)
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().ElementAt(1),
                Channel = dataContext.Set<Channel>().ElementAt(4)
            },
            
            new()
            {
                Classroom = dataContext.Set<Classroom>().ElementAt(1),
                Channel = dataContext.Set<Channel>().ElementAt(5)
            }
        };
        
        dataContext.Set<ClassroomChannels>().AddRange(seedingClassroomChannels);
        dataContext.SaveChanges();
    }

    private static void SeedServerTypes(DataContext dataContext)
    {
        if (dataContext.Set<ServerTypes>().Any())
        {
            return;
        }

        var seededServerType1 = new ServerTypes
        {
            Name = "School",
            Description = "Southeastern Louisiana University",

        };
        dataContext.Set<ServerTypes>().Add(seededServerType1);
        dataContext.SaveChanges();

        var seededServerType2 = new ServerTypes
        {
            Name = "Class",
            Description = ""
        };
        dataContext.Set<ServerTypes>().Add(seededServerType2);
        dataContext.SaveChanges();
    }



    private static async Task SeedUsers(DataContext dataContext, UserManager<User> userManager)
    {
        var numUsers = dataContext.Users.Count();

        if (numUsers == 0)
        {
            var seededUser1 = new User
            {
                FirstName = "Admin",
                LastName = "User",
                UserName = "admin",
            };
            
            var seededUser2 = new User
            {
                FirstName = "Thomas",
                LastName = "Rumfola",
                UserName = "TRumfola1",
            };

            await userManager.CreateAsync(seededUser1, "Password");
            await userManager.AddToRoleAsync(seededUser1, "Admin");
            
            await userManager.CreateAsync(seededUser2, "Password");
            await userManager.AddToRoleAsync(seededUser2, "Student");
            await dataContext.SaveChangesAsync();
        }
    }

    private static async Task SeedRoles(DataContext dataContext, RoleManager<Role> roleManager)
    {
        var numRoles = dataContext.Roles.Count();

        if (numRoles == 0)
        {
            var seededRole1 = new Role
            {
                Name = "Admin"
            };
            
            var seededRole2 = new Role
            {
                Name = "Student"
            };

            await roleManager.CreateAsync(seededRole1);
            await roleManager.CreateAsync(seededRole2);
            await dataContext.SaveChangesAsync();
        }
    }
}
