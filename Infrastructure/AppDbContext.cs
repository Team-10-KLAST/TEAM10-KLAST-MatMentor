using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure;

public class AppDbContext : DbContext
{

   public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
   public DbSet<Student> Students => Set<Student>();
   public DbSet<Interest> Interests => Set<Interest>();
}
