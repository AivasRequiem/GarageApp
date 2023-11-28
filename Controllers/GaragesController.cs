using Microsoft.AspNetCore.Mvc;
using GarageApp.Data;
using GarageApp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GarageApp.Services;

namespace GarageApp.Controllers
{
    public class GaragesController : Controller
    {
        private readonly GarageManagmentService _garageManagmentService;
        private readonly SpecializationManagmentService _specializationManagmentService;
        private readonly GarageServicesManagmentService _garageServicesManagmentService;

        public GaragesController(ApplicationDbContext context)
        {
            _garageManagmentService = new GarageManagmentService(context);
            _specializationManagmentService = new SpecializationManagmentService(context);
            _garageServicesManagmentService = new GarageServicesManagmentService(context);
        }

        // GET: Garages
        public async Task<IActionResult> Index(string garageSpecialization, string searchString)
        {
            GarageSpecializationsViewModel garages = await _garageManagmentService.GetGaragesBySpecialization(garageSpecialization, searchString);
            if (garages.isValid)
            {
                return View(await _garageManagmentService.GetGaragesBySpecialization(garageSpecialization, searchString));
            }
            else
            {
                return View(new ErrorViewModel { RequestId = garages.Exeption });
            }
        }

        [HttpGet("GetGarages")]
        public async Task<IEnumerable<Garage>> GetGarages()
        {
            return await _garageManagmentService.GetAllGarages();
        }

        // GET: Garages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _garageManagmentService.IsAnyGarages())
            {
                return NotFound();
            }

            Garage garage = await _garageManagmentService.GetGarage(id);

            if (garage == null)
            {
                return NotFound();
            }

            ViewBag.GarageServices = await _garageServicesManagmentService.GetGarageServicesByGarageId(id);
            ViewBag.Specialization = await _specializationManagmentService.GetAllSpecializations();

            return View(garage);
        }

        // GET: Garages/Create
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Specialization = await _specializationManagmentService.GetAllSpecializations();
            return View();
        }

        // POST: Garages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Create(Garage garage, string[] GarageSpecializations)
        {
            if (_garageManagmentService.IsExistGarage(garage))
            {
                return View(new ErrorViewModel { RequestId = "Garage with same name exists" });
            }            

            if (ModelState.IsValid)
            {
                await _garageManagmentService.CreateGarage(garage, GarageSpecializations,
                                new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)));

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Specialization = await _specializationManagmentService.GetAllSpecializations();

            return View(garage);
        }

        // GET: Garages/Edit/5
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _garageManagmentService.IsAnyGarages())
            {
                return NotFound();
            }

            Garage garage = await _garageManagmentService.GetGarage(id);

            if (garage == null)
            {
                return NotFound();
            }

            ViewBag.Specialization = await _specializationManagmentService.GetAllSpecializations();

            return View(garage);
        }

        // POST: Garages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Edit(int id, Garage garage, string[] garageSpecializations)
        {
            if (id != garage.Id)
            {
                return View(new ErrorViewModel { RequestId = $"Garage with ID {id} doesn't exist" });
            }

            Garage existingGarage = await _garageManagmentService.GetGarage(id);

            if (ModelState.IsValid)
            {
                await _garageManagmentService.EditGarage(existingGarage, garage, garageSpecializations);
                return RedirectToAction(nameof(Index));
            }
            return View(existingGarage);
        }

        // GET: Garages/Delete/5
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _garageManagmentService.IsAnyGarages())
            {
                return NotFound();
            }

            Garage garage = await _garageManagmentService.GetGarage(id);

            if (garage == null)
            {
                return NotFound();
            }

            return View(garage);
        }

        // POST: Garages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,garageOwner")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if(await _garageManagmentService.DeleteGarage(id))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(new ErrorViewModel { RequestId = "Entity set 'ApplicationDbContext.Garages'  is null." });
            }            
        }
    }
}
