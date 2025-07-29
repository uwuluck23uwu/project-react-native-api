using AutoMapper;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace ProjectReactNative.Services
{
    public class FacilityService : Service<Facility>, IFacilityService
    {
        private readonly IMapper _mapper;

        public FacilityService(ApplicationDbContext db, IMapper mapper) : base(db)
        {
            _mapper = mapper;
        }

        public async Task<ResponseMessage> CreateAsync(List<FacilityCreateDTO> createDTOs)
        {
            foreach (var createDTO in createDTOs)
            {
                Facility model = _mapper.Map<Facility>(createDTO);
                model.FacilityId = await GenerateRunningIdAsync("FacilityId", "FA");
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                await CreateAsync(model);
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข้อมูล Facility หลายรายการสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(FacilityUpdateDTO updateDTO)
        {
            var model = await dbSet.FirstOrDefaultAsync(x => x.FacilityId == updateDTO.FacilityId);

            if (model == null)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข้อมูล Facility ที่ต้องการอัปเดต"
                );
            }

            _mapper.Map(updateDTO, model);
            model.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(model);
        }
    }
}
