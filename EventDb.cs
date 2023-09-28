using Microsoft.EntityFrameworkCore;

public class EventDb : DbContext
{
    public EventDb(DbContextOptions<EventDb> options)
        : base(options) { }

    public DbSet<Event> Events => Set<Event>();
}