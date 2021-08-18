using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_Vote_System.Models;
using E_Vote_System.Models.ViewModels;

namespace E_Vote_System.Controllers
{
    [Authorize(Roles = "Voter")]
    public class VoterElectionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VoterElections
        public async Task<ActionResult> Index()
        {
            try
            {
                VoterModel voter = Utils.GetCurrentVoter();
                if (voter == null)
                {
                    TempData["Message"] = Utils.GenerateToastError("Voter details not found");
                    return View();
                }

                var electionModels = db.ElectionModels.Include(e => e.Creator).Include(e => e.Type).Where(e => e.VoterCategoryId == voter.VoterCategoryModelId);
                return View(await electionModels.ToListAsync());
            }
            catch(Exception e)
            {
                Utils.LogException(e);

                TempData["Message"] = Utils.GenerateToastError();
            }

            return View();
            
        }
        public async Task<ActionResult> Active()
        {
            var electionModels = db.ElectionModels.Where(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now).Include(e => e.Creator).Include(e => e.Type);
            return View(await electionModels.ToListAsync());
        }
        public async Task<ActionResult> Closed()
        {
            var electionModels = db.ElectionModels.Where(e => e.EndDate < DateTime.Now).Include(e => e.Creator).Include(e => e.Type);
            return View(await electionModels.ToListAsync());
        }

        // GET: VoterElections/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ElectionModel electionModel = await db.ElectionModels.FindAsync(id);
            if (electionModel == null)
            {
                return HttpNotFound();
            }
            return View(electionModel);
        }

        
        public async Task<ActionResult> Cast(int id)
        {

            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {

                ElectionModel election = await db.ElectionModels.Include(e => e.Creator).Include(e => e.Type).Include(e => e.ElectionPositionModels).FirstOrDefaultAsync(e => e.Id == id);

                if(election == null)
                {
                    TempData["Message"] = Utils.GenerateToastError("Election not found");
                    return View();
                }

                return View(election);

            }catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError();
            }

            return View();
        }

        public ActionResult VoterPositionsPartial(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<ElectionPositionModel> positions = null;

            try
            {
                ApplicationUser user = Utils.GetCurrentUser();

                if(user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                positions = db.ElectionPositionModels.Where(p => p.ElectionId == id).Include(p => p.ElectionCandidateModels).ToList();

                List<VoteModel> votes = db.VoteModels.Where(v => v.Candidate.ElectionPositionModel.ElectionId == id && v.VoterId == user.Id).ToList();

                ViewBag.Votes = votes;

            }catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError();
            }

            return PartialView("_VoterPositionsPartial", positions);
        }

        [HttpPost]
        public async Task<ActionResult> Vote(CastVoteViewModel model)
        {
            DefaultJsonResponse response = new DefaultJsonResponse
            {
                status = ResultCodes.FAIL,
                message = Configs.DefaultErrorMessage
            };

            if (ModelState.IsValid)
            {

                ApplicationUser user = Utils.GetCurrentUser();


                if(user == null)
                {
                    response.message = "User not found. Please reload the page to check your session";
                    return Json(response);
                }

                ElectionCandidateModel candidateModel = await db.ElectionCandidateModels.Include(c => c.ElectionPositionModel).FirstOrDefaultAsync(c => c.Id == model.CandidateId);

                if(candidateModel == null)
                {
                    response.message = "Candidate not found";
                    return Json(response);
                }

                int positionId = candidateModel.PositionId;

                bool voted = false;

                voted = await db.VoteModels.AnyAsync(v => v.Candidate.PositionId == positionId && v.VoterId == user.Id);

                if (voted)
                {
                    response.message = "You have already submitted your vote for this position";
                    return Json(response);
                }
                

                VoteModel vote = new VoteModel
                {
                    CandidateId = model.CandidateId,
                    VoterId = user.Id,
                    DateCreated = DateTime.Now
                };

                if(vote.CandidateId == 0)
                {
                    response.message = "Candidate vote submission error";
                    return Json(response);
                }

                db.VoteModels.Add(vote);

                await db.SaveChangesAsync();

                response.status = ResultCodes.SUCCESS;
                response.message = "Vote cast successfully";
            }
            else
            {
                response.message = "Check your form.";
            }

            return Json(response);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
