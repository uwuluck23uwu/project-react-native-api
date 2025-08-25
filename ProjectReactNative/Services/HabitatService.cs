using AutoMapper;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace ProjectReactNative.Services
{
    public class HabitatService : Service<Habitat>, IHabitatService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILocationService _locationService;

        public HabitatService(
            ApplicationDbContext db,
            IHubContext<SignalHub> hub,
            IMapper mapper,
            ILocationService locationService
        ) : base(db, hub)
        {
            _db = db;
            _mapper = mapper;
            _locationService = locationService;
        }

        public async Task<ResponsePagination> GetAllHabitats(int pageSize, int currentPage, string search)
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

            var models = baseResult.Data.Cast<Habitat>().ToList();
            var modelIds = models.Select(h => h.HabitatId).ToList();
            var locations = await _db.Locations
                .Where(l => modelIds.Contains(l.RefId))
                .ToListAsync();

            var result = _mapper.Map<List<HabitatDTO>>(models);
            var locByRef = locations.ToDictionary(l => l.RefId, l => l);

            foreach (var dto in result)
            {
                if (locByRef.TryGetValue(dto.HabitatId, out var loc))
                {
                    dto.Location = _mapper.Map<LocationDTO>(loc);
                }
            }

            return new ResponsePagination(
                statusCode: baseResult.StatusCode,
                taskStatus: baseResult.TaskStatus,
                message: baseResult.Message,
                pagin: baseResult.Pagin,
                data: result.Cast<object>().ToList()
            );
        }

        public async Task<ResponseMessage> CreateAsync(List<HabitatCreateDTO> createDTOs)
        {
            foreach (var createDTO in createDTOs)
            {
                Habitat model = _mapper.Map<Habitat>(createDTO);
                model.HabitatId = await GenerateRunningIdAsync("HabitatId", "HA");

                await CreateAsync(model);

                if (createDTO.Location != null)
                {
                    createDTO.Location.RefId = model.HabitatId;
                    createDTO.Location.Name = model.Name;
                    createDTO.Location.Description = model.Description;

                    await _locationService.CreateAsync(createDTO.Location);
                }
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข้อมูล Habitat หลายรายการสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(HabitatUpdateDTO updateDTO)
        {
            var model = await _dbSet.FirstOrDefaultAsync(x => x.HabitatId == updateDTO.HabitatId);

            if (model == null)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข้อมูล Habitat ที่ต้องการอัปเดต"
                );
            }

            _mapper.Map(updateDTO, model);

            if (updateDTO.Location != null)
            {
                await _locationService.UpdateAsync(updateDTO.Location);
            }

            return await UpdateAsync(model);
        }
    }
}
