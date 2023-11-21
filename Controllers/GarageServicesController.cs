using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GarageApp.Data;
using GarageApp.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using GarageApp.Services;

namespace GarageApp.Controllers
{
    public class GarageServicesController : Controller
    {
        private readonly GarageServicesManagmentService _garageServicesManagment;
        private readonly GarageManagmentService _garageManagment;
        private readonly SpecializationManagmentService _specializationManagment;

        public GarageServicesController(ApplicationDbContext context)
        {
            _garageServicesManagment = new GarageServicesManagmentService(context);
            _garageManagment = new GarageManagmentService(context);
            _specializationManagment = new SpecializationManagmentService(context);
        }

        // GET: GarageServices
        public async Task<IActionResult> Index()
        {
            return View(await _garageServicesManagment.GetAllGarageServices());
        }

        // GET: GarageServices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _garageServicesManagment.IsAnyGarageServices())
            {
                return NotFound();
            }

            GarageService garageService = await _garageServicesManagment.GetGarageService(id);
            if (garageService == null)
            {
                return NotFound();
            }

            return View(garageService);
        }

        // GET: GarageServices/Create/id
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Create(int id)
        {
            ViewData["GarageId"] = id;

            try
            {
                List<Specialization> specializations = await _specializationManagment.GetSpecializationOfGarage(id);
                ViewBag.Specializations = new SelectList(specializations, "Id", "Name");
            }
            catch (Exception ex)
            {
                return View(new ErrorViewModel { RequestId = ex.Message });
            }

            return View();
        }

        public class GarageServiceFromForm
        {
            [Required]
            public string Name { get; set; }
            public string? Description { get; set; }
            public decimal? Price { get; set; }
            public Guid SpecializationId { get; set; }
        }

        // POST: GarageServices/Create/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Create(int id, [Bind("Name,Description,Price,SpecializationId")] GarageServiceFromForm garageService)
        {            
            if (ModelState.IsValid)
            {
                await _garageServicesManagment.CreateService(id, garageService);
                return RedirectToAction("Details", "Garages", new { id = id });
            }

            return View(garageService);
        }

        // GET: GarageServices/Edit/5
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _garageServicesManagment.IsAnyGarageServices())
            {
                return NotFound();
            }

            GarageService garageService = await _garageServicesManagment.GetGarageService(id);
            if (garageService == null)
            {
                return NotFound();
            }
            return View(garageService);
        }

        // POST: GarageServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,Price")] GarageService garageService)
        {
            if (id != garageService.Id)
            {
                return View(new ErrorViewModel { RequestId = $"Garage service with ID {id} doesn't match." });
            }

            GarageService existingService = await _garageServicesManagment.GetGarageService(id);

            if (ModelState.IsValid)
            {
                try
                {
                    await _garageServicesManagment.EditService(garageService);
                }
                catch (Exception ex)
                {
                    return View(new ErrorViewModel { RequestId = ex.Message });
                }
                return RedirectToAction(nameof(Index));
            }
            return View(garageService);
        }

        // GET: GarageServices/Delete/5
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _garageServicesManagment.IsAnyGarageServices())
            {
                return NotFound();
            }

            var garageService = await _garageServicesManagment.GetGarageService(id);
            if (garageService == null)
            {
                return NotFound();
            }

            return View(garageService);
        }

        // POST: GarageServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _garageServicesManagment.DeleteService(id);
            }
            catch (Exception ex)
            {
                return View(new ErrorViewModel { RequestId = ex.Message });
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
