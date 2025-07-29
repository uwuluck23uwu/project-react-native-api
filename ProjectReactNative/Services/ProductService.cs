using AutoMapper;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace ProjectReactNative.Services
{
    public class ProductService : Service<Product>, IProductService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IQrScanLogService _qrScanLogService;

        public ProductService(
            ApplicationDbContext db,
            IMapper mapper,
            IImageService imageService,
            IQrScanLogService qrScanLogService
        ) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _imageService = imageService;
            _qrScanLogService = qrScanLogService;
        }

        public async Task<ResponsePagination> GetAllProducts(int pageSize, int currentPage, string search)
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

            var products = baseResult.Data.Cast<Product>().ToList();
            var productIds = products.Select(a => a.ProductId).ToList();
            var images = await _db.Images
                .Where(i => productIds.Contains(i.RefId))
                .ToListAsync();

            var result = _mapper.Map<List<ProductDTO>>(products);

            foreach (var dto in result)
            {
                dto.Images = images
                    .Where(img => img.RefId == dto.ProductId)
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


        public async Task<ResponseMessage> CreateAsync(List<ProductCreateDTO> createDTOs)
        {
            foreach (var createDTO in createDTOs)
            {
                Product model = _mapper.Map<Product>(createDTO);
                model.ProductId = await GenerateRunningIdAsync("ProductId", "PR");
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                if (createDTO.Images != null)
                {
                    foreach (var item in createDTO.Images)
                    {
                        await _imageService.CreateAsync(item, model.ProductId);
                    }
                }
                if (createDTO.ScanId != null)
                {
                    await _qrScanLogService.CreateAsync(model.ProductId);
                }

                await CreateAsync(model);
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข้อมูล Product หลายรายการสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(ProductUpdateDTO updateDTO)
        {
            var model = await dbSet.FirstOrDefaultAsync(x => x.ProductId == updateDTO.ProductId);

            if (model == null)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข้อมูล Product ที่ต้องการอัปเดต"
                );
            }

            _mapper.Map(updateDTO, model);
            model.UpdatedAt = DateTime.UtcNow;

            if (updateDTO.ImageIds != null)
            {
                await _imageService.DeleteAsync(updateDTO.ImageIds, updateDTO.ProductId);
            }
            if (updateDTO.Images != null)
            {
                foreach (var file in updateDTO.Images)
                {
                    await _imageService.CreateAsync(file, model.ProductId);
                }
            }

            return await UpdateAsync(model);
        }
    }
}
