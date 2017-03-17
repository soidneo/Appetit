using ECommerce.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace ECommerce.Clases
{
    public class UsuariosHelper:IDisposable
    {
        private static ECommerceContext db = new ECommerceContext();
        private static ApplicationDbContext usuarioContext = new ApplicationDbContext();

        public static void CheckRole(string roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(usuarioContext));
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }

        public static bool DeleteUser(string email, string roleName)
        {
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(usuarioContext));
            var userASP = userManager.FindByEmail(email);
            if (userASP == null)
            {
                return false;
            }
            var response = userManager.RemoveFromRole(userASP.Id,roleName);
            return response.Succeeded;
        }

        public static bool UpdateUserName(string currentUserName, string newUserName)
        {
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(usuarioContext));
            var userASP = userManager.FindByEmail(currentUserName);
            if (userASP == null)
            {
                return false;
            }
            userASP.UserName = newUserName;
            userASP.Email = newUserName;
            var response = userManager.Update(userASP);
            return response.Succeeded;
        }

        public static void CheckSuperUser()
        {
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(usuarioContext));
            var email = WebConfigurationManager.AppSettings["AdminUser"];
            var password = WebConfigurationManager.AppSettings["AdminPassword"];
            var userASP = userManager.FindByName(email);
            if (userASP == null)
            {
                CreateUserAsp(email, "SuperAdmin");
                return;
            }
            userManager.AddToRole(userASP.Id, "SuperAdmin");
            //userManager.AddToRole(userASP.Id, "User");
        }

        public static void CreateUserAsp(string email, string roleName)
        {
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(usuarioContext));
            var userASP = userManager.FindByEmail(email);
            if (userASP == null)
            {
                userASP = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                };
                userManager.Create(userASP, email);
            }
            userManager.AddToRole(userASP.Id, roleName);
        }

        public static async Task PasswordRecovery(string email)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(usuarioContext));
            var userASP = userManager.FindByEmail(email);
            if (userASP == null)
            {
                return;
            }

            var user = db.Usuarios.Where(tp => tp.UserName == email).FirstOrDefault();
            if (user == null)
            {
                return;
            }

            var random = new Random();
            var newPassword = string.Format("{0}{1}{2:04}*", 
                user.Nombre.ToUpper().Substring(0, 1), 
                user.Apellido.ToLower(), random.Next(9999));

            userManager.RemovePassword(userASP.Id);
            userManager.AddPassword(userASP.Id, newPassword);

            var subject = "Recuperar Contraseña";
            var body = string.Format(@"
            <h1>Resetear su contraseña</h1>
            <p>Su nueva contraseña es <strong>{0}</strong></p>
            <p>Por favor cambiela por una nueva</p>", newPassword);
            await MailHelper.SendMail(email, subject, body);
        }

        public void Dispose()
        {
            usuarioContext.Dispose();
            db.Dispose();
        }
    }

}
