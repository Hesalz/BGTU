using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DAL_Celebrity_MSSQL;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using ASPA008_1.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using ANC25_WEBAPI_DLL.Services;
using ASPA008_1.Filters;
using ANC25_WEBAPI_DLL;

namespace ASPA008_1.Controllers
{
    public static class ImgPath {
        public static string path = "";
    }
    public class CelebritiesController : Controller
    {
        private readonly IRepository _repository;
        private readonly IWebHostEnvironment _env;
        private readonly CelebritiesConfig _config;
        private readonly string _customPhotosPath = @"D:\БГТУ\V сем\СТСР\lab08\ASPA008_1\Photos";


        public CelebritiesController(
            IRepository repository,
            IWebHostEnvironment env,
            IOptions<CelebritiesConfig> config)
        {
            _repository = repository;
            _env = env;
            _config = config.Value;
        }

        public class IndexModel
        {
            public string photosrequestpath { get; set; }
            public IEnumerable<Celebrity> celebrities { get; set; }
        }

        public IActionResult Index()
        {
            var model = new IndexModel
            {
                photosrequestpath = _config.PhotosFolderRequestPath,
                celebrities = _repository.GetAllCelebrities()
            };
            return View(model);
        }

        public IActionResult NewHumanForm(bool isCancel = false)
        {
            if (isCancel)
            {
                if (!string.IsNullOrEmpty(ImgPath.path))
                {
                    var tempFilePath = Path.Combine(_customPhotosPath, ImgPath.path);
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        System.IO.File.Delete(tempFilePath);
                    }
                    ImgPath.path = "";
                }
                TempData.Clear();
            }

            var countriesPath = Path.Combine(_env.ContentRootPath, _config.ISO3166alpha2Path);
            var countryCodes = CountryCodes.LoadFromFile(countriesPath);
            ViewData["Nationality"] = new SelectList(countryCodes, "Code", "Name");

            return View(new CelebrityViewModel { IsCorrect = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessForm(CelebrityViewModel model, string handler)
        {
            if (handler == "render")
            {
                return HandleRender(model);
            }
            else if (handler == "save")
            {
                return await HandleSave(model);
            }
            else if (handler == "cancel")
            {
                return HandleCancel();
            }

            return RedirectToAction("NewHumanForm");
        }

        private IActionResult HandleRender(CelebrityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("NewHumanForm", model);
            }

            if (string.IsNullOrWhiteSpace(model.FullName) || string.IsNullOrWhiteSpace(model.Nationality))
            {
                ModelState.AddModelError("", "Пожалуйста, заполните все обязательные поля");
                return View("NewHumanForm", model);
            }

            if (model.Upload == null || model.Upload.Length == 0)
            {
                ModelState.AddModelError("Upload", "Пожалуйста, загрузите фотографию");
                return View("NewHumanForm", model);
            }

            if (model.Upload != null && model.Upload.Length > 0)
            {
                ImgPath.path = model.Upload.FileName;
                var tempFileName = model.Upload.FileName;
                var tempFilePath = Path.Combine(_customPhotosPath, tempFileName);

                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    model.Upload.CopyTo(stream);
                }

                ViewData["TempPhotoPath"] = tempFileName;
                TempData["TempPhotoPath"] = tempFileName;
                TempData.Keep("TempPhotoPath");
            }

            ViewData["FullName"] = model.FullName;
            ViewData["Nationality"] = model.Nationality;
            TempData["FullName"] = model.FullName;
            TempData["Nationality"] = model.Nationality;
            TempData.Keep("FullName");
            TempData.Keep("Nationality");

            model.IsCorrect = false;
            return View("NewHumanForm", model);
        }

        private async Task<IActionResult> HandleSave(CelebrityViewModel model)
        {
            try
            {
                var tempFileName = ViewData["TempPhotoPath"] as string ?? TempData["TempPhotoPath"] as string;
                TempData.Keep("TempPhotoPath");

                if (string.IsNullOrEmpty(tempFileName))
                {
                    ModelState.AddModelError("", "Фото не было загружено");
                    return View("NewHumanForm", model);
                }

                var tempFilePath = Path.Combine(_customPhotosPath, tempFileName);
                var uniqueFileName = $"perm_{Guid.NewGuid()}{Path.GetExtension(tempFileName)}";
                var permFilePath = Path.Combine(_customPhotosPath, uniqueFileName);

                System.IO.File.Move(tempFilePath, permFilePath);

                using (var db = new Repository(_config.ConnectionString)) 
                {
                    var celebrity = new Celebrity
                    {
                        FullName = TempData["FullName"]?.ToString(),
                        Nationality = TempData["Nationality"]?.ToString(),
                        ReqPhotoPath = uniqueFileName 
                    };

                    if (!db.AddCelebrity(celebrity))
                    {
                        if (System.IO.File.Exists(permFilePath))
                        {
                            System.IO.File.Delete(permFilePath);
                        }

                        ModelState.AddModelError("", "Не удалось сохранить данные в базу данных");
                        return View("NewHumanForm", model);
                    }
                }

                TempData.Remove("TempPhotoPath");
                TempData.Remove("FullName");
                TempData.Remove("Nationality");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка: {ex.Message}");
                return View("NewHumanForm", model);
            }
        }

        [HttpPost, ActionName("HandleCancel")]
        private IActionResult HandleCancel()
        {
            if (!string.IsNullOrEmpty(ImgPath.path))
            {
                var tempFilePath = Path.Combine(_customPhotosPath, ImgPath.path);
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }
            }
            ImgPath.path = "";
            TempData.Clear();

            return RedirectToAction("NewHumanForm");
        }

        [InfoAsyncActionFilter(InfoAsyncActionFilter.Wikipedia)]
        public IActionResult Celebrities(int id)
        {
            var celebrity = _repository.GetCelebrityById(id);
            if (celebrity == null)
            {
                return NotFound();
            }

            ViewBag.LifeEvents = _repository.GetLifeEventsByCelebrityId(id);

            if (HttpContext.Items.TryGetValue(InfoAsyncActionFilter.Wikipedia, out var wikiRefs))
            {
                ViewBag.WikipediaReferences = wikiRefs as Dictionary<string, string>;
            }
            //celebrity.ReqPhotoPath = Path.Combine(_customPhotosPath, celebrity.ReqPhotoPath);
            return View(celebrity);
        }
        public IActionResult Edit(int id)
        {
            var celebrity = _repository.GetCelebrityById(id);
            if (celebrity == null)
            {
                return NotFound();
            }

            var countriesPath = Path.Combine(_env.ContentRootPath, _config.ISO3166alpha2Path);
            var countryCodes = CountryCodes.LoadFromFile(countriesPath);
            ViewData["Nationality"] = new SelectList(countryCodes, "Code", "Name", celebrity.Nationality);

            return View(celebrity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Celebrity celebrity, IFormFile newPhoto)
        {
            try
            {
                if (id != celebrity.Id)
                {
                    return NotFound();
                }

                var existingCelebrity = _repository.GetCelebrityById(id);
                if (existingCelebrity == null)
                {
                    return NotFound();
                }

                celebrity.ReqPhotoPath = existingCelebrity.ReqPhotoPath;

                if (newPhoto != null && newPhoto.Length > 0)
                {
                    if (!Directory.Exists(_customPhotosPath))
                    {
                        Directory.CreateDirectory(_customPhotosPath);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(newPhoto.FileName)}";
                    var filePath = Path.Combine(_customPhotosPath, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await newPhoto.CopyToAsync(stream);
                    }

                    if (!string.IsNullOrEmpty(existingCelebrity.ReqPhotoPath))
                    {
                        var oldFileName = Path.GetFileName(existingCelebrity.ReqPhotoPath);
                        var oldFilePath = Path.Combine(_customPhotosPath, oldFileName);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    celebrity.ReqPhotoPath = uniqueFileName;
                }

                if (!_repository.UpdateCelebrity(id, celebrity))
                {
                    throw new Exception("Failed to update celebrity");
                }

                return RedirectToAction(nameof(Celebrities), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Произошла ошибка при обновлении данных");

                var originalCelebrity = _repository.GetCelebrityById(id);
                return View(originalCelebrity ?? celebrity);
            }
        }

        public IActionResult Delete(int id)
        {
            var celebrity = _repository.GetCelebrityById(id);
            if (celebrity == null)
            {
                return NotFound();
            }

            return View(celebrity);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var celebrity = _repository.GetCelebrityById(id);
                if (celebrity == null)
                {
                    return NotFound();
                }
                    
                if (!string.IsNullOrEmpty(celebrity.ReqPhotoPath))
                {
                    var filePath = Path.Combine(_env.WebRootPath, celebrity.ReqPhotoPath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                if (!_repository.DeleteCelebrity(id))
                {
                    throw new Exception("Failed to delete celebrity");
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Delete", _repository.GetCelebrityById(id));
            }
        }

    }
}