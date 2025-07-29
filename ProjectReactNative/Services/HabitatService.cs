using AutoMapper;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace ProjectReactNative.Services
{
    public class HabitatService : Service<Habitat>, IHabitatService
    {
        private readonly IMapper _mapper;

        public HabitatService(ApplicationDbContext db, IMapper mapper) : base(db)
        {
            _mapper = mapper;
        }

        public async Task<ResponseMessage> CreateAsync(List<HabitatCreateDTO> createDTOs)
        {
            foreach (var createDTO in createDTOs)
            {
                Habitat model = _mapper.Map<Habitat>(createDTO);
                model.HabitatId = await GenerateRunningIdAsync("HabitatId", "HA");

                await CreateAsync(model);
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข้อมูล Habitat หลายรายการสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(HabitatUpdateDTO updateDTO)
        {
            var model = await dbSet.FirstOrDefaultAsync(x => x.HabitatId == updateDTO.HabitatId);

            if (model == null)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข้อมูล Habitat ที่ต้องการอัปเดต"
                );
            }

            _mapper.Map(updateDTO, model);

            return await UpdateAsync(model);
        }
    }
}
