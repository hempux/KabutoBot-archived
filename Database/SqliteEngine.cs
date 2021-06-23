 using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.Bot.Schema;
using System.Threading.Tasks;
using net.hempux.ninjawebhook.Models;

namespace net.hempux.kabuto.database
{

public class SqliteEngine {



private readonly TeamsBotDbContext db;
public SqliteEngine(){
    db = new TeamsBotDbContext();
    db.Database.EnsureCreated();
    
}
        /*
         public void Insert(string _id, string _text){

            db.Add(new ActivityModel {Id = _id,
                                     Text = _text,
                                     Attachment = ""
                                     });
            db.SaveChanges();
         }
         public void Insert(OrganizationModel organization){

            db.Add(organization);
            db.SaveChanges();
         }
        */
        public void Insert(ActivityModel activity)
        {

            db.Add(activity);
            db.SaveChanges();
        }
        public void Insert(OrganizationModel organization){

    db.Add(organization);
    db.SaveChanges();
 }

public ActivityModel Read(){

            var activity = db.Activities
               .OrderBy(b => b.Id)
               .FirstOrDefault();

            return activity;
}

        public DashboardActivityModel[] ReadAllActiveDashboardIssues()
        {

            var activeIssuesArray = db.DashboardIssues.
                Where(a => a.isActiveIssue).ToArray();

            return activeIssuesArray;
        }
        public void Update(ActivityModel _activityModel){


             

}

    public void Delete(ActivityModel _activityModel){


                db.Remove(_activityModel);
                db.SaveChanges();
    }

        public void createDashboardIssue(DetailedActivity detailedActivity)
        {
            DashboardActivityModel dashboardissue = new DashboardActivityModel
            {
                HostName = detailedActivity.DeviceId.ToString(),
                Id = detailedActivity.Id.ToString(),
                isActiveIssue = true,
                Organization = "org",
                Text = detailedActivity.Status
            };

            db.Add(dashboardissue);
            db.SaveChangesAsync();
            
            
        }
    }
}