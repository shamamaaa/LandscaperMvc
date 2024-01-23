using Landscaper.Areas.Admin.ViewModels;
using Landscaper.Areas.Admin.ViewModels.ServiceVms;
using Landscaper.DAL;
using Landscaper.Models;
using Landscaper.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Landscaper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;


        public ServiceController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int page=1)
        {
            double count = await _context.Services.CountAsync();
            var services = await _context.Services.Skip((page-1)*5).Take(5).ToListAsync();
            PaginationVm<Service> paginationVm = new PaginationVm<Service>
            {
                TotalPage = Math.Ceiling(count / 5),
                CurrentPage = page,
                Items = services
            };
            return View(paginationVm);
        }

        public IActionResult Create()
        {
            return View(); 
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceVm serviceVm)
        {
            if (!ModelState.IsValid)
            {
                return View(serviceVm);
            }

            bool result = _context.Services.Any(x => x.Name.Trim().ToLower() == serviceVm.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Service already exists.");
                return View(serviceVm);
            }
            if (!serviceVm.Photo.CheckType())
            {
                ModelState.AddModelError("Photo", "Photo type is not valid.");
                return View(serviceVm);
            }
            if (!serviceVm.Photo.CheckSize())
            {
                ModelState.AddModelError("Photo", "Photo size is not valid.");
                return View(serviceVm);
            }
            string name = await serviceVm.Photo.CreateFile(_env.WebRootPath,"assets","img","services");
            Service service = new Service
            {
                Name = serviceVm.Name,
                Description = serviceVm.Description,
                ImageUrl = name
            };
            await _context.Services.AddAsync(service);
            await _context.SaveChangesAsync();
            return RedirectToAction (nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            var serviceVm = await _context.Services.FirstOrDefaultAsync(x=>x.Id== id);
            if (serviceVm == null) return NotFound();
            UpdateServiceVm updateService = new UpdateServiceVm
            {
                Name = serviceVm.Name,
                Description = serviceVm.Description,
                ImageUrl = serviceVm.ImageUrl
            };
            return View(updateService);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateServiceVm serviceVm, int id)
        {
            if (!ModelState.IsValid)
            {
                return View(serviceVm);
            }
            var existed = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
            if (existed == null) return NotFound();

            bool result = _context.Services.Any(x => x.Name.Trim().ToLower() == serviceVm.Name.ToLower().Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "Service already exists.");
                return View(serviceVm);
            }
            if (serviceVm.Photo is not null)
            {
                if (!serviceVm.Photo.CheckType())
                {
                    ModelState.AddModelError("Photo", "Photo type is not valid.");
                    return View(serviceVm);
                }
                if (!serviceVm.Photo.CheckSize())
                {
                    ModelState.AddModelError("Photo", "Photo size is not valid.");
                    return View(serviceVm);
                }
                string newimage = await serviceVm.Photo.CreateFile(_env.WebRootPath, "assets", "img", "services");
                existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img", "services");
                existed.ImageUrl=newimage;
            }
            existed.Name=serviceVm.Name;
            existed.Description=serviceVm.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            var serviceVm = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
            if (serviceVm == null) return NotFound();
            _context.Services.Remove(serviceVm);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            var serviceVm = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
            if (serviceVm == null) return NotFound();

            return View(serviceVm);
        }
    }
}
