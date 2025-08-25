using AutoMapper;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace ProjectReactNative.Services
{
    public class EventService : Service<Event>, IEventService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocationService _locationService;
        private readonly IImageService _imageService;

        public EventService(
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

        public async Task<ResponsePagination> GetAllEvents(int pageSize, int currentPage, string search)
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

            var models = baseResult.Data.Cast<Event>().ToList();
            var modelIds = models.Select(a => a.EventId).ToList();
            var locations = await _db.Locations
                .Where(l => modelIds.Contains(l.RefId))
                .ToListAsync();
            var images = await _db.Images
                .Where(i => modelIds.Contains(i.RefId))
                .ToListAsync();

            var result = _mapper.Map<List<EventDTO>>(models);
            var locByRef = locations.ToDictionary(l => l.RefId, l => l);

            foreach (var dto in result)
            {

                if (dto.Status != "4")
                {
                    dto.Status = CalculateEventStatus(dto.EventDate, dto.StartTime, dto.EndTime, dto.Status);
                }
                if (locByRef.TryGetValue(dto.EventId, out var loc))
                {
                    dto.Location = _mapper.Map<LocationDTO>(loc);
                }

                dto.Images = images
                    .Where(img => img.RefId == dto.EventId)
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

        public async Task<ResponseMessage> CreateAsync(List<EventCreateDTO> createDTOs)
        {
            foreach (var createDTO in createDTOs)
            {
                Event model = _mapper.Map<Event>(createDTO);
                model.EventId = await GenerateRunningIdAsync("EventId", "EV");
                model.Status = "1";
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                await CreateAsync(model);

                if (createDTO.Location != null)
                {
                    createDTO.Location.RefId = model.EventId;
                    createDTO.Location.Name = model.Title;
                    createDTO.Location.Description = model.Description;

                    await _locationService.CreateAsync(createDTO.Location);
                }
                if (createDTO.Images != null)
                {
                    foreach (var item in createDTO.Images)
                    {
                        await _imageService.CreateAsync(item, model.EventId);
                    }
                }
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข้อมูล Event หลายรายการสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(EventUpdateDTO updateDTO)
        {
            var model = await _dbSet.FirstOrDefaultAsync(x => x.EventId == updateDTO.EventId);

            if (model == null)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข้อมูล Event ที่ต้องการอัปเดต"
                );
            }

            _mapper.Map(updateDTO, model);
            model.Status = "5";
            model.UpdatedAt = DateTime.UtcNow;

            if (updateDTO.Location != null)
            {
                if (updateDTO.Location.LocationId == null)
                {
                    updateDTO.Location.RefId = model.EventId;
                    updateDTO.Location.Name = model.Title;
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
                await _imageService.DeleteAsync(updateDTO.ImageIds, updateDTO.EventId);
            }
            if (updateDTO.Images != null)
            {
                foreach (var file in updateDTO.Images)
                {
                    await _imageService.CreateAsync(file, model.EventId);
                }
            }

            return await UpdateAsync(model);
        }

        private string CalculateEventStatus(DateTime? eventDate, TimeSpan? startTime, TimeSpan? endTime, string status)
        {
            if (eventDate == null || startTime == null || endTime == null)
                return "1";

            var now = DateTime.Now;
            var startDateTime = eventDate.Value.Date + startTime.Value;
            var endDateTime = eventDate.Value.Date + endTime.Value;

            if (now < startDateTime && status != "5")
                return "5";
            if (now < startDateTime)
                return "1";
            if (now >= startDateTime && now <= endDateTime)
                return "2";
            if (now > endDateTime)
                return "3";

            return "1";
        }
    }
}
