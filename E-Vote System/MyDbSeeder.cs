using E_Vote_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Vote_System
{
    public class MyDbSeeder
    {
        private ApplicationDbContext _context;

        public MyDbSeeder(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = (ApplicationDbContext)serviceProvider.GetService(typeof(ApplicationDbContext));

            Utils.LogDebug(context);

            if(context != null)
            {
                SeedAdminUserStatic(context);
            }
        }

        private static async void SeedAdminUserStatic(ApplicationDbContext _context)
        {

            try
            {

                var user = new ApplicationUser
                {
                    UserName = Configs.AdminUserEmail,
                    Email = Configs.AdminUserEmail,
                    FirstName = Configs.AdminUserFirstName,
                    LastName = Configs.AdminUserLastName,
                    Address = Configs.AdminUserAddress,
                    PhoneNumber = Configs.AdminUserPhoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                Utils.LogDebug(user);

                //var roleStore = new RoleStore<IdentityRole>(_context);

                //Utils.LogDebug(roleStore);


                if (_context.Roles.Any(r => r.Name == "Administrator"))
                {
                    if (!string.IsNullOrWhiteSpace(user.UserName))
                    {
                        if (!_context.Users.Any(u => u.UserName == user.UserName))
                        {
                            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));

                            Utils.LogDebug(userManager);

                            if (userManager != null)
                            {
                                var result = await userManager.CreateAsync(user, Configs.AdminUserPassword);

                                if (!result.Succeeded)
                                {
                                    string errors = "Failed to seed Administrator: " + string.Join(",", result.Errors);

                                    Utils.LogErrors(errors);
                                }
                                else
                                {
                                    var res = await userManager.AddToRoleAsync(user.Id, "Administrator");

                                    if (!res.Succeeded)
                                    {
                                        string errors = "Failed to Assign role to Administrator Account: " + string.Join(",", result.Errors);

                                        Utils.LogErrors(errors);
                                    }
                                }

                            }
                            else
                            {
                                string errors = "Failed to seed Administrator: No User Manager";

                                Utils.LogErrors(errors);
                            }

                        }

                    }
                }
                else
                {
                    string errors = "Failed to seed Administrator: No Admin Role";

                    Utils.LogErrors(errors);
                }



            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }


        }

        public async void SeedAdminUser()
        {

            try
            {

                var user = new ApplicationUser
                {
                    UserName = Configs.AdminUserEmail,
                    Email = Configs.AdminUserEmail,
                    FirstName = Configs.AdminUserFirstName,
                    LastName = Configs.AdminUserLastName,
                    Address = Configs.AdminUserAddress,
                    PhoneNumber = Configs.AdminUserPhoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                Utils.LogDebug(user);

                //var roleStore = new RoleStore<IdentityRole>(_context);

                //Utils.LogDebug(roleStore);


                if (_context.Roles.Any(r => r.Name == "Administrator"))
                {
                    if (!string.IsNullOrWhiteSpace(user.UserName))
                    {
                        if (!_context.Users.Any(u => u.UserName == user.UserName))
                        {
                            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));

                            Utils.LogDebug(userManager);

                            if (userManager != null)
                            {
                                var result = await userManager.CreateAsync(user, Configs.AdminUserPassword);

                                if (!result.Succeeded)
                                {
                                    string errors = "Failed to seed Administrator: " + string.Join(",", result.Errors);

                                    Utils.LogErrors(errors);
                                }
                                else
                                {
                                    var res = await userManager.AddToRoleAsync(user.Id, "Administrator");

                                    if (!res.Succeeded)
                                    {
                                        string errors = "Failed to Assign role to Administrator Account: " + string.Join(",", result.Errors);

                                        Utils.LogErrors(errors);
                                    }
                                }

                            }
                            else
                            {
                                string errors = "Failed to seed Administrator: No User Manager";

                                Utils.LogErrors(errors);
                            }

                        }

                    }
                }
                else
                {
                    string errors = "Failed to seed Administrator: No Admin Role";

                    Utils.LogErrors(errors);
                }

                

            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }          


        }
    }
}