using Edukatee.Contexts;
using Edukatee.ViewModels.SettingVM;
using Edukatee.ViewModels.SliderVm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Edukatee.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : Controller
    {
        DataDbContext _db { get; set; }
        
        public SettingController(DataDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Settings.Select(c=> new SettingListItemVm
            {
                Id= c.Id,
                UpdatedTime= c.UpdatedTime,
                Address= c.Address,
                Email= c.Email,
                PhoneNumber= c.PhoneNumber,
               
            }).ToListAsync());
        }
		public async Task<IActionResult> Cancel()
		{
			return RedirectToAction(nameof(Index));
		}
		public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();
            var data = await _db.Settings.FindAsync(id);
            if (data == null) return NotFound();
            return View(new SettingUpdateVm
            {
                Address = data.Address,
                Email = data.Email,
                PhoneNumber = data.PhoneNumber,
                
            });
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, SettingUpdateVm vm)
        {
            if (id == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var data = await _db.Settings.FindAsync(id);
            if (data == null) return NotFound();
            data.Email = vm.Email;
            data.Address = vm.Address;
            data.PhoneNumber = vm.PhoneNumber;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
