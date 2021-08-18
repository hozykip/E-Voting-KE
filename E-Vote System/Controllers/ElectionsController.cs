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
    [Authorize(Roles = "Administrator")]
    public class ElectionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Elections
        public async Task<ActionResult> Index()
        {
            var electionModels = db.ElectionModels.Include(e => e.Creator).Include(e => e.Type).Include(e => e.VoterCategory);
            return View(await electionModels.ToListAsync());
        }

        // GET: Elections/Details/5
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

        // GET: Elections/Create
        public ActionResult Create()
        {
            ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TypeId = new SelectList(db.ElectionTypes, "Id", "Type");
            ViewBag.CategoryId = new SelectList(db.VoterCategoryModels, "Id", "Name");
            return View();
        }

        // POST: Elections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,TypeId,VoterCategoryId,CreatedBy,Name,StartDate,EndDate,DateCreated,DateModified")] ElectionModel electionModel)
        {           

            try
            {
                var user = Utils.GetCurrentUser();                

                if (ModelState.IsValid)
                {
                    electionModel.CreatedBy = user.Id;
                    electionModel.DateCreated = DateTime.Now;

                    db.ElectionModels.Add(electionModel);
                    await db.SaveChangesAsync();
                    TempData["Message"] = Utils.GenerateToastSuccess($"Election created successfully", "New Election");
                    return RedirectToAction("Index");
                }
                else
                {
                    var errors = ModelState.Select(y => new { key = y.Key, errors = y.Value.Errors }).Where(y => y.errors.Count > 0).ToList();
                    TempData["Message"] = Utils.GenerateToastError($"Errors: " + string.Join(",",errors), "New Election");
                }

                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", electionModel.CreatedBy);
                ViewBag.TypeId = new SelectList(db.ElectionTypes, "Id", "Type", electionModel.TypeId);
                ViewBag.CategoryId = new SelectList(db.VoterCategoryModels, "Id", "Name");
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError($"Election not created", "New Election");
            }
            
            return View(electionModel);
        }

        // GET: Elections/Edit/5
        public async Task<ActionResult> Edit(int? id)
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

            ElectionEditViewModel model = null;

            try
            {

                model = new ElectionEditViewModel
                {
                    StartDate = electionModel.StartDate,
                    EndDate = electionModel.EndDate,
                    Name = electionModel.Name,
                    Type = electionModel.Type,
                    TypeId = electionModel.TypeId,
                    Id = electionModel.Id,
                    VoterCategoryId = electionModel.VoterCategoryId
                };

                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", electionModel.CreatedBy);
                ViewBag.TypeId = new SelectList(db.ElectionTypes, "Id", "Type", electionModel.TypeId);
                ViewBag.CategoryId = new SelectList(db.VoterCategoryModels, "Id", "Name");

            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError($"Election not found");
            }            
            
            return View(model);
        }

        // POST: Elections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,TypeId,CreatedBy,Name,StartDate,EndDate,DateCreated,DateModified,VoterCategoryId")] ElectionEditViewModel model, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string startDate = form["sDate"];
                    string endtDate = form["eDate"];

                    DateTime Start = DateTime.Parse(startDate);
                    DateTime End = DateTime.Parse(endtDate);

                    ElectionModel electionModel = await db.ElectionModels.FindAsync(model.Id);
                    electionModel.DateModified = DateTime.Now;
                    electionModel.Name = model.Name;
                    electionModel.StartDate = Start;
                    electionModel.EndDate = End;
                    electionModel.TypeId = model.TypeId;
                    electionModel.VoterCategoryId = model.VoterCategoryId;

                    db.Entry(electionModel).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["Message"] = Utils.GenerateToastSuccess($"Election updated successfully", "Edit Election");
                    return RedirectToAction("Index");
                }
                
            }
            catch(Exception e)
            {
                TempData["Message"] = Utils.GenerateToastError($"Election not updated", "Edit Election");
                Utils.LogException(e);
            }

            try
            {
                ViewBag.CreatedBy = new SelectList(db.Users, "Id", "FirstName", model.CreatedBy);
                ViewBag.TypeId = new SelectList(db.ElectionTypes, "Id", "Type", model.TypeId);
                ViewBag.CategoryId = new SelectList(db.VoterCategoryModels, "Id", "Name");
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError($"Election not updated", "Edit Election");
                return RedirectToAction("Edit", new { id = model.Id });
            }

            return View(model);
        }

        // GET: Elections/Delete/5
        public async Task<ActionResult> Delete(int? id)
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

        // POST: Elections/Delete/5
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
                ElectionModel electionModel = await db.ElectionModels.FindAsync(id);
                db.ElectionModels.Remove(electionModel);
                await db.SaveChangesAsync();

                response.status = ResultCodes.SUCCESS;
                response.message = "Election deleted successfully";
            }
            catch (Exception e)
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
