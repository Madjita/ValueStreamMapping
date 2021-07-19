using System;
using Microsoft.EntityFrameworkCore;
using DiplomReactNetCore.DAL.Models.DataBase;
using System.Collections.Generic;
using DiplomReactNetCore.DAL.Models.Work;

namespace DiplomReactNetCore.DAL.Context
{
    public class MyContext : DbContext
    {
        public DbSet<Production> Production { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<BufferVSM> BufferVSM { get; set; }
        public DbSet<CardVSM> CardVSM { get; set; }
        public DbSet<EtapVSM> EtapVSM { get; set; }
        public DbSet<QueueBufferVSM> QueueBufferVSM { get; set; }

        //new
        public DbSet<OrderListForWork> OrderListForWork { get; set; }
        public DbSet<OrderListFinishedWork> OrderListFinishedWork { get; set; }



        public MyContext(DbContextOptions<MyContext> options)
        : base(options)
        {
        }


        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           optionsBuilder.UseSqlite("Filename=DBDiplom.db"); //"Filename=DBDiplom.db"
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public void Init()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

            CreateProductionAsync();
            CreateBufferVSMAsync();
            CreateEtapVSMAsync();
            CreateCardVSMAsync();

        }

        public void CreateProductionAsync()
        {
            Production.Add(
               new Production
               {
                   Name = "Кофе"
               });

            SaveChanges();
        }

        public void CreateBufferVSMAsync()
        {
            BufferVSM.Add(
               new BufferVSM
               {
                   Name = "Кофе",
                   Type = "гр",
                   MinHold = 100,
                   Max = 1000,
                   Value = 150,
                   ValueDefault = 50,
                   ReplenishmentSec = 60,
                   ReplenishmentCount = 100,
                   Parallel = false,
               }); ;

            BufferVSM.Add(
                new BufferVSM
                {
                    Name = "Фильтры",
                    Type = "шт",
                    MinHold = 1,
                    Max = 10,
                    Value = 2,
                    ValueDefault = 1,
                    ReplenishmentSec = 60,
                    ReplenishmentCount = 3,
                    Parallel = false,
                });

            BufferVSM.Add(
                new BufferVSM
                {
                    Name = "Вода",
                    Type = "л",
                    MinHold = 0,
                    Max = 999,
                    Value = 100,
                    ValueDefault = 1,
                    ReplenishmentSec = 60,
                    ReplenishmentCount = 5,
                    Parallel = false,
                });

            SaveChanges();
        }

        public void CreateEtapVSMAsync()
        {

            EtapVSM.Add(
                new EtapVSM
                {
                    Name = "Отделение для фильтра",
                    Description = "Установить фильтр",
                    ActualTimeCircle = 10,
                    DefaultTimeCircle = 10,
                    ActualTimePreporation = 0,
                    DefaultTimePreporation = 0,
                    ActualAvailability = 0,
                    Time = 0,
                    Parallel = false,
                }
             );

          EtapVSM.Add(
                new EtapVSM
                {
                    Name = "Кофеварка",
                    Description = "Молоть кофе",
                    ActualTimeCircle = 15,
                    DefaultTimeCircle = 30,
                    ActualTimePreporation = 0,
                    DefaultTimePreporation = 0,
                    ActualAvailability = 100,
                    Time = 0,
                    Parallel = false,
                });

            EtapVSM.Add(
                new EtapVSM
                {
                    Name = "Кофеварка",
                    Description = "Отмерить и добавить молотый кофе",
                    ActualTimeCircle = 15,
                    DefaultTimeCircle = 15,
                    ActualTimePreporation = 0,
                    DefaultTimePreporation = 0,
                    ActualAvailability = 100,
                    Time = 0,
                    Parallel = false,
                });

            EtapVSM.Add(
               new EtapVSM
               {
                   Name = "Кофеварка",
                   Description = "Добавить воды",
                   ActualTimeCircle = 11,
                   DefaultTimeCircle = 11,
                   ActualTimePreporation = 0,
                   DefaultTimePreporation = 0,
                   ActualAvailability = 100,
                   Time = 0,
                   Parallel = false,
               });

            EtapVSM.Add(
               new EtapVSM
               {
                   Name = "Кофеварка",
                   Description = "Варить кофе",
                   ActualTimeCircle = 40, //300
                   DefaultTimeCircle = 40,
                   ActualTimePreporation = 0,
                   DefaultTimePreporation = 0,
                   ActualAvailability = 0,
                   Time = 0,
                   Parallel = false,
               }) ;

            SaveChanges();
        }


        public void CreateCardVSMAsync()
        {
            
            CardVSM.Add(
              new CardVSM
              {
                 EtapNumeric = 1,
                 ProductionId = 1,
                 BufferVSM = BufferVSM.Find(1),
                 EtapVSM = EtapVSM.Find(1),
              });

            CardVSM.Add(
             new CardVSM
             {
                 EtapNumeric = 1,
                 ProductionId = 1,
                 BufferVSM = BufferVSM.Find(2),
                 EtapVSM = EtapVSM.Find(3),
             });

              CardVSM.Add(
                new CardVSM
                {
                    EtapNumeric = 2,
                    ProductionId = 1,
                    EtapVSM = EtapVSM.Find(2),
                });

                 CardVSM.Add(
                 new CardVSM
                 {
                     EtapNumeric = 3,
                     ProductionId = 1,
                     BufferVSM = BufferVSM.Find(3),
                     EtapVSM = EtapVSM.Find(4),
                 });

                 CardVSM.Add(
                 new CardVSM
                 {
                    EtapNumeric = 4,
                    ProductionId = 1,
                    EtapVSM = EtapVSM.Find(5),
                 });

            SaveChanges();
        }


    }
}
