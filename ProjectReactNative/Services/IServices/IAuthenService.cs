namespace ProjectReactNative.Services.IServices
{
    public interface IAuthenService : IService<User>
    {
        bool IsUniqueUser(string username);
        Task<ResponseData> GetAsync(string userId);
        Task<ResponseData> Login(LoginRequestDTO loginRequestDTO);
        Task<ResponseData> Register(RegisterationRequestDTO registerationRequestDTO);
        Task<ResponseMessage> UpdateAsync(UserDTO updateDTO);
        Task<ResponseData> RefreshAccessToken(TokenDTO tokenDTO);
    }
}
