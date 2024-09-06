using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.OutputCaching;
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

builder.Services.AddScoped<IRepositoryGenre, RepositoryGenre>(); //Allows the use of the repository genre in the application

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    // swagger ui for development only
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(); //allow app to use the defined cores above.
app.UseOutputCache(); //use the output cache middleware

app.MapGet("/", [EnableCors(policyName: "open")] () => "Hello World!"); //adding a policy will change how this end point will be accesed.



app.MapGet("/genre", async (IRepositoryGenre repository) =>
{
    return await repository.GetAll();    
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)).Tag("genre-get")); //cache the response for 15 seconds


app.MapGet("/genre/{id:int}", async (IRepositoryGenre repository, int id) =>
{
    var genre = await repository.GetById(id);

    if (genre == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(genre);
});

//post request to create a genre
app.MapPost("/genre", async (Genre genre, IRepositoryGenre repository, IOutputCacheStore outputCache) =>
{
    var id = await repository.Create(genre);

    await outputCache.EvictByTagAsync("genre-get", default); //cleans the cache of the method with the tag.

    return Results.Created($"/genre/{id}", genre);
});

app.MapPut("/genre/{id:int}", async (Genre genre, IRepositoryGenre repository, IOutputCacheStore outputCache, int id) =>
{
    var exists = await repository.Exists(id);

    if(!exists)
    {
        return Results.NotFound();
    }

    await repository.Update(genre);
    await outputCache.EvictByTagAsync("genre-get", default); //cleans the cache of the method with the tag.
    return Results.NoContent();
});

app.MapDelete("/genre/{id:int}", async (IRepositoryGenre repository, IOutputCacheStore outputCache, int id) =>
{
    var exists = await repository.Exists(id);

    if (!exists)
    {
        return Results.NotFound();
    }

    await repository.Delete(id);
    await outputCache.EvictByTagAsync("genre-get", default); //cleans the cache of the method with the tag.
    return Results.NoContent();
});

app.Run();
