using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebReservation.Models;

namespace WebReservation.Data
{
    public class WebReservationContext : IdentityDbContext
    {
        public WebReservationContext (DbContextOptions<WebReservationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Choice> Choice { get; set; }
        public DbSet<Building> Building { get; set; }
    }
}
