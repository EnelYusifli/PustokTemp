using Microsoft.EntityFrameworkCore;
using PustokTemp.Models;

namespace PustokTemp.DAL
{
    public class PustokDbContext : DbContext
    {
        public PustokDbContext(DbContextOptions<PustokDbContext> options) : base(options) { }

        public DbSet<Slider> Sliders { get; set; }
    }
}
