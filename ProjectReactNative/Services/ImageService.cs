using System.Net;
using Microsoft.EntityFrameworkCore;

namespace ProjectReactNative.Services
{
    public class ImageService : Service<Image>, IImageService
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ImageService(
            ApplicationDbContext db,
            IWebHostEnvironment hostEnvironment
        ) : base(db)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ResponseMessage> CreateAsync(IFormFile file, string refId)
        {
            var uploadsRoot = Path.Combine(_hostEnvironment.WebRootPath, "images");

            if (!Directory.Exists(uploadsRoot))
            {
                Directory.CreateDirectory(uploadsRoot);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsRoot, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var model = new Image
            {
                ImageId = await GenerateRunningIdAsync("ImageId", "IM"),
                RefId = refId,
                ImageUrl = $"/images/{uniqueFileName}",
                UploadedDate = DateTime.UtcNow,
            };

            await CreateAsync(model);

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: $"สร้างรูปสำเร็จ"
            );
        }

        public async Task<ResponseMessage> DeleteAsync(List<string> Ids, string refId)
        {
            var oldImages = await _db.Images
                .Where(i => i.RefId == refId)
                .ToListAsync();

            var imagesToDelete = oldImages
                .Where(img => !Ids.Contains(img.ImageId))
                .ToList();

            if (imagesToDelete != null && imagesToDelete.Any())
            {
                foreach (var img in imagesToDelete)
                {
                    var physicalPath = Path.Combine(
                        _hostEnvironment.WebRootPath,
                        "images",
                        Path.GetFileName(img.ImageUrl)
                    );

                    if (File.Exists(physicalPath))
                    {
                        File.Delete(physicalPath);
                    }
                }

                _db.Images.RemoveRange(imagesToDelete);
                await SaveAsync();
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: $"อัปเดตรูปภาพสำเร็จ"
            );
        }
    }
}
