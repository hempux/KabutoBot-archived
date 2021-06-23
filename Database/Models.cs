using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using net.hempux.Utilities;

namespace net.hempux.kabuto.database
{
    public class TeamsBotDbContext : DbContext
    {
       

        public DbSet<ActivityModel> Activities { get; set; }
        //public DbSet<Post> Posts { get; set; }
        public DbSet<OrganizationModel> Organizations { get; set; }
        public DbSet<DashboardActivityModel> DashboardIssues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={AppSettings.Current.SqliteDatabase}");
    }
    [Table("Activities")]
      public class ActivityModel
      {
          public string Id { get; set; }
          public string Text { get; set; }
        public string Attachment { get; set; }
    }

    [Table("DashboardActivities")]
    public class DashboardActivityModel
    {
        // Node hostname
        public string HostName { get; set; }
        // Organization that the Node belongs to
        public string Organization { get; set; }
        // Node ID (in NinjaRMM)
        public string Id { get; set; }
        // Text output from NinjaRMM status
        public string Text { get; set; }
        // Bool value to notify if the issue is active or not to tell the dashboard to add/remove it.
        public bool isActiveIssue { get; set; }
    }

    [Table("Organizations")]
    public class OrganizationModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    [Table("Devices")]
    public class DeviceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }

}