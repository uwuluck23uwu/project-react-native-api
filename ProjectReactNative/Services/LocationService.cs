using AutoMapper;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace ProjectReactNative.Services
{
    public class LocationService : Service<Location>, ILocationService
    {
        private readonly IMapper _mapper;

        public LocationService(
            ApplicationDbContext db,
            IHubContext<SignalHub> hub,
            IMapper mapper
        ) : base(db, hub)
        {
            _mapper = mapper;
        }

        public async Task<ResponsePagination> GetAllLocations(int pageSize, int currentPage, string search)
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

            var result = (baseResult.Data as IEnumerable<object>)
                ?.Cast<Location>()
                .Where(x => x.RefId == null || !x.RefId.StartsWith("AN"))
                .Cast<object>()
                .ToList() ?? new List<object>();

            return new ResponsePagination(
                statusCode: baseResult.StatusCode,
                taskStatus: baseResult.TaskStatus,
                message: baseResult.Message,
                pagin: baseResult.Pagin,
                data: result.Cast<object>().ToList()
            );
        }

        public async Task<ResponseMessage> CreateAsync(LocationCreateDTO createDTO)
        {
            Location model = _mapper.Map<Location>(createDTO);
            model.LocationId = await GenerateRunningIdAsync("LocationId", "LO");

            await CreateAsync(model);

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข้อมูล Location หลายรายการสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(LocationUpdateDTO updateDTO)
        {
            var model = await _dbSet.FirstOrDefaultAsync(x => x.RefId == updateDTO.RefId);

            if (model == null)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข้อมูล Location ที่ต้องการอัปเดต"
                );
            }

            _mapper.Map(updateDTO, model);

            await UpdateAsync(model);

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: $"แก้ไขข้อมูลสำเร็จ"
            );
        }
    }
}
