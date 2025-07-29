using AutoMapper;
using System.Net;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.Models.Data;

namespace ProjectReactNative.Services
{
    public class NewsService : Service<News>, INewsService
    {
        ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public NewsService(ApplicationDbContext db, IMapper mapper, IImageService imageServic) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _imageService = imageServic;
        }

        public async Task<ResponsePagination> GetAllNews(int pageSize, int currentPage, string search)
        {
            var baseResult = await GetAllAsync(
                pageSize,
                currentPage,
                search,
                null
            );

            if (!baseResult.TaskStatus)
            {
                return baseResult;
            }

            var newsList = baseResult.Data.Cast<News>().ToList();
            var newsIds = newsList.Select(n => n.NewsId).ToList();
            var images = await _db.Images
                .Where(i => newsIds.Contains(i.RefId))
                .ToListAsync();

            var result = _mapper.Map<List<NewsDTO>>(newsList);

            foreach (var dto in result)
            {
                dto.Images = images
                    .Where(img => img.RefId == dto.NewsId)
                    .Select(img => new ImageDTO
                    {
                        ImageId = img.ImageId,
                        RefId = img.RefId,
                        ImageUrl = img.ImageUrl,
                        UploadedDate = img.UploadedDate
                    })
                    .ToList();
            }

            return new ResponsePagination(
                statusCode: baseResult.StatusCode,
                taskStatus: baseResult.TaskStatus,
                message: baseResult.Message,
                pagin: baseResult.Pagin,
                data: result.Cast<object>().ToList()
            );
        }

        public async Task<ResponseMessage> CreateAsync(List<NewsCreateDTO> createDTOs)
        {
            foreach (var createDTO in createDTOs)
            {
                News model = _mapper.Map<News>(createDTO);
                model.NewsId = await GenerateRunningIdAsync("NewsId", "NE");
                model.PublishedDate = DateTime.UtcNow;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                if (createDTO.Images != null)
                {
                    foreach (var item in createDTO.Images)
                    {
                        await _imageService.CreateAsync(item, model.NewsId);
                    }
                }

                await CreateAsync(model);
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข่าวหลายรายการสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(NewsUpdateDTO updateDTO)
        {
            var model = await dbSet.FirstOrDefaultAsync(x => x.NewsId == updateDTO.NewsId);

            if (model == null)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข่าวที่ต้องการอัปเดต"
                );
            }

            _mapper.Map(updateDTO, model);
            model.UpdatedAt = DateTime.UtcNow;

            if (updateDTO.ImageIds != null)
            {
                await _imageService.DeleteAsync(updateDTO.ImageIds, updateDTO.NewsId);
            }
            if (updateDTO.Images != null)
            {
                foreach (var file in updateDTO.Images)
                {
                    await _imageService.CreateAsync(file, model.NewsId);
                }
            }

            return await UpdateAsync(model);
        }
    }
}
