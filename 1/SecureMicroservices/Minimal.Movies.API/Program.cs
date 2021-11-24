using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//Enable the API explorer
builder.Services.AddEndpointsApiExplorer();

//Add Open API Services to the container
builder.Services.AddSwaggerGen();

//Add EntityFrameworkCore DB context
builder.Services.AddDbContext<MoviesAPIContext>(options =>
                    options.UseInMemoryDatabase("Movies"));


builder.Services.AddAuthorization();

//Add authentication services
builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    //Identity Server 4
                    options.Authority = "https://localhost:5006";

                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

var app = builder.Build();


//Configure for development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

SeedDatabase(app);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//Enable Get of movies
app.MapGet("/movies", async(MoviesAPIContext context) => await context.Movie.ToListAsync())
    .Produces<IEnumerable<Movie>>(StatusCodes.Status200OK)
    .WithName("GetAllMovies")
    .WithTags("Getters")
    .RequireAuthorization();

//Enable Get for a specific movie
app.MapGet("/movies/{id}", async (MoviesAPIContext context, int id) =>
{
    var movie = await context.Movie.FindAsync(id);

    if (movie == null)
    {        
        return Results.NotFound();
    }

    return Results.Ok(movie);

})
.Produces<Movie>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("GetMovies")
.WithTags("Getters");

//Enable Creation of a new movie
app.MapPost("/movies/", async ([FromBody] Movie movie, [FromServices] MoviesAPIContext context, HttpResponse httpResponse) =>
{
    context.Movie.Add(movie);
    await context.SaveChangesAsync();

    httpResponse.StatusCode = 201;
    httpResponse.Headers.Location = $"/movies/{movie.Id}";

})
.Accepts<Movie>("application/json")
.Produces<Movie>(StatusCodes.Status201Created)
.WithName("CreateMovie")
.WithTags("Creators");

app.Run();

static void SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var moviesContext = services.GetRequiredService<MoviesAPIContext>();
    MovieContextSeed.SeedAsync(moviesContext);
}

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
    public string Rating { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string ImageUrl { get; set; }
    public string Owner { get; set; }
}

public class MoviesAPIContext : DbContext
{
    public MoviesAPIContext(DbContextOptions<MoviesAPIContext> options)
            : base(options)
    {
    }

    public DbSet<Movie> Movie { get; set; }
}

public class MovieContextSeed
{
    public static void SeedAsync(MoviesAPIContext moviesContext)
    {
        if (!moviesContext.Movie.Any())
        {
            var movies = new List<Movie>
                {
                    new Movie
                    {
                        Id = 1,
                        Genre = "Drama",
                        Title = "The Shawshank Redemption",
                        Rating = "9.3",
                        ImageUrl = "images/src",
                        ReleaseDate = new DateTime(1994, 5, 5),
                        Owner = "alice"
                    },
                    new Movie
                    {
                        Id = 2,
                        Genre = "Crime",
                        Title = "The Godfather",
                        Rating = "9.2",
                        ImageUrl = "images/src",
                        ReleaseDate = new DateTime(1972, 5, 5),
                        Owner = "alice"
                    },
                    new Movie
                    {
                        Id = 3,
                        Genre = "Action",
                        Title = "The Dark Knight",
                        Rating = "9.1",
                        ImageUrl = "images/src",
                        ReleaseDate = new DateTime(2008, 5, 5),
                        Owner = "bob"
                    },
                    new Movie
                    {
                        Id = 4,
                        Genre = "Crime",
                        Title = "12 Angry Men",
                        Rating = "8.9",
                        ImageUrl = "images/src",
                        ReleaseDate = new DateTime(1957, 5, 5),
                        Owner = "bob"
                    },
                    new Movie
                    {
                        Id = 5,
                        Genre = "Biography",
                        Title = "Schindler's List",
                        Rating = "8.9",
                        ImageUrl = "images/src",
                        ReleaseDate = new DateTime(1993, 5, 5),
                        Owner = "alice"
                    },
                    new Movie
                    {
                        Id = 6,
                        Genre = "Drama",
                        Title = "Pulp Fiction",
                        Rating = "8.9",
                        ImageUrl = "images/src",
                        ReleaseDate = new DateTime(1994, 5, 5),
                        Owner = "alice"
                    },
                    new Movie
                    {
                        Id = 7,
                        Genre = "Drama",
                        Title = "Fight Club",
                        Rating = "8.8",
                        ImageUrl = "images/src",
                        ReleaseDate = new DateTime(1999, 5, 5),
                        Owner = "bob"
                    },
                    new Movie
                    {
                        Id = 8,
                        Genre = "Romance",
                        Title = "Forrest Gump",
                        Rating = "8.8",
                        ImageUrl = "images/src",
                        ReleaseDate = new DateTime(1994, 5, 5),
                        Owner = "bob"
                    }
                };

            moviesContext.Movie.AddRange(movies);
            moviesContext.SaveChanges();
        }
    }
}
