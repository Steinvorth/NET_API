using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using MinimalAPI;
using MinimalAPI.Entities;
using MinimalAPI.Repository;

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

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    // swagger ui for development only
    app.UseSwagger();
    app.UseSwaggerUI();
}

builder.Services.AddScoped<IRepositoryGenre, RepositoryGenre>(); //Allows the use of the repository genre in the application

app.UseCors(); //allow app to use the defined cores above.
app.UseOutputCache(); //use the output cache middleware

app.MapGet("/", [EnableCors(policyName: "open")] () => "Hello World!"); //adding a policy will change how this end point will be accesed.

app.MapGet("/genre", () =>
{
    var genres = new List<Genre>
    {
        new Genre()
        {
            Id = 1,
            Name = "Action"
        },
        new Genre()
        {
            Id = 2,
            Name = "Adventure"
        },
        new Genre()
        {
            Id = 3,
            Name = "Comedy"
        },
        new Genre()
        {
            Id = 4,
            Name = "Drama"
        }
    };

    return genres;
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15))); //cache the response for 15 seconds

//post request to create a genre
app.MapPost("/genre", async (IRepositoryGenre repository, Genre genre) =>
{
    var id = await repository.Create(genre);
    return Results.Created($"/genre/{id}", genre);
});

app.Run();
