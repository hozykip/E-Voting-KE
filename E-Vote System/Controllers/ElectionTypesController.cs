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
    public class ElectionTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ElectionTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.ElectionTypes.ToListAsync());
        }

        // GET: ElectionTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ElectionTypes electionTypes = await db.ElectionTypes.FindAsync(id);
            if (electionTypes == null)
            {
                return HttpNotFound();
            }
            return View(electionTypes);
        }

        // GET: ElectionTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ElectionTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Type,DateCreated,DateModified,Description")] ElectionTypes electionTypes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    electionTypes.DateCreated = DateTime.Now;
                    db.ElectionTypes.Add(electionTypes);
                    await db.SaveChangesAsync();
                    TempData["Message"] = Utils.GenerateToastSuccess($"Election type created successfully","New Election Type");
                    return RedirectToAction("Index");
                }
            }
            catch(Exception e)
            {
                TempData["Message"] = Utils.GenerateToastError($"Election type not created", "New Election Type");
                Utils.LogException(e);
            }
            

            return View(electionTypes);
        }

        // GET: ElectionTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ElectionTypesEditViewModel model = null;

            try
            {
                ElectionTypes electionTypes = await db.ElectionTypes.FindAsync(id);

                model = new ElectionTypesEditViewModel
                {
                    Id = id.Value,
                    Type = electionTypes.Type,
                    Description = electionTypes.Description
                };

                if (electionTypes == null)
                {
                    return HttpNotFound();
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError($"Election type not found");
            }

            
            return View(model);
        }

        // POST: ElectionTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Type,Description")] ElectionTypesEditViewModel model)
        {

            try
            {

                if (ModelState.IsValid)
                {
                    ElectionTypes electionTypes = await db.ElectionTypes.FindAsync(model.Id);

                    electionTypes.Type = model.Type;
                    electionTypes.Description = model.Description;
                    electionTypes.DateModified = DateTime.Now;

                    db.Entry(electionTypes).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["Message"] = Utils.GenerateToastSuccess($"Election type updated successfully", "Edit Election Type");
                    return RedirectToAction("Index");
                }
                
            }
            catch (Exception e)
            {
                TempData["Message"] = Utils.GenerateToastError($"Election type not updated", "Edit Election Type");
                Utils.LogException(e);
            }

            
            return View(model);
        }

        // GET: ElectionTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ElectionTypes electionTypes = await db.ElectionTypes.FindAsync(id);
            if (electionTypes == null)
            {
                return HttpNotFound();
            }
            return View(electionTypes);
        }

        // POST: ElectionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DefaultJsonResponse response = new DefaultJsonResponse
            {
                status = ResultCodes.FAIL,
                message = Configs.DefaultErrorMessage
            };                        

            try
            {
                ElectionTypes electionTypes = await db.ElectionTypes.FindAsync(id);
                db.ElectionTypes.Remove(electionTypes);
                await db.SaveChangesAsync();

                response.status = ResultCodes.SUCCESS;
                response.message = "Election type deleted successfully";
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
