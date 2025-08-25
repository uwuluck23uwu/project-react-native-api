using AutoMapper;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace ProjectReactNative.Services
{
    public class TicketService : Service<Ticket>, ITicketService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public TicketService(
            ApplicationDbContext db,
            IHubContext<SignalHub> hub,
            IMapper mapper,
            IImageService imageService
        ) : base(db, hub)
        {
            _db = db;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<ResponsePagination> GetAllTickets(int pageSize, int currentPage, string search)
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

            var tickets = baseResult.Data.Cast<Ticket>().ToList();
            var ticketIds = tickets.Select(a => a.TicketId).ToList();
            var images = await _db.Images
                .Where(i => ticketIds.Contains(i.RefId))
                .ToListAsync();

            var result = _mapper.Map<List<TicketDTO>>(tickets);

            foreach (var dto in result)
            {
                dto.Images = images
                    .Where(img => img.RefId == dto.TicketId)
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

        public async Task<ResponseMessage> CreateAsync(List<TicketCreateDTO> createDTOs)
        {
            foreach (var createDTO in createDTOs)
            {
                Ticket model = _mapper.Map<Ticket>(createDTO);
                model.TicketId = await GenerateRunningIdAsync("TicketId", "TI");
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                if (createDTO.Images != null)
                {
                    foreach (var item in createDTO.Images)
                    {
                        await _imageService.CreateAsync(item, model.TicketId);
                    }
                }

                await CreateAsync(model);
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข้อมูล Ticket หลายรายการสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(TicketUpdateDTO updateDTO)
        {
            var model = await _dbSet.FirstOrDefaultAsync(x => x.TicketId == updateDTO.TicketId);

            if (model == null)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข้อมูล Ticket ที่ต้องการอัปเดต"
                );
            }

            _mapper.Map(updateDTO, model);
            model.UpdatedAt = DateTime.UtcNow;

            if (updateDTO.ImageIds != null)
            {
                await _imageService.DeleteAsync(updateDTO.ImageIds, updateDTO.TicketId);
            }
            if (updateDTO.Images != null)
            {
                foreach (var file in updateDTO.Images)
                {
                    await _imageService.CreateAsync(file, model.TicketId);
                }
            }

            return await UpdateAsync(model);
        }
    }
}
