using Edukatee.Contexts;
using Edukatee.ViewModels.SliderVm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Edukatee.ViewComponents
{
	public class SliderViewComponent : ViewComponent
	{
		DataDbContext _db { get; set; }

		public SliderViewComponent(DataDbContext db)
		{
			_db = db;
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
            return View(await _db.Sliders.Select(c => new SliderListItemVm
            {
                Id = c.Id,
                ImageUrl = c.ImageUrl,
                Title = c.Title,
                CreatedTime = c.CreatedTime,
                UpdatedTime = c.UpdatedTime,
                IsDeleted = c.IsDeleted,
                Profession = c.Profession
            }).ToListAsync());
        }
	}
}
