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
        public async Task<ActionResult> Create(int id, [Bind(Include = "Id,ElectionId,Position,DateCreated,DateModified")] ElectionPositionModel electionPositionModel)
        {
            if (ModelState.IsValid)
            {
                electionPositionModel.DateCreated = DateTime.Now;
                db.ElectionPositionModels.Add(electionPositionModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ElectionId = new SelectList(db.ElectionModels, "Id", "CreatedBy", electionPositionModel.ElectionId);
            return View(electionPositionModel);
        }

        // GET: ElectionPositionModels/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            ViewBag.ElectionId = new SelectList(db.ElectionModels, "Id", "CreatedBy", electionPositionModel.ElectionId);
            return View(electionPositionModel);
        }

        // POST: ElectionPositionModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ElectionId,Position,DateCreated,DateModified")] ElectionPositionModel electionPositionModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(electionPositionModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ElectionId = new SelectList(db.ElectionModels, "Id", "CreatedBy", electionPositionModel.ElectionId);
            return View(electionPositionModel);
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ElectionPositionModel electionPositionModel = await db.ElectionPositionModels.FindAsync(id);
            db.ElectionPositionModels.Remove(electionPositionModel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
