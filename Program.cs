using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
Console.WriteLine("First string:" + builder.Configuration.GetConnectionString("EventDbConnection"));

builder.Services.AddDbContext<EventDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventDbConnection"))
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(builder =>  builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);


var eventItems = app.MapGroup("/eventitems");
eventItems.MapGet("/", GetAllEvents);
eventItems.MapGet("/{id:int}", GetEvent);
eventItems.MapPost("/", CreateEvent);
eventItems.MapPut("/{id:int}", UpdateEvent);
eventItems.MapDelete("/{id:int}", DeleteEvent);

app.Run();

static async Task<IResult> GetAllEvents(EventDb db)
{
    Console.WriteLine("GetAllEvents");
    return TypedResults.Ok(await db.Events.ToArrayAsync());
}

static async Task<IResult> GetEvent(int id, EventDb db)
{
    return await db.Events.FindAsync(id)
        is Event publicEvent
            ? TypedResults.Ok(publicEvent)
            : TypedResults.NotFound();
}

static async Task<IResult> CreateEvent(Event publicEvent, EventDb db)
{
    db.Events.Add(publicEvent);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/eventitems/{publicEvent.Id}", publicEvent);
}

static async Task<IResult> UpdateEvent(int id, Event inputEvent, EventDb db)
{
    var publicEvent = await db.Events.FindAsync(id);

    if (publicEvent is null) return TypedResults.NotFound();

    publicEvent.Name = inputEvent.Name;
    publicEvent.Name = inputEvent.Name;
    publicEvent.Date = inputEvent.Date;
    publicEvent.Description = inputEvent.Description;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteEvent(int id, EventDb db)
{
    if (await db.Events.FindAsync(id) is Event publicEvent)
    {
        db.Events.Remove(publicEvent);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}
