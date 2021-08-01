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
    public class ElectionPositionModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ElectionPositionModels
        public async Task<ActionResult> Index(int id)
        {
            try
            {
                var electionPositionModels = db.ElectionPositionModels.Where(y => y.ElectionId == id).Include(e => e.Election);

                ViewBag.ElectionId = id;

                return View(await electionPositionModels.ToListAsync());
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }

            return HttpNotFound();
        }

        // GET: ElectionPositionModels/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ElectionPositionModel electionPositionModel = await db.ElectionPositionModels.FindAsync(id);

            ViewBag.ElectionId = id;

            if (electionPositionModel == null)
            {
                return HttpNotFound();
            }
            return View(electionPositionModel);
        }

        // GET: ElectionPositionModels/Create
        public async Task<ActionResult> Create(int? id)
        {

            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {            
                ViewBag.Election = await db.ElectionModels.FindAsync(id);

            }catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(Configs.DefaultErrorMessage, "New Position");
            }

            return View();
        }

        // POST: ElectionPositionModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int id, [Bind(Include = "Id,ElectionId,Position,DateCreated,DateModified")] ElectionPositionModel electionPositionModel, FormCollection form)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {

                var electionId = form["Model.ElectionId"];

                if (ModelState.IsValid)
                {
                    electionPositionModel.DateCreated = DateTime.Now;
                    electionPositionModel.ElectionId = Convert.ToInt32(electionId);
                    db.ElectionPositionModels.Add(electionPositionModel);
                    await db.SaveChangesAsync();
                    TempData["Message"] = Utils.GenerateToastSuccess("New Position added successfully", "New Position");
                    return RedirectToAction("Index", new { id = electionId });
                }

            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(Configs.DefaultErrorMessage, "New Position");
            }

            try
            {

                ViewBag.Election = await db.ElectionModels.FindAsync(id);

            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(Configs.DefaultErrorMessage, "New Position");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            

            return View(electionPositionModel);
        }

        // GET: ElectionPositionModels/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ElectionPositionEditViewModel model = null;

            try
            {

                ElectionPositionModel electionPositionModel = await db.ElectionPositionModels.FindAsync(id);
                if (electionPositionModel == null)
                {
                    return HttpNotFound();
                }

                model = new ElectionPositionEditViewModel
                {
                    Id = electionPositionModel.Id,
                    ElectionId = electionPositionModel.ElectionId,
                    Position = electionPositionModel.Position
                };



            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(Configs.DefaultErrorMessage);
            }

            return View(model);
        }

        // POST: ElectionPositionModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ElectionId,Position,DateCreated,DateModified")] ElectionPositionEditViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ElectionPositionModel electionPositionModel = await db.ElectionPositionModels.FindAsync(model.Id);
                    electionPositionModel.Position = model.Position;
                    electionPositionModel.DateModified = DateTime.Now;
                    db.Entry(electionPositionModel).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["Message"] = Utils.GenerateToastSuccess("Position updated successfully", "Edit Position");
                    return RedirectToAction("Index", new { id = electionPositionModel.ElectionId });
                }
                else
                {
                    var errors = ModelState.Select(y => new { key = y.Key, errors = y.Value.Errors }).Where(y => y.errors.Count > 0).ToList();

                    TempData["Message"] = Utils.GenerateToastError(string.Join(",", errors), "Edit Position");
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(Configs.DefaultErrorMessage, "Edit Position");
            }

            
            return View(model);
        }

        // GET: ElectionPositionModels/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ElectionPositionModel electionPositionModel = await db.ElectionPositionModels.FindAsync(id);
            if (electionPositionModel == null)
            {
                return HttpNotFound();
            }
            return View(electionPositionModel);
        }

        // POST: ElectionPositionModels/Delete/5
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
                ElectionPositionModel electionPositionModel = await db.ElectionPositionModels.FindAsync(id);
                db.ElectionPositionModels.Remove(electionPositionModel);
                await db.SaveChangesAsync();

                response.status = ResultCodes.SUCCESS;
                response.message = "Position deleted successfully";
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
