using Microsoft.EntityFrameworkCore;

namespace ContactList.Server.Model;

public partial class Database : DbContext
{
    public Database(DbContextOptions<Database> options)
        : base(options)
    {
    }
}
