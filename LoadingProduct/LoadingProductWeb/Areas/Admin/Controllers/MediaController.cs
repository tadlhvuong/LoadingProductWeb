using LoadingProductShared.Data;
using LoadingProductShared.Helpers;
using LoadingProductWeb.Admin.Models;
using LoadingProductWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LoadingProductWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MediaController : Controller
    {
        private readonly string mediaUrl;
        private readonly string mediaPath;

        private readonly ILogger _logger;
        private readonly AppDBContext _dbContext;

        public MediaController(
            ILogger<MediaController> logger,
            AppDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;

            mediaUrl = AppSettings.Strings["MediaUrl"] ?? "/media1";
            mediaPath = AppSettings.Strings["MediaPath"] ?? "./wwwroot/media";
        }

        public IActionResult FileBrowser(PagedList<MediaFile> model)
        {
            model.PageSize = 10;

            var filterQuery = _dbContext.MediaFiles.Where(x => model.Search == null || x.FileName.Contains(model.Search) || x.MediaAlbum.FullName.Contains(model.Search));
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();

            foreach (var item in model.Content)
                item.FileLink = Path.Combine(mediaUrl, item.FullPath);

            return View(model);
        }

        public IActionResult FileManager(PagedList<MediaFile> model)
        {
            model.PageSize = 20;

            var filterQuery = _dbContext.MediaFiles.Where(x => (model.Search == null || x.FileName.Contains(model.Search) || x.MediaAlbum.FullName.Contains(model.Search))&& x.MediaAlbum.ShortName.Equals("Default"));
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();

            foreach (var item in model.Content)
                item.FileLink = Path.Combine(mediaUrl, item.FullPath);

            return View(model);
        }
        public IActionResult Banner(PagedList<MediaFile> model)
        {
            model.PageSize = 20;

            //var filterQuery = _dbContext.MediaFiles.Include(y => y.MediaAlbum).Where(x => model.Search == null || x.FileName.Contains(model.Search) || x.MediaAlbum.FullName.Contains(model.Search) && x.MediaAlbum.ShortName.Equals("Banner"));

            var filterQuery = _dbContext.MediaFiles.Include(x => x.MediaAlbum).Where(x => (model.Search == null || x.FileName.Contains(model.Search) || x.MediaAlbum.FullName.Contains(model.Search)) && x.MediaAlbum.ShortName.Equals("Banner"));
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();

            foreach (var item in model.Content)
                item.FileLink = Path.Combine(mediaUrl, item.FullPath);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> FileUpload(int? id, IEnumerable<IFormFile> files)
        {
            MediaAlbum album = GetDefaultAlbum(id);
            if (album == null)
                return BadRequest("Could not create Default Album!");

            return await DoUploadFile(album, files, _dbContext);
        }

        [HttpPost]
        public async Task<IActionResult> FileUploadBanner(int? id, IEnumerable<IFormFile> files)
        {
            var albumBanner = _dbContext.MediaAlbums.Include(x => x.MediaFiles).Where(x => x.ShortName.Equals("Banner")).Select(x=>x.Id).FirstOrDefault();

            MediaAlbum album = GetDefaultAlbumBanner(albumBanner);
            if (album == null)
                return BadRequest("Could not create Default Album!");

            return await DoUploadFile(album, files, _dbContext);
        }
        private async Task<IActionResult> DoUploadFile(MediaAlbum album, IEnumerable<IFormFile> files, AppDBContext dbContext)
        {
            try
            {
                string albumDir = Path.Combine(mediaPath, album.ShortName);
                if (!Directory.Exists(albumDir))
                    Directory.CreateDirectory(albumDir);

                List<MediaFile> newFiles = new List<MediaFile>();
                foreach (var file in files)
                {
                    string fileName = file.FileName.ToLower();
                    string fileExt = Path.GetExtension(fileName);

                    while (true)
                    {
                        fileName = Common.Random_Mix(6).ToLower() + fileExt;
                        string filePath = Path.Combine(albumDir, fileName);
                        if (!System.IO.File.Exists(filePath))
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                                await file.CopyToAsync(stream);

                            break;
                        }
                    }

                    MediaFile newFile = new MediaFile()
                    {
                        AlbumId = album.Id,
                        FileName = file.FileName.ToLower(),
                        FullPath = Path.Combine(album.ShortName, fileName),
                        FileSize = file.Length,
                        CreateTime = DateTime.Now,
                    };

                    newFiles.Add(newFile);
                    dbContext.MediaFiles.Add(newFile);
                }

                dbContext.SaveChanges();
                return new JsonResult(new FileUploadResult
                {
                    initialPreview = newFiles.Select(x => Path.Combine(mediaUrl, x.FullPath)).ToArray(),
                    initialPreviewConfig = newFiles.Select(x => new { key = x.Id, caption = x.FileName, size = x.FileSize, showDrag = false }).ToArray(),
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    

        //[HttpPost]
        //public async Task<IActionResult> FileUploadProduct(int? id, IEnumerable<IFormFile> files)
        //{
        //    ShopItem model = _dbContext.ShopItems.Find(id);
        //    if (model == null)
        //        return BadRequest();

        //    if (model.MediaAlbum == null)
        //    {
        //        try
        //        {
        //            model.MediaAlbum = new MediaAlbum()
        //            {
        //                UserId = 1,
        //                ShortName = string.Format("Item{0:D4}", model.Id),
        //                FullName = "Album " + model.Name,
        //                CreateTime = DateTime.Now,
        //            };

        //            _dbContext.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //    return await DoUploadFile(model.MediaAlbum, files, _dbContext);
        //}

        [HttpPost]
        public IActionResult FileRemove(int? key)
        {
            MediaFile model = _dbContext.MediaFiles.Find(key);
            if (model == null)
                return BadRequest("File not found!");

            try
            {
                _dbContext.MediaFiles.Remove(model);
                _dbContext.SaveChanges();

                string fileName = Path.Combine(mediaPath, model.FullPath);
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);

                return Json(new ModalFormResult() { Code = 1, Message = "File deleted!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }
        }
        private MediaAlbum GetDefaultAlbumBanner(int? id)
        {
            try
            {
                MediaAlbum album = _dbContext.MediaAlbums.FirstOrDefault(x => x.Id == id || x.ShortName == "Banner");
                if (album == null)
                {
                    album = new MediaAlbum()
                    {
                        UserId = 1,
                        ShortName = "Banner",
                        FullName = "Banner Album",
                        Description = "Album Banner",
                        CreateTime = DateTime.Now,
                    };

                    _dbContext.MediaAlbums.Add(album);
                    _dbContext.SaveChanges();
                }

                return album;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        private MediaAlbum GetDefaultAlbum(int? id)
        {
            try
            {
                MediaAlbum album = _dbContext.MediaAlbums.FirstOrDefault(x => x.Id == id || x.ShortName == "Default");
                if (album == null)
                {
                    album = new MediaAlbum()
                    {
                        UserId = 1,
                        ShortName = "Default",
                        FullName = "Default Album",
                        Description = "Album mặc định",
                        CreateTime = DateTime.Now,
                    };

                    _dbContext.MediaAlbums.Add(album);
                    _dbContext.SaveChanges();
                }

                return album;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
