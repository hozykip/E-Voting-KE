using E_Vote_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;


namespace E_Vote_System.Controllers
{
    [Authorize]
    public class StatsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static ApplicationDbContext dbContenxt = new ApplicationDbContext();

        public static int TotalElection()
        {
            int total= 0;

            try
            {
                total = dbContenxt.ElectionModels.ToList().Count;

            }catch(Exception e)
            {
                Utils.LogException(e);
            }

            return total;
        }
        public static int ActiveElection()
        {
            int total= 0;

            try
            {
                total = dbContenxt.ElectionModels.Where(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now).ToList().Count;

            }catch(Exception e)
            {
                Utils.LogException(e);
            }

            return total;
        }
        public static int UpcomingElection()
        {
            int total= 0;

            try
            {
                total = dbContenxt.ElectionModels.Where(e => e.StartDate > DateTime.Now).ToList().Count;

            }catch(Exception e)
            {
                Utils.LogException(e);
            }

            return total;
        }
        public static int CountElectionsVotedIn()
        {
            int total= 0;

            try
            {
                ApplicationUser user = Utils.GetCurrentUser();

                if(user == null)
                {
                    return 0;
                }

                List<VoteModel> votes = dbContenxt.VoteModels.Where(v => v.VoterId == user.Id && v.CandidateId != null && v.Candidate.PositionId != null).Include(v => v.Candidate.ElectionPositionModel.Election).ToList();

                HashSet<ElectionModel> elections = new HashSet<ElectionModel>();

                votes.ForEach(v =>
                {
                    elections.Add(v.Candidate.ElectionPositionModel.Election);
                });

                total = elections.ToList().Count;

            }catch(Exception e)
            {
                Utils.LogException(e);
            }

            return total;
        }
        
        public static int ClosedElection()
        {
            int total= 0;

            try
            {
                total = dbContenxt.ElectionModels.Where(e => e.EndDate < DateTime.Now).ToList().Count;

            }catch(Exception e)
            {
                Utils.LogException(e);
            }

            return total;
        }
        public async Task<ActionResult> Election(int id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            ElectionModel election = null;

            try
            {
                election = await db.ElectionModels.Include(e => e.Creator).Include(e => e.Type).Include(e => e.ElectionPositionModels).FirstOrDefaultAsync(e => e.Id == id);

            }
            catch (Exception e)
            {
                TempData["Message"] = Utils.GenerateToastError();
                Utils.LogException(e);
            }

            return View(election);
        }

        public ActionResult PositionDetailsPartial(int position_id)
        {
            if (position_id == null)
            {
                ViewBag.Message = "Please specify a position id";
                return View("_PositionDetailsPartial");
            }

            ElectionPositionModel position = null;

            try
            {

                position = db.ElectionPositionModels.Find(position_id);

                if (position != null)
                {

                    List<ElectionCandidateModel> candidates = db.ElectionCandidateModels.Include(c => c.Votes).Where(c => c.PositionId == position_id).ToList();

                    ViewBag.Candidates = candidates;

                }
                else
                {
                    ViewBag.Message = "Position not found";
                }

            }
            catch (Exception e)
            {
                Utils.LogException(e);
                ViewBag.Message = Configs.DefaultErrorMessage;
            }

            return PartialView("_PositionDetailsPartial", position);
        }
        public ActionResult PositionChartPartial(int position_id)
        {
            if (position_id == null)
            {
                ViewBag.Message = "Please specify a position id";
                return View("_PositionChartPartial");
            }

            ElectionPositionModel position = null;

            try
            {

                position = db.ElectionPositionModels.Find(position_id);

                if (position != null)
                {

                    List<ElectionCandidateModel> candidates = db.ElectionCandidateModels.Include(c => c.Votes).Where(c => c.PositionId == position_id).ToList();

                    ViewBag.Candidates = candidates;

                }
                else
                {
                    ViewBag.Message = "Position not found";
                }

            }
            catch (Exception e)
            {
                Utils.LogException(e);
                ViewBag.Message = Configs.DefaultErrorMessage;
            }

            return PartialView("_PositionChartPartial", position);
        }
        public ActionResult CandidateVotesPartial(int position_id)
        {
            if (position_id == null)
            {
                ViewBag.Message = "Please specify a position id";
                return View("_CandidateVotesPartial");
            }

            ElectionPositionModel position = null;

            try
            {

                position = db.ElectionPositionModels.Find(position_id);

                if (position != null)
                {

                    List<ElectionCandidateModel> candidates = db.ElectionCandidateModels.Include(c => c.Votes).Where(c => c.PositionId == position_id).ToList();

                    ViewBag.Candidates = candidates;

                }
                else
                {
                    ViewBag.Message = "Position not found";
                }

            }
            catch (Exception e)
            {
                Utils.LogException(e);
                ViewBag.Message = Configs.DefaultErrorMessage;
            }

            return PartialView("_CandidateVotesPartial", position);
        }

        public async Task<ActionResult> CandidateVotes(int id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            List<VoteModel> votes = null;

            try
            {
                ElectionCandidateModel candidate = await db.ElectionCandidateModels.Include(c => c.ElectionPositionModel).FirstOrDefaultAsync(c => c.Id == id);


                if(candidate != null)
                {
                    votes = await db.VoteModels.Include(v => v.Candidate).Include(v => v.Voter).Where(v => v.CandidateId == id).ToListAsync();
                }

                ViewBag.Candidate = candidate;
                

            }catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError();
            }

            return View(votes);
        }

    }
}