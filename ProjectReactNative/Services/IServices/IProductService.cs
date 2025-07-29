namespace ProjectReactNative.Services.IServices
{
    public interface IProductService : IService<Product>
    {
        Task<ResponsePagination> GetAllProducts(int pageSize, int currentPage, string search);
        Task<ResponseMessage> CreateAsync(List<ProductCreateDTO> createDTOs);
        Task<ResponseMessage> UpdateAsync(ProductUpdateDTO updateDTO);
    }
}
