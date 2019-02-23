using LoadingProductShared.Data;
using LoadingProductShared.Helpers;
using LoadingProductWeb.Admin.Models;
using LoadingProductWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

            var filterQuery = _dbContext.MediaFiles.Where(x => model.Search == null || x.FileName.Contains(model.Search) || x.MediaAlbum.FullName.Contains(model.Search));
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

        [HttpPost]
        public async Task<IActionResult> FileUploadCrop(FileUploadModel model)
        {
            MediaAlbum album = GetDefaultAlbum(model.Id);
            if (album == null)
                return BadRequest("Could not create Default Album!");

            var fileData = model.FileData;
            var pos = fileData.IndexOf(";base64,");
            if (pos <= 0)
                return BadRequest("Invalid FileData");

            fileData = fileData.Substring(pos + 8);
            var fileBytes = Convert.FromBase64String(fileData);

            return await DoUploadFileCrop(album, model.FileName, fileBytes, _dbContext);
        }

        private async Task<IActionResult> DoUploadFileCrop(MediaAlbum album, string fileName, byte[] fileData, AppDBContext dbContext)
        {
            try
            {
                string albumDir = Path.Combine(mediaPath, album.ShortName);
                if (!Directory.Exists(albumDir))
                    Directory.CreateDirectory(albumDir);

                string fileExt = string.IsNullOrEmpty(fileName) ? ".jpg" : Path.GetExtension(fileName);

                string newName = "";
                while (true)
                {
                    newName = Common.Random_Mix(6).ToLower() + fileExt;
                    string filePath = Path.Combine(albumDir, newName);
                    if (!System.IO.File.Exists(filePath))
                    {
                        await System.IO.File.WriteAllBytesAsync(filePath, fileData);
                        break;
                    }
                }

                MediaFile newFile = new MediaFile()
                {
                    AlbumId = album.Id,
                    FullPath = Path.Combine(album.ShortName, newName),
                    FileSize = fileData.Length,
                    CreateTime = DateTime.Now
                };

                dbContext.MediaFiles.Add(newFile);
                dbContext.SaveChanges();

                var newFiles = new List<MediaFile> { newFile };
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
        //public async Task<IActionResult> FileUploadBlog(int? id, IEnumerable<IFormFile> files)
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
        //                ShortName = string.Format("Blog{0:D4}", model.Id),
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
            catch
            {
                return null;
            }
        }
    }
}
