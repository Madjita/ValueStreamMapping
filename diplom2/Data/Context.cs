using System;
using auntification.Models;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace diplom2.Data
{

   public class Context : DbContext
   {

      public Context(DbContextOptions<Context> options) : base(options)
      {
            this.Database.SetCommandTimeout(99999999);
        }

        public DbSet<Productions> Productions { set; get; }
        public DbSet<Order> Order { set; get; }
        public DbSet<Orders_production> Orders_production { set; get; }
        public DbSet<Orders_production_items> Orders_production_items { set; get; }

        public DbSet<User> Users { set; get; }
        public DbSet<BufferVSM> BufferVSM { set; get; }
        public DbSet<BufferVSMQueue> BufferVSMQueue { set; get; }
        public DbSet<CardVSM> CardVSM { set; get; }
        public DbSet<EtapVSM> EtapVSM { set; get; }
        public DbSet<EtapSections> EtapSections { set; get; }
        public DbSet<ArchiveSection> ArchiveSection { set; get; }
        public DbSet<ActualOrderCurrentSection> ActualOrderCurrentSection { get; set; }





        /* public DbSet<User> Users { set; get; }
           public DbSet<Productions> Productions { set; get; }
           public DbSet<BufferVSM> BufferVSM { set; get; }
           public DbSet<CardVSM> CardVSM { set; get; }
           public DbSet<EtapVSM> EtapVSM { set; get; }
           public DbSet<EtapSections> EtapSections { set; get; }
           public DbSet<Orders_production> Orders_production { set; get; }
           public DbSet<Orders_production_items> Orders_production_items { set; get; }
           public DbSet<ArchiveSection> ArchiveSection { set; get; }*/
        //public DbSet<OrderCurrentSection> OrderCurrentSection { set; get; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
                modelBuilder.Entity<User>(entity => { entity.HasIndex(e => e.Email).IsUnique(); });
       }
   }
}
