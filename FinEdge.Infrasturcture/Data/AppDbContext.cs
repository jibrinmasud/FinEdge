﻿using FinEdge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinEdge.Infrasturcture.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}