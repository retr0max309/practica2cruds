using Microsoft.EntityFrameworkCore;
using Pr2Cruds.Models;

namespace Pr2Cruds.Data
{
    public class TareaDbContext : DbContext
    {
        public TareaDbContext(DbContextOptions<TareaDbContext> options) : base(options) { }

        // Un solo DbSet, en plural
        public DbSet<Tarea> Tareas { get; set; } = null!;
    }
}
