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
using System.IO;

namespace E_Vote_System.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CandidatesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Candidates
        public async Task<ActionResult> Index(int id)
        {
            try
            {
                var electionCandidateModels = db.ElectionCandidateModels.Where(y => y.PositionId == id).Include(e => e.ElectionPositionModel);

                ViewBag.PositionId = id;

                return View(await electionCandidateModels.ToListAsync());
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                return HttpNotFound();
            }
            
        }

        // GET: Candidates/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ElectionCandidateModel electionCandidateModel = await db.ElectionCandidateModels.FindAsync(id);
            if (electionCandidateModel == null)
            {
                return HttpNotFound();
            }
            return View(electionCandidateModel);
        }

        // GET: Candidates/Create
        public async Task<ActionResult> Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                ViewBag.Position = await db.ElectionPositionModels.FindAsync(id);
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(Configs.DefaultErrorMessage, "New Candidate");
            }

            
            return View();
        }

        // POST: Candidates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int id, [Bind(Include = "Id,PositionId,SurName,FirstName,OtherName,EmailAddress,PhoneNumber,ManifestoFile,ProfilePicture,DateCreated,DateModified,ManifestoFileUpload,ProfilePictureUpload")] ElectionCandidateModel electionCandidateModel, FormCollection form)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var positionId = form["Model.PositionId"];
                if (ModelState.IsValid)
                {

                    if(electionCandidateModel.ManifestoFileUpload.ContentLength > 0)
                    {
                        string extension = Path.GetExtension(electionCandidateModel.ManifestoFileUpload.FileName);
                        string guid = Guid.NewGuid().ToString();
                        string fileName = "Manifesto-"+ guid + extension;

                        electionCandidateModel.ManifestoFileUpload.SaveAs(Configs.DocumentsPath + fileName);

                        electionCandidateModel.ManifestoFile = fileName;
                    }

                    if(electionCandidateModel.ProfilePictureUpload.ContentLength > 0)
                    {
                        string extension = Path.GetExtension(electionCandidateModel.ProfilePictureUpload.FileName);
                        string guid = Guid.NewGuid().ToString();
                        string fileName = "Profile-"+ guid + extension;

                        electionCandidateModel.ProfilePictureUpload.SaveAs(Configs.DocumentsPath + fileName);

                        electionCandidateModel.ProfilePicture = fileName;
                    }

                    electionCandidateModel.DateCreated = DateTime.Now;
                    electionCandidateModel.PositionId = Convert.ToInt32(positionId);
                    db.ElectionCandidateModels.Add(electionCandidateModel);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { id = positionId });
                }

                /*ViewBag.PositionId = new SelectList(db.ElectionPositionModels, "Id", "Position", electionCandidateModel.PositionId);
                return View(electionCandidateModel);*/
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(Configs.DefaultErrorMessage, "New Candidate");
            }

            try
            {

                ViewBag.Position = await db.ElectionPositionModels.FindAsync(id);

            }catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(Configs.DefaultErrorMessage, "New Candidate");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(electionCandidateModel);
        }

        // GET: Candidates/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ElectionCandidateEditViewModel model = null;

            try
            {
                ElectionCandidateModel electionCandidateModel = await db.ElectionCandidateModels.FindAsync(id);
                if (electionCandidateModel == null)
                {
                    return HttpNotFound();
                }

                model = new ElectionCandidateEditViewModel
                {
                    Id = electionCandidateModel.Id,
                    PositionId = electionCandidateModel.PositionId,
                    FirstName = electionCandidateModel.FirstName,
                    SurName = electionCandidateModel.SurName,
                    OtherName = electionCandidateModel.OtherName,
                    EmailAddress = electionCandidateModel.EmailAddress,
                    ManifestoFile = electionCandidateModel.ManifestoFile,
                    ProfilePicture = electionCandidateModel.ProfilePicture,
                    PhoneNumber = electionCandidateModel.PhoneNumber
                };
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(Configs.DefaultErrorMessage,"Edit Candidate");
            }

            
            //ViewBag.PositionId = new SelectList(db.ElectionPositionModels, "Id", "Position", model.PositionId);
            return View(model);
        }

        // POST: Candidates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,PositionId,SurName,FirstName,OtherName,EmailAddress,PhoneNumber,ManifestoFile,ProfilePicture,DateCreated,DateModified")] ElectionCandidateEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ElectionCandidateModel electionCandidateModel = await db.ElectionCandidateModels.FindAsync(model.Id);
                    electionCandidateModel.SurName = model.SurName;
                    electionCandidateModel.FirstName = model.FirstName;
                    electionCandidateModel.OtherName = model.OtherName;
                    electionCandidateModel.PhoneNumber = model.PhoneNumber;
                    electionCandidateModel.EmailAddress = model.EmailAddress;
                    electionCandidateModel.ManifestoFile = model.ManifestoFile;
                    electionCandidateModel.ProfilePicture = model.ProfilePicture;

                    electionCandidateModel.DateModified = DateTime.Now;

                    db.Entry(electionCandidateModel).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { id = electionCandidateModel.PositionId });
                }
                else
                {
                    var errors = ModelState.Select(y => new { key = y.Key, errors = y.Value.Errors }).Where(y => y.errors.Count > 0).ToList();

                    TempData["Message"] = Utils.GenerateToastError(string.Join(",", errors), "Edit Candidate");
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(Configs.DefaultErrorMessage, "Edit Candidate");
            }           

            return View(model);
        }

        // GET: Candidates/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ElectionCandidateModel electionCandidateModel = await db.ElectionCandidateModels.FindAsync(id);
            if (electionCandidateModel == null)
            {
                return HttpNotFound();
            }
            return View(electionCandidateModel);
        }

        // POST: Candidates/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            DefaultJsonResponse response = new DefaultJsonResponse
            {
                status = ResultCodes.FAIL,
                message = Configs.DefaultErrorMessage
            };

            try
            {
                ElectionCandidateModel electionCandidateModel = await db.ElectionCandidateModels.FindAsync(id);
                db.ElectionCandidateModels.Remove(electionCandidateModel);
                await db.SaveChangesAsync();
                response.status = ResultCodes.SUCCESS;
                response.message = "Candidate deleted successfully";
            }
            catch(Exception e)
            {
                Utils.LogException(e);
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
