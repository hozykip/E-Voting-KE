using E_Vote_System.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace E_Vote_System.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();
        public ActionResult Dashboard()
        {
            return RedirectToAction("Index");
        }
        public ActionResult Index()
        {
            try
            {

                var user = User;

                bool isAdmin = User.IsInRole("Administrator");
                bool isVoter = User.IsInRole("Voter");

            }catch(Exception e)
            {
                Utils.LogException(e);
            }
            return View();
        }

        public ActionResult ElectionSummaries()
        {

            //List<ElectionModel> elections = dbContext.ElectionModels.ToList();

            ViewBag.Active = StatsController.ActiveElection();
            ViewBag.Closed = StatsController.ClosedElection();
            ViewBag.Total = StatsController.TotalElection();
            ViewBag.Upcoming = StatsController.UpcomingElection();

            return PartialView("_ElectionSummaries");
        }

        [HttpPost]
        public async Task<ActionResult> CreateNotification(FormCollection form)
        {
            DefaultJsonResponse response = new DefaultJsonResponse
            {
                status = ResultCodes.FAIL,
                message = Configs.DefaultErrorMessage
            };

            try
            {

                string message = form["message"];
                string start = form["start"];
                string end = form["end"];
                string title = form["title"];

                DateTime startTime = DateTime.Parse(start);
                DateTime endTime = DateTime.Parse(end);

                if(message != null && start != null && end != null)
                {

                    ApplicationUser user = Utils.GetCurrentUser();

                    if(user != null)
                    {
                        NotificationModel notificationModel = new NotificationModel
                        {
                            Message = message,
                            StartDate = startTime,
                            EndDate = endTime,
                            DateCreated = DateTime.Now,
                            UserId = user.Id,
                            Title = title
                        };

                        dbContext.NotificationModels.Add(notificationModel);

                        await dbContext.SaveChangesAsync();

                        response.status = ResultCodes.SUCCESS;
                        response.message = "Notification created successfully";

                    }

                    

                }
                else
                {
                    response.message = "Please ensure that you have filled the form correctly";
                }


            }catch(Exception e)
            {
                Utils.LogException(e);
                response.message = e.Message;
            }

            return Json(response);
        }

        public ActionResult AdminNotificationsPartial()
        {
            List<NotificationModel> notifications = null;
            try
            {
                notifications = dbContext.NotificationModels.ToList();
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                ViewBag.Message = Utils.GenerateToastError(e.Message);
            }           

            return PartialView("_AdminNotificationsPartial", notifications);
        }
        
        public ActionResult VoterNotificationsListPartial()
        {
            List<NotificationModel> notifications = null;
            try
            {
                notifications = dbContext.NotificationModels.Where(n => n.StartDate <= DateTime.Now && n.EndDate >= DateTime.Now).ToList();
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                ViewBag.Message = Utils.GenerateToastError(e.Message);
            }           

            return PartialView("_VoterNotificationsListPartial", notifications);
        }

        public async Task<ActionResult> DeleteNotification(int id)
        {
            DefaultJsonResponse response = new DefaultJsonResponse
            {
                status = ResultCodes.FAIL,
                message = Configs.DefaultErrorMessage
            };

            if(id == null)
            {
                response.message = "Please ensure you select correct notification to delete";
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            try
            {

                NotificationModel notification = await dbContext.NotificationModels.FindAsync(id);

                if(notification == null)
                {
                    response.message = "Please ensure you select correct notification to delete. Notification not found.";
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                dbContext.NotificationModels.Remove(notification);
                await dbContext.SaveChangesAsync();

                response.status = ResultCodes.SUCCESS;
                response.message = "Notification deleted successfully";

            }catch(Exception e)
            {
                Utils.LogException(e);
                response.message = e.Message;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public ActionResult MyElectionsChart()
        {
            try
            {
                ViewBag.Total = StatsController.TotalElection();
                ViewBag.VotedIn = StatsController.CountElectionsVotedIn();

                ViewBag.NotVotedIn = ViewBag.Total - ViewBag.VotedIn;
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                ViewBag.Message = Utils.GenerateToastError(e.Message);
            }           

            return PartialView("_MyElectionsChart");
        }

        public ActionResult VotersChartPartial()
        {
            HashSet<VoterModel> registeredVoters = new HashSet<VoterModel>();
            List<VoterModel> all = null;
            try
            {
                List<ApplicationUser> users = dbContext.Users.ToList();

                all = dbContext.VoterModels.ToList();

                all.ForEach(u =>
                {
                    ApplicationUser user = users.FirstOrDefault(ui => ui.Email == u.EmailAddress);

                    if(user != null)
                    {
                        registeredVoters.Add(u);
                    }
                });

                int registeredCount = registeredVoters.Count;
                int allCount = all.Count;
                int unregisteredCount = allCount - registeredCount;


                ViewBag.RegisteredCount = registeredCount;
                ViewBag.AllCount = allCount;
                ViewBag.UnregisteredCount = unregisteredCount;


            }
            catch(Exception e)
            {
                Utils.LogException(e);
                ViewBag.Message = Utils.GenerateToastError(e.Message);
            }
            return PartialView("_VotersChartPartial");
        }

        public ActionResult AdminstratorsListPartial()
        {
            List<ApplicationUser> administrators = new List<ApplicationUser>();
            try
            {
                IdentityRole role = dbContext.Roles.Where(r => r.Name == "Administrator").FirstOrDefault();     
                
                if(role == null)
                {
                    ViewBag.Message = "Administraor role not found";
                    return PartialView("_AdminstratorsListPartial");
                }

                string roleId = role.Id;

                administrators = dbContext.Users.Where(u => u.Roles.Any(r => r.UserId == u.Id && r.RoleId == roleId)).ToList();

                
            }catch(Exception e)
            {
                Utils.LogException(e);
                ViewBag.Message = Utils.GenerateToastError(e.Message);
            }

            return PartialView("_AdminstratorsListPartial", administrators);
        }



        public ActionResult NavBar()
        {            
            return PartialView("_NavBar");
        }
        public ActionResult SideBarPartial()
        {            
            return PartialView("_SideBarPartial");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}