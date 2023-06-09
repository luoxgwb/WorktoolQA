﻿using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public sealed class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<WorkToolCount> WorkToolCount { get; set; }
        public DbSet<WorkToolLog> WorkToolLog { get; set; }
    }
}
