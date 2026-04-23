using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Core.Models;
using Krugozor.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.SeedData
{
  public class DataContextSeed
  {

    public static async Task SeedDataAsync(AppDbContext context, ILoggerFactory loggerFactory)
    {
      try
      {

        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // /// <summary>
        // /// seeding countrys
        // /// </summary>
        // /// <returns></returns>
        // if (!context.Countrys.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Seed/SeedData/countrys.json");
        //   var items = JsonSerializer.Deserialize<List<Country>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.Countrys.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }

        // /// <summary>
        // /// seeding citys
        // /// </summary>
        // /// <returns></returns>
        // if (!context.City.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Seed/SeedData/citys.json");
        //   var items = JsonSerializer.Deserialize<List<City>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.City.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }

        // /// <summary>
        // /// adding investors
        // /// </summary>
        // /// <returns></returns>
        // if (!context.Investors.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Seed/SeedData/investors.json");
        //   var items = JsonSerializer.Deserialize<List<Investor>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.Investors.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }

        // /// <summary>
        // /// seeding amenities
        // /// </summary>
        // /// <returns></returns>
        // if (!context.Amenities.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Seed/SeedData/amenities.json");
        //   var items = JsonSerializer.Deserialize<List<Amenitie>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.Amenities.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }

        // /// <summary>
        // /// proposal types seed
        // /// </summary>
        // /// <returns></returns>
        // if (!context.ProposalTypes.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Seed/SeedData/types.json");
        //   var items = JsonSerializer.Deserialize<List<ProposalType>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.ProposalTypes.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }


        // /// <summary>
        // /// proposal description translated seeed
        // /// </summary>
        // /// <returns></returns>
        // if (!context.ProposalDescriptionTranaltaions.Any())
        // {
        //   var itemsData = File.ReadAllText(path + @"/Seed/SeedData/proposal-descriptions.json");
        //   var items = JsonSerializer.Deserialize<List<ProposalDescriptionTranaltaion>>(itemsData);
        //   foreach (var item in items)
        //   {
        //     context.ProposalDescriptionTranaltaions.Add(item);
        //   }
        //   await context.SaveChangesAsync();
        // }



        /// <summary>
        /// seeding actions for history log
        /// </summary>
        /// <returns></returns>
        // if (!context.ProposalActions.Any())
        // {
        //   // var itemsData = File.ReadAllText(path + @"/Seed/SeedData/proposals.json");
        //   // var items = JsonSerializer.Deserialize<List<Proposal>>(itemsData);

        //   var action1 = new ProposalAction()
        //   {
        //     ActionName = "Объект готов к оформлению и покупке",
        //     ActionDescription = "Идет оформление сделки риэлтором",
        //     EnrolledDate = DateTime.Now.AddDays(42).AddHours(11).AddMinutes(55),
        //     ProfileId = 1
        //   };
        //   var action2 = new ProposalAction()
        //   {
        //     ActionName = "Пользователь Matew Green готов вложиться",
        //     ActionDescription = "Предполагается к вложению $40000",
        //     EnrolledDate = DateTime.Now.AddDays(40).AddHours(5).AddMinutes(3),
        //     ProfileId = 1
        //   };
        //   var action3 = new ProposalAction()
        //   {
        //     ActionName = "Пользователь Andrey Kamenev увеличил долю",
        //     ActionDescription = "Предполагается к вложению $40000",
        //     EnrolledDate = DateTime.Now.AddDays(33).AddHours(22).AddMinutes(31),
        //     ProfileId = 1
        //   };
        //   var action4 = new ProposalAction()
        //   {
        //     ActionName = "Пользователь Elen Black готов вложиться",
        //     ActionDescription = "Предполагается к вложению $20000",
        //     EnrolledDate = DateTime.Now.AddDays(24).AddHours(22).AddMinutes(12),
        //     ProfileId = 1
        //   };
        //   var action5 = new ProposalAction()
        //   {
        //     ActionName = "Пользователь Andrey Kamenev готов вложиться",
        //     ActionDescription = "Предполагается к вложению $20000",
        //     EnrolledDate = DateTime.Now.AddDays(30).AddHours(13).AddMinutes(41),
        //     ProfileId = 1
        //   };
        //   var action6 = new ProposalAction()
        //   {
        //     ActionName = "Объект доступен для инвестирования",
        //     ActionDescription = "Осталось для покупки 100%",
        //     EnrolledDate = DateTime.Now.AddDays(11).AddHours(10).AddMinutes(12),
        //     ProfileId = 1
        //   };

        //   var items = new List<ProposalAction>
        //   {
        //     action1,
        //     action2,
        //     action3,
        //     action4,
        //     action5,
        //     action6
        //   };


        //   foreach (var item in items)
        //   {
        //     context.ProposalActions.Add(item);
        //   };

        //   await context.SaveChangesAsync();
        // }

        /// <summary>
        /// seedeing orders from investors
        /// </summary>
        /// <returns></returns>
        var requestStatuses = await context.RequestStatuses.ToArrayAsync();
        // if (requestStatuses.Length == 0)
        if (requestStatuses.Length == 0)
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/request-statuses.json");
          var items = JsonSerializer.Deserialize<List<RequestStatus>>(itemsData);
          foreach (var item in items)
          {
            context.RequestStatuses.Add(item);
          }
          await context.SaveChangesAsync();
        }


        /// <summary>
        /// seedeing orders from investors
        /// </summary>
        /// <returns></returns>
        var proposalStatuses = await context.ProposalProfileStatuses.ToArrayAsync();
        // if (requestStatuses.Length == 0)
        if (requestStatuses.Length == 0)
        {
          var itemsData = File.ReadAllText(path + @"/Seed/SeedData/proposal-statuses.json");
          var items = JsonSerializer.Deserialize<List<ProposalProfileStatus>>(itemsData);
          foreach (var item in items)
          {
            context.ProposalProfileStatuses.Add(item);
          }
          await context.SaveChangesAsync();
        }


      }
      catch (Exception ex)
      {
        var logger = loggerFactory.CreateLogger<DataContextSeed>();
        logger.LogError(ex.Message);
      }
    }


  }
}