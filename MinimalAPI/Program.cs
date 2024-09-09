using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MinimalAPI;
using MinimalAPI.EndPoints;
using MinimalAPI.Entities;
using MinimalAPI.Repository;
using MinimalAPI.Services;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetValue<string>("allowed_origins")!;

builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer("name=DefaultConnection"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy( settings =>
    {
        settings.WithOrigins(allowedOrigins) //allow any origin
               .AllowAnyMethod() //allow any HTTP method POST,PUT,DELETE,GET etc
               .AllowAnyHeader(); //allow any header to be used to make the request
    });

    options.AddPolicy("open", settings =>
    {
        settings.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });

});

builder.Services.AddOutputCache();
//swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositoryGenre, RepositoryGenre>(); //Allows the use of the repository genre in the application
builder.Services.AddScoped<IRepositoryActor, RepositoryActor>(); //Allows the use of the repository genre in the application
builder.Services.AddScoped<IRepositoryMovie, RepositoryMovie>(); //Allows the use of the repository genre in the application

//file storage
builder.Services.AddScoped<I_FileStore, FileStorage>(); //Allows the use of the file store in the application
builder.Services.AddHttpContextAccessor(); //Allows the use of the http context accessor in the application

builder.Services.AddAutoMapper(typeof(Program)); //Allows the use of automapper in the application

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    // swagger ui for development only
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); //allow the use of static files in the application

app.UseCors(); //allow app to use the defined cores above.
app.UseOutputCache(); //use the output cache middleware

app.MapGet("/", [EnableCors(policyName: "open")] () => "Hello World!"); //adding a policy will change how this end point will be accesed.

app.MapGroup("/genre").MapGenre();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();

app.Run();
