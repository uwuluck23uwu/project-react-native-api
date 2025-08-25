using AutoMapper;
using System.Net;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace ProjectReactNative.Services
{
    public class AnimalService : Service<Animal>, IAnimalService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocationService _locationService;
        private readonly IImageService _imageService;

        public AnimalService(
            ApplicationDbContext db,
            IHubContext<SignalHub> hub,
            IMapper mapper,
            ILocationService locationService,
            IImageService imageService
        ) : base(db, hub)
        {
            _db = db;
            _mapper = mapper;
            _locationService = locationService;
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

            var models = baseResult.Data.Cast<Animal>().ToList();
            var modelIds = models.Select(a => a.AnimalId).ToList();
            var locations = await _db.Locations
                .Where(l => modelIds.Contains(l.RefId))
                .ToListAsync();
            var images = await _db.Images
                .Where(i => modelIds.Contains(i.RefId))
                .ToListAsync();

            var result = _mapper.Map<List<AnimalDTO>>(models);
            var locByRef = locations.ToDictionary(l => l.RefId, l => l);

            foreach (var dto in result)
            {
                if (locByRef.TryGetValue(dto.AnimalId, out var loc))
                {
                    dto.Location = _mapper.Map<LocationDTO>(loc);
                }

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

                await CreateAsync(model);

                if (createDTO.Location != null)
                {
                    createDTO.Location.RefId = model.AnimalId;
                    createDTO.Location.Name = model.Name;
                    createDTO.Location.Description = model.Description;

                    await _locationService.CreateAsync(createDTO.Location);
                }
                if (createDTO.Images != null)
                {
                    foreach (var item in createDTO.Images)
                    {
                        await _imageService.CreateAsync(item, model.AnimalId);
                    }
                }
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข้อมูลสัตว์หลายรายการสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(AnimalUpdateDTO updateDTO)
        {
            var model = await _dbSet.FirstOrDefaultAsync(x => x.AnimalId == updateDTO.AnimalId);

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

            if (updateDTO.Location != null)
            {
                if (updateDTO.Location.LocationId == null)
                {
                    updateDTO.Location.RefId = model.AnimalId;
                    updateDTO.Location.Name = model.Name;
                    updateDTO.Location.Description = model.Description;

                    LocationCreateDTO createDTO = _mapper.Map<LocationCreateDTO>(updateDTO.Location);

                    await _locationService.CreateAsync(createDTO);
                }
                else
                {
                    await _locationService.UpdateAsync(updateDTO.Location);
                }
            }
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

            return await UpdateAsync(model);
        }
    }
}
