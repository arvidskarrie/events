using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EventDatabaseBackend
{
    public static class EventEndpoints
    {
        public static void InitiateEndpoints(RouteGroupBuilder mapGroup)
        {
            mapGroup.MapGet("/", GetAllEvents);
            mapGroup.MapGet("/{id:int}", GetEvent);
            mapGroup.MapPost("/", CreateEvent);
            mapGroup.MapPut("/{id:int}", UpdateEvent);
            mapGroup.MapDelete("/{id:int}", DeleteEvent);
        }

        public static async Task<IResult> GetAllEvents(EventDb db)
        {
            Console.WriteLine("GetAllEvents");
            var events = await db.Events.Where(e => !string.IsNullOrEmpty(e.Name)).ToArrayAsync();
            return TypedResults.Ok(events);
        }

        public static async Task<IResult> GetEvent(int id, EventDb db)
        {
            Console.WriteLine("GetEvent");
            return await db.Events.FindAsync(id)
                is Event publicEvent
                    ? TypedResults.Ok(publicEvent)
                    : TypedResults.NotFound();
        }

        public static async Task<IResult> CreateEvent(HttpContext httpContext, EventDb db)
        {
            Console.WriteLine("CreateEvent");
            // Read JSON from request body into a string
            string requestBody;
            using (StreamReader reader = new StreamReader(httpContext.Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Deserialize JSON to an Event object
            Console.WriteLine("requestBody " + requestBody);
            Event publicEvent = JsonSerializer.Deserialize<Event>(requestBody);
            Console.WriteLine("Event data: " + publicEvent.Name + " " + publicEvent.Id + " " + publicEvent.Description + " " + publicEvent.Date + " ");
            // Check for null or validation
            if (publicEvent == null)
            {
                return Results.BadRequest("Invalid input data");
            }

            // Add to database and save changes
            db.Events.Add(publicEvent);
            await db.SaveChangesAsync();

            // Return success response
            return Results.Created($"/eventitems/{publicEvent.Id}", publicEvent);
        }

        public static async Task<IResult> UpdateEvent(int id, Event inputEvent, EventDb db)
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

        public static async Task<IResult> DeleteEvent(int id, EventDb db)
        {
            Console.WriteLine("DeleteEvent");
            if (await db.Events.FindAsync(id) is Event publicEvent)
            {
                db.Events.Remove(publicEvent);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();
        }
    }
}
