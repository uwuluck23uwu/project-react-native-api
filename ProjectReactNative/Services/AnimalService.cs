using AutoMapper;
using System.Net;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ProjectReactNative.Services
{
    public class AnimalService : Service<Animal>, IAnimalService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public AnimalService(
            ApplicationDbContext db,
            IMapper mapper,
            IImageService imageService
        ) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<ResponsePagination> GetAllAnimals(int pageSize, int currentPage, string search)
        {
            var baseResult = await GetAllAsync(
                pageSize,
                currentPage,
                search,
                new Expression<Func<Animal, object>>[] { x => x.Habitat }
            );

            if (!baseResult.TaskStatus)
            {
                return baseResult;
            }

            var animals = baseResult.Data.Cast<Animal>().ToList();
            var animalIds = animals.Select(a => a.AnimalId).ToList();
            var images = await _db.Images
                .Where(i => animalIds.Contains(i.RefId))
                .ToListAsync();

            var result = _mapper.Map<List<AnimalDTO>>(animals);

            foreach (var dto in result)
            {
                dto.Images = images
                    .Where(img => img.RefId == dto.AnimalId)
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

        public async Task<ResponseMessage> CreateAsync(List<AnimalCreateDTO> createDTOs)
        {
            foreach (var createDTO in createDTOs)
            {
                Animal model = _mapper.Map<Animal>(createDTO);
                model.AnimalId = await GenerateRunningIdAsync("AnimalId", "AN");
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                if (createDTO.Images != null)
                {
                    foreach (var item in createDTO.Images)
                    {
                        await _imageService.CreateAsync(item, model.AnimalId);
                    }
                }

                await CreateAsync(model);
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข้อมูลสัตว์หลายรายการสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(AnimalUpdateDTO updateDTO)
        {
            var model = await dbSet.FirstOrDefaultAsync(x => x.AnimalId == updateDTO.AnimalId);

            if (model == null)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข้อมูลสัตว์ที่ต้องการอัปเดต"
                );
            }

            _mapper.Map(updateDTO, model);
            model.UpdatedAt = DateTime.UtcNow;

            if (updateDTO.ImageIds != null)
            {
                await _imageService.DeleteAsync(updateDTO.ImageIds, updateDTO.AnimalId);
            }
            if (updateDTO.Images != null)
            {
                foreach (var file in updateDTO.Images)
                {
                    await _imageService.CreateAsync(file, model.AnimalId);
                }
            }

            await UpdateAsync(model);

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: $"แก้ไขข้อมูลสำเร็จ"
            );
        }
    }
}
