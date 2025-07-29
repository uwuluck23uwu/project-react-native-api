namespace ProjectReactNative.Services.IServices
{
    public interface IAuthenService
    {
        bool IsUniqueUser(string username);
        Task<ResponseData> GetAsync(string userId);
        Task<ResponseData> Login(LoginRequestDTO loginRequestDTO);
        Task<ResponseData> Register(RegisterationRequestDTO registerationRequestDTO);
        Task<ResponseData> RefreshAccessToken(TokenDTO tokenDTO);
    }
}
