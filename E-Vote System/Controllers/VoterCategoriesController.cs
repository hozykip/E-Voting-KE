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
    public class VoterCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: VoterCategories
        public async Task<ActionResult> Index()
        {
            return View(await db.VoterCategoryModels.ToListAsync());
        }

        // GET: VoterCategories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoterCategoryModel voterCategoryModel = await db.VoterCategoryModels.FindAsync(id);
            if (voterCategoryModel == null)
            {
                return HttpNotFound();
            }
            return View(voterCategoryModel);
        }

        // GET: VoterCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VoterCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,DateCreated,DateModified")] VoterCategoryModel voterCategoryModel)
        {
            if (ModelState.IsValid)
            {
                voterCategoryModel.DateCreated = DateTime.Now;
                db.VoterCategoryModels.Add(voterCategoryModel);
                await db.SaveChangesAsync();
                TempData["Message"] = Utils.GenerateToastSuccess($"Voter category created successfully", "New Voter Category");
                return RedirectToAction("Index");
            }

            return View(voterCategoryModel);
        }

        // GET: VoterCategories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            VoterCategoryViewModel model = null;

            try
            {
                VoterCategoryModel voterCategoryModel = await db.VoterCategoryModels.FindAsync(id);
                if (voterCategoryModel == null)
                {
                    return HttpNotFound();
                }

                model = new VoterCategoryViewModel
                {
                    Id = voterCategoryModel.Id,
                    Name = voterCategoryModel.Name,
                    Description = voterCategoryModel.Description
                };
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError($"Voter category not found");
            }

            



            return View(model);
        }

        // POST: VoterCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description")] VoterCategoryViewModel model)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    VoterCategoryModel voterCategoryModel = await db.VoterCategoryModels.FindAsync(model.Id);

                    if (voterCategoryModel == null)
                    {
                        TempData["Message"] = Utils.GenerateToastError($"Election type not found", "Edit Election Type");
                    }
                    else
                    {
                        voterCategoryModel.Name = model.Name;
                        voterCategoryModel.Description = model.Description;
                        voterCategoryModel.DateModified = DateTime.Now;

                        db.Entry(voterCategoryModel).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        TempData["Message"] = Utils.GenerateToastSuccess($"Voter category updated successfully", "Edit Voter Category");
                        return RedirectToAction("Index");
                    }

                }
            }
            catch(Exception e)
            {
                TempData["Message"] = Utils.GenerateToastError($"Voter category not updated", "Edit Voter Category");
                Utils.LogException(e);
            }

            
            return View(model);
        }

        // GET: VoterCategories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoterCategoryModel voterCategoryModel = await db.VoterCategoryModels.FindAsync(id);
            if (voterCategoryModel == null)
            {
                return HttpNotFound();
            }
            return View(voterCategoryModel);
        }

        // POST: VoterCategories/Delete/5
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
                VoterCategoryModel voterCategoryModel = await db.VoterCategoryModels.FindAsync(id);
                db.VoterCategoryModels.Remove(voterCategoryModel);
                await db.SaveChangesAsync();

                response.status = ResultCodes.SUCCESS;
                response.message = "Election type deleted successfully";
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
