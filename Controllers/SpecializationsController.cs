using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarageApp.Data;
using GarageApp.Models;
using Microsoft.AspNetCore.Authorization;
using GarageApp.Services;

namespace GarageApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SpecializationsController : Controller
    {
        private readonly SpecializationManagmentService _specializationManagmentService;

        public SpecializationsController(ApplicationDbContext context)
        {
            _specializationManagmentService = new SpecializationManagmentService(context);
        }

        // GET: Specializations
        public async Task<IActionResult> Index()
        {
              return _specializationManagmentService.CheckContext() ? 
                          View(await _specializationManagmentService.GetAllSpecializations()) :
                          Problem("Entity set 'ApplicationDbContext.Specialization'  is null.");
        }

        // GET: Specializations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _specializationManagmentService.CheckContext())
            {
                return NotFound();
            }

            Specialization specialization = await _specializationManagmentService.GetSpecializationById(id);
            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        // GET: Specializations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specializations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Specialization specialization)
        {
            if (ModelState.IsValid)
            {
                await _specializationManagmentService.CreateSpecialization(specialization);
                return RedirectToAction(nameof(Index));
            }
            return View(specialization);
        }

        // GET: Specializations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _specializationManagmentService.CheckContext())
            {
                return NotFound();
            }

            Specialization specialization = await _specializationManagmentService.GetSpecializationById(id);
            if (specialization == null)
            {
                return NotFound();
            }
            return View(specialization);
        }

        // POST: Specializations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] Specialization specialization)
        {
            if (id != specialization.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _specializationManagmentService.EditSpecialization(specialization);
                return RedirectToAction(nameof(Index));
            }
            return View(specialization);
        }

        // GET: Specializations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _specializationManagmentService.CheckContext())
            {
                return NotFound();
            }

            Specialization specialization = await _specializationManagmentService.GetSpecializationById(id);
            if (specialization == null)
            {
                return NotFound();
            }

            return View(specialization);
        }

        // POST: Specializations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (await _specializationManagmentService.DeleteSpecialization(id))
            {
                return Problem("Entity set 'ApplicationDbContext.Specialization'  is null.");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
