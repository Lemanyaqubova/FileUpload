using FrontToBack.DAL;
using FrontToBack.Extentions;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FrontToBack.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext appDbContext, IWebHostEnvironment env)
        {
            _appDbContext = appDbContext;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_appDbContext.Sliders.ToList());
        }
        public IActionResult Create(SliderCreateVM sliderCreateVM)
        {
            if (sliderCreateVM.Photo==null)
            {
                ModelState.AddModelError("Photo", "bosh qoyma");
                return View();
            }
            if (!sliderCreateVM.Photo.IsImage()) 
            {
                ModelState.AddModelError("Photo", "ancag shekil");
                return View();
            }
            if (sliderCreateVM.Photo.CheckImageSize(500))
            {
                ModelState.AddModelError("Photo", "olcusu boyukdur");
                return View();
            }
             string fileName=Guid.NewGuid().ToString()+sliderCreateVM.Photo.FileName;
            string fullpath = Path.Combine(_env.WebRootPath, "img", fileName);
            using(FileStream stream=new FileStream(fullpath, FileMode.Create))
            {
                sliderCreateVM.Photo.CopyTo(stream);
            }

            Slider newSlider = new();
            newSlider.ImageUrl = sliderCreateVM.Photo.SaveImage(_env,"img",sliderCreateVM.Photo.FileName);
            _appDbContext.Sliders.Add(newSlider);
            _appDbContext.SaveChanges();
                return RedirectToAction("index");
        }
    }
}
    