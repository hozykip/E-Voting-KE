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
using System.Data.Entity.Validation;

namespace E_Vote_System.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class VotersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Voters
        public async Task<ActionResult> Index()
        {
            try
            {
                var voterModels = db.VoterModels.Include(v => v.User).Include(v => v.VoterCategoryModel);
                return View(await voterModels.ToListAsync());
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(e.Message);
            }

            return View();
        }

        // GET: Voters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoterModel voterModel = await db.VoterModels.FindAsync(id);
            if (voterModel == null)
            {
                return HttpNotFound();
            }
            return View(voterModel);
        }

        // GET: Voters/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.VoterCategoryId = new SelectList(db.VoterCategoryModels, "Id", "Name");          


            return View(new VoterModelViewModel());
        }

        // POST: Voters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,IdNumber,Address,DateCreated,DateModified,VoterCategoryModelId,ProfilePictureUpload")] VoterModelViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    VoterModel voterModel = new VoterModel();

                    VoterModel voterModel1 = await db.VoterModels.FirstOrDefaultAsync(v => v.EmailAddress == voterModel.EmailAddress);
                    if(voterModel1 != null)
                    {
                        TempData["Message"] = Utils.GenerateToastError("A voter with this email address already exists");
                        return View(voterModel);
                    }

                    voterModel.Address = viewModel.Address;
                    voterModel.EmailAddress = viewModel.EmailAddress;
                    voterModel.FirstName = viewModel.FirstName;
                    voterModel.LastName = viewModel.LastName;
                    voterModel.IdNumber = viewModel.IdNumber;
                    voterModel.VoterCategoryModelId = viewModel.VoterCategoryModelId;

                    if (viewModel.ProfilePictureUpload.ContentLength > 0)
                    {
                        string extension = Path.GetExtension(viewModel.ProfilePictureUpload.FileName);
                        string guid = Guid.NewGuid().ToString();
                        string fileName = "Profile-" + guid + extension;

                        string dirPath = $"{Configs.DocumentsPath}VoterImages\\";

                        if (Utils.CreateFolderIfNotExists(dirPath))
                        {
                            viewModel.ProfilePictureUpload.SaveAs(dirPath + fileName);

                            voterModel.ProfilePicture = fileName;
                        }
                        
                    }

                    voterModel.DateCreated = DateTime.Now;

                    ApplicationUser user = await db.Users.FirstOrDefaultAsync(u => u.Email == voterModel.EmailAddress);

                    if (user != null)
                    {
                        voterModel.UserId = user.Id;
                    }
                    db.VoterModels.Add(voterModel);
                    await db.SaveChangesAsync();

                    TempData["Message"] = Utils.GenerateToastSuccess("New voter has been added successfully. Voter number: " + voterModel.Id,"New Voter");

                    return RedirectToAction("Index");
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(e.Message);
            }

            

            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", viewModel.UserId);
            ViewBag.VoterCategoryId = new SelectList(db.VoterCategoryModels, "Id", "Name", viewModel.VoterCategoryModelId);
            return View(viewModel);
        }

        // GET: Voters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            try
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                VoterModel voterModel = await db.VoterModels.FindAsync(id);
                if (voterModel == null)
                {
                    return HttpNotFound();
                }

                VoterModelEditViewModel vm = VoterModelEditViewModel.fromVoterModel(voterModel);

                ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", voterModel.UserId);
                ViewBag.VoterCategoryId = new SelectList(db.VoterCategoryModels, "Id", "Name", voterModel.VoterCategoryModelId);
                return View(vm);
            }
            catch(Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(e.Message);
            }

            
            return View();
        }

        // POST: Voters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,IdNumber,Address,DateModified,UserId,VoterCategoryModelId,ProfilePictureUpload,ProfilePicture")] VoterModelEditViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    VoterModel voterModel1 = await db.VoterModels.FirstOrDefaultAsync(v => v.EmailAddress == viewModel.EmailAddress && v.Id != viewModel.Id);
                    if (voterModel1 != null)
                    {
                        TempData["Message"] = Utils.GenerateToastError("A voter with this email address already exists");
                        return View(viewModel);
                    }

                    VoterModel voterModel = await db.VoterModels.FindAsync(viewModel.Id);

                    if(voterModel == null)
                    {
                        return HttpNotFound();
                    }

                    if (viewModel.ProfilePictureUpload != null && viewModel.ProfilePictureUpload.ContentLength > 0)
                    {
                        string extension = Path.GetExtension(viewModel.ProfilePictureUpload.FileName);
                        string guid = Guid.NewGuid().ToString();

                        string fileName = voterModel.ProfilePicture ?? "Profile-" + guid + extension;

                        string dirPath = $"{Configs.DocumentsPath}VoterImages\\";

                        if (Utils.CreateFolderIfNotExists(dirPath))
                        {
                            viewModel.ProfilePictureUpload.SaveAs(dirPath + fileName);

                            voterModel.ProfilePicture = fileName;
                        }

                    }

                    voterModel.Address = viewModel.Address;
                    voterModel.EmailAddress = viewModel.EmailAddress;
                    voterModel.FirstName = viewModel.FirstName;
                    voterModel.LastName = viewModel.LastName;
                    voterModel.IdNumber = viewModel.IdNumber;
                    voterModel.VoterCategoryModelId = viewModel.VoterCategoryModelId;


                    voterModel.DateModified = DateTime.Now;

                    ApplicationUser user = await db.Users.FirstOrDefaultAsync(u => u.Email == voterModel.EmailAddress);

                    if(user != null)
                    {
                        voterModel.UserId = user.Id;
                    }

                    db.Entry(voterModel).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    TempData["Message"] = Utils.GenerateToastSuccess($"Voter #{voterModel.Id} has been updated successfully", "Edit voter");
                    return RedirectToAction("Index");
                }
                else
                {

                    var errors = ModelState.Select(e => new { key = e.Key, val = e.Value.Errors }).Where(r => r.val.Count > 0);

                    TempData["Message"] = Utils.GenerateToastError($"Validation errors: {string.Join(",", errors)}");
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {

                        Utils.LogErrors(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));

                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);

                        TempData["Message"] = Utils.GenerateToastError(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
                
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                TempData["Message"] = Utils.GenerateToastError(e.Message);
            }

            
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", viewModel.UserId);
            ViewBag.VoterCategoryId = new SelectList(db.VoterCategoryModels, "Id", "Name", viewModel.VoterCategoryModelId);
            return View(viewModel);
        }

        // GET: Voters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoterModel voterModel = await db.VoterModels.FindAsync(id);
            if (voterModel == null)
            {
                return HttpNotFound();
            }
            return View(voterModel);
        }

        // POST: Voters/Delete/5
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
                VoterModel voterModel = await db.VoterModels.FindAsync(id);
                db.VoterModels.Remove(voterModel);
                await db.SaveChangesAsync();

                try
                {
                    ApplicationUser user = await db.Users.FirstOrDefaultAsync(u => u.Email == voterModel.EmailAddress);

                    if (user != null)
                    {
                        db.Users.Remove(user);
                        await db.SaveChangesAsync();
                    }
                }
                catch(Exception e)
                {
                    Utils.LogException(e);
                }

                response.status = ResultCodes.SUCCESS;
                response.message = "Voter has been deleted successfully";
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
