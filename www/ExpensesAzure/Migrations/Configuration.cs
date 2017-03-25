namespace SocialApps.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SocialApps.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<SocialApps.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            //  https://www.evernote.com/shard/s132/nl/14501366/467de8f5-7d84-4c65-bec9-1b85f10ef1b5
            //AutomaticMigrationsEnabled = true;
            //AutomaticMigrationDataLossAllowed = true; 
        }

        void CreateRoles(SocialApps.Models.ApplicationDbContext context)
        {
            IdentityResult ir;
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            ir = rm.Create(new IdentityRole("User"));

            rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            ir = rm.Create(new IdentityRole("Administrator"));
        }

        public static bool AddUser(SocialApps.Models.ApplicationDbContext context, string userName, string email, string password, string role)
        {
            IdentityResult ir;
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            
            var user = new ApplicationUser()
            {
                UserName = userName,
                Email = email
            };
            ir = um.Create(user, password);
            if (ir.Succeeded == false)
                return ir.Succeeded;

            ir = um.AddToRole(user.Id, role);
            return ir.Succeeded;
        }

        protected override void Seed(SocialApps.Models.ApplicationDbContext context)
        {
            CreateRoles(context);
            AddUser(context, "user@mail.ru", "user@mail.ru", "123abc!ABC", "User");
            AddUser(context, "administrator@mail.ru", "administrator@mail.ru", "dmitry!MOROZOV", "Administrator");
        }
    }
}
