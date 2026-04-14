using DAL_Celebrity_MSSQL.Interfaces;
using DAL_Celebrity_MSSQL.Models;
using Lab7.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.IO;

namespace Lab7.Pages
{
    public class NewCelebrityModel : PageModel
    {
        public IRepository repository;
        public string PhotosRequestPath { get; set; }
        public string PhotosFolder { get; set; }
        public Celebrity? Celebrity { get; set; }

        public NewCelebrityModel(IRepository repository, IOptions<CelebritiesConfig> config) {
            this.repository = repository;
            PhotosRequestPath = config.Value.PhotosRequestPath;
            PhotosFolder = config.Value.PhotosFolder;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnGetConfirm(string fullname, string nationality, string filename)
        {
            ViewData["Confirm"] = true;
            Celebrity = new Celebrity()
            {
                FullName = fullname,
                Nationality = nationality,
                ReqPhotoPath = filename,
            };
            return Page();
        }
        public IActionResult OnPost(
            [FromForm]string? fullname,
            [FromForm]string? nationality,
            IFormFile? upload,
            [FromForm]string? press,
            [FromForm]string? filename)
        {
            IActionResult rc = RedirectToPage("Celebrities");
            if(string.IsNullOrEmpty(press) && upload != null)
            {
                string fn = Path.GetFileName(Path.GetTempFileName());
                string fp = Path.Combine(PhotosFolder, fn);
                FileStream file = new FileStream(fp, FileMode.CreateNew);
                upload.CopyTo(file);
                file.Close();
                rc = RedirectToPage("NewCelebrity", "Confirm", new { filename = fn, fullname, nationality });
            }
            else if(press.Equals("Confirm"))
            {
                string newFileName = $"{fullname.Replace(" ", "_")}.{filename}.jpg";
                Directory.Move(Path.Combine(PhotosFolder, filename), Path.Combine(PhotosFolder, newFileName));
                repository.AddCelebrity(new Celebrity
                {
                    FullName = fullname,
                    Nationality = nationality,
                    ReqPhotoPath = newFileName,
                });
                rc = RedirectToPage("Celebrities");
            }
            else if(press != null && press.Equals("Cancel", StringComparison.OrdinalIgnoreCase))
            {
                // Удаляем временный файл при отмене
                if (!string.IsNullOrEmpty(filename))
                {
                    string tempFilePath = Path.Combine(PhotosFolder, filename);
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        System.IO.File.Delete(tempFilePath);
                    }
                }
                rc = RedirectToPage("NewCelebrity");
            }
            else
            {
                rc = RedirectToPage("NewCelebrity");
            }
            return rc;
        }
    }
}
