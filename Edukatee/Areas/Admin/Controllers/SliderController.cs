using Edukatee.Contexts;
using Edukatee.Models;
using Edukatee.ViewModels.Common;
using Edukatee.ViewModels.SliderVm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace Edukatee.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]
	public class SliderController : Controller
	{
        DataDbContext _db { get; set; }
        IWebHostEnvironment _env { get; set; }
        public SliderController(DataDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }


		
		public async Task<IActionResult> Index()
		{
			int take = 4;
			var items = await _db.Sliders.Take(take).Select(c => new SliderListItemVm
			{
				Id = c.Id,
				ImageUrl = c.ImageUrl,
				Title = c.Title,
				CreatedTime = c.CreatedTime,
				UpdatedTime = c.UpdatedTime,
				IsDeleted = c.IsDeleted,
				Profession = c.Profession
			}).ToListAsync();
			int count = await _db.Sliders.CountAsync();
			PaginationVm<IEnumerable<SliderListItemVm>> pag = new(count, 1, (int)Math.Ceiling((decimal)count / take), items);
			return View(pag);
		}
        public async Task<IActionResult>ProductPagination(int page =1,int count = 8)
        {
            var items = await _db.Sliders.Skip((page-1)*count).Take(count).Select(c=> new SliderListItemVm
            {
				Id = c.Id,
				ImageUrl = c.ImageUrl,
				Title = c.Title,
				CreatedTime = c.CreatedTime,
				UpdatedTime = c.UpdatedTime,
				IsDeleted = c.IsDeleted,
				Profession = c.Profession
			}).ToListAsync();
            int totalCount = await _db.Sliders.CountAsync();
            PaginationVm<IEnumerable<SliderListItemVm>> pag = new(totalCount, page, (int)Math.Ceiling((decimal)totalCount / count), items);
            return PartialView("ProductPagination", pag);
        }
		public async Task<IActionResult> Create()
        {
            return View();
        }
        public async Task<IActionResult> Cancel()
        {
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult>Create(SliderCreateItemVm vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }
            string filename = null;
            if (vm.MainIMage != null)
            {
                filename = Guid.NewGuid() + Path.GetExtension(vm.MainIMage.FileName);
                using (Stream fs = new FileStream(Path.Combine(_env.WebRootPath, "Assets", "images", "stories", filename), FileMode.Create))
                {
                    await vm.MainIMage.CopyToAsync(fs);
                }
            }
            Slider slider = new Slider()
            {
                Title = vm.Title,
                Profession = vm.Profession,
                ImageUrl = filename
            };
            _db.Sliders.AddAsync(slider);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult>Update(int? id)
        {
            if (id == null) return BadRequest();
            var data = await _db.Sliders.FindAsync(id);
            if (data == null) return NotFound();
            return View(new SliderUpdateVm
            {
                ImageUrl = data.ImageUrl,
                Profession = data.Profession,
                Title = data.Title,
            });
        }
        [HttpPost]
        public async Task<IActionResult>Update(int? id,SliderUpdateVm vm)
        {
            if(id == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var data = await _db.Sliders.FindAsync(id);
            if (data == null) return NotFound();
            data.Profession = vm.Profession;
            data.Title = vm.Title;

            if (!string.IsNullOrEmpty(data.ImageUrl))
            {
                string filepath = Path.Combine(_env.WebRootPath, "Assets", "images", "stories", data.ImageUrl);
                if(System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
            }
            string filename = Guid.NewGuid() + Path.GetExtension(vm.MainIMage.FileName);
            using (Stream fs = new FileStream(Path.Combine(_env.WebRootPath, "Assets", "images", "stories", filename), FileMode.Create))
            {
                await vm.MainIMage.CopyToAsync(fs);
            }
            data.ImageUrl = filename;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult>DeleteProduct(int? id)
        {
            if(id==null) return BadRequest();
            var data = await _db.Sliders.FindAsync(id);
            if (data == null) return NotFound();
            data.IsDeleted = true;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> RestoreProduct(int? id)
        {
            if (id == null) return BadRequest();
            var data = await _db.Sliders.FindAsync(id);
            if (data == null) return NotFound();
            data.IsDeleted = false;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteFromData(int? id)
        {
            if (id == null) return BadRequest();
            var data = await _db.Sliders.FindAsync(id);
            if (data == null) return NotFound();
             _db.Sliders.Remove(data);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
