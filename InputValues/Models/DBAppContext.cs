using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CooverBoxWebApplication.Models;
using CooverBoxWebApplication.Models.WorkRecord;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using CooverBoxWebApplication.Models.Boxes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CooverBoxWebApplication.Models
{
    //база данных
    public class DBAppContext : IdentityDbContext<User>
    {
        public DbSet<DesignPaper> DesignPapers { get; set; }
        public DbSet<CoverCarton> CoverCartons { get; set; }
        public DbSet<Ribbon> Ribbons { get; set; }
        public DbSet<Isolon> Isolons { get; set; }
        public DbSet<ArtHay> ArtHays { get; set; }
        public DbSet<FringePaper> FringePapers { get; set; }
        public DbSet<TissuePaper> TissuePapers { get; set; }
        public DbSet<Knife> Knifes { get; set; }
        public DbSet<KnifeType> KnifeTypes { get; set; }
        public DbSet<Magnet> Magnets { get; set; }
        public DbSet<Rubber> Rubbers { get; set; }
        public DbSet<MDF> MDFs { get; set; }
        public DbSet<Cloth> Cloths { get; set; }
        public DbSet<Grommet> Grommets { get; set; }
        public DbSet<Cord> Cords { get; set; }


        public DbSet<BoxDBData> BoxesSaved { get; set; }
        public DbSet<BoxDBDataTest> BoxesTestSaved { get; set; }


        public DbSet<BoxOrder> BoxOrders { get; set; }
        public DbSet<CalendarOff> Calendars { get; set; }
        public DbSet<DateTaskBox> DateTaskBoxes { get; set; }
        public DbSet<TaskBox> TaskBoxes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UsersAndDepartnments> UsersAndDepartnments {get;set;}
        public DbSet<TaskBoxGroup> TaskBoxGroups { get;set; }

        public DbSet<BitrixUser> BitrixUsers { get; set; }

        private readonly IWebHostEnvironment _webHostEnvironment;
        public DBAppContext(IWebHostEnvironment webHostEnvironment, DbContextOptions<DBAppContext> options)
        : base(options)
        {
            _webHostEnvironment = webHostEnvironment;
            Database.EnsureCreated();
        }
        public BaseBox GetDBBox(string Id)
        {
            { BaseBox model = BaseBox.Box(Id); if (model != null) { model.Id = Id; return model; } }
            if (File.Exists($"{_webHostEnvironment.WebRootPath}\\SessionFile\\{Id}.json"))
            {
                StreamReader file = File.OpenText($"{_webHostEnvironment.WebRootPath}\\SessionFile\\{Id}.json");
                string type = file.ReadLine();
                string json = file.ReadToEnd();
                file.Close();
                if (string.IsNullOrEmpty(json))
                    return null;
                return type switch
                {
                    "TopBotton" => JsonConvert.DeserializeObject<TopBotton>(json),
                    "BookValve" => JsonConvert.DeserializeObject<BookValve>(json),
                    "OptimusPride" => JsonConvert.DeserializeObject<OptimusPride>(json),
                    "BookWithString" => JsonConvert.DeserializeObject<BookWithString>(json),
                    "Casket" => JsonConvert.DeserializeObject<Casket>(json),
                    "SliderMDF" => JsonConvert.DeserializeObject<SliderMDF>(json),
                    "Cub" => JsonConvert.DeserializeObject<Cub>(json),
                    "Slider" => JsonConvert.DeserializeObject<Slider>(json),
                    "Bumblebee" => JsonConvert.DeserializeObject<Bumblebee>(json),
                    "CraftTopBottonOptimus" => JsonConvert.DeserializeObject<CraftTopBottonOptimus>(json),
                    "BookValveMDF" => JsonConvert.DeserializeObject<BookValveMDF>(json),
                    "BookWithStringMDF" => JsonConvert.DeserializeObject<BookWithStringMDF>(json),
                    "CircleTopBotton" => JsonConvert.DeserializeObject<CircleTopBotton>(json),
                    "Mercedes" => JsonConvert.DeserializeObject<Mercedes>(json),
                    "CraftWithEars" => JsonConvert.DeserializeObject<CraftWithEars>(json),
                    "GofraTopBottonOptimus" => JsonConvert.DeserializeObject<GofraTopBottonOptimus>(json),
                    "GofraWithEars" => JsonConvert.DeserializeObject<GofraWithEars>(json),
                    "CatHouse" => JsonConvert.DeserializeObject<CatHouse>(json),
                    _ => null,
                };
            }
            return null;
        }

        public void SaveDBBox(BaseBox box)
        {
            StreamWriter file = File.CreateText($"{_webHostEnvironment.WebRootPath}\\SessionFile\\{box.Id}.json");
            file.WriteLine(box.GetType().Name);
            file.WriteLine(JsonConvert.SerializeObject(box));
            file.Close();
        }
    }

}
