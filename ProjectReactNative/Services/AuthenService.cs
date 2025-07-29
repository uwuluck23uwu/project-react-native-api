using AutoMapper;
using System.Net;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ProjectReactNative.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secretKey;
        private readonly int _accessTokenMinutes;
        private readonly int _refreshTokenMinutes;

        public AuthenService(
            ApplicationDbContext db,
            IMapper mapper,
            IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _issuer = configuration.GetValue<string>("JwtSettings:Issuer");
            _audience = configuration.GetValue<string>("JwtSettings:Audience");
            _secretKey = configuration.GetValue<string>("Settings:SecretProgram");
            _accessTokenMinutes = configuration.GetValue<int>("JwtSettings:AccessTokenMinutes");
            _refreshTokenMinutes = configuration.GetValue<int>("JwtSettings:RefreshTokenMinutes");
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.Users.FirstOrDefault(x => x.Name == username);
            return user == null;
        }

        public async Task<ResponseData> GetAsync(string id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == id);

            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            var accessToken = await GetAccessToken(user, jwtTokenId);
            var refreshToken = await CreateNewRefreshToken(user.UserId, jwtTokenId);

            return new ResponseData(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "ส่งข้อมูลสำเร็จ",
                data: new TokenDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            );
        }

        public async Task<ResponseData> Login(LoginRequestDTO loginRequestDTO)
        {
            if (string.IsNullOrWhiteSpace(loginRequestDTO.Identifier))
            {
                return new ResponseData(
                    statusCode: HttpStatusCode.BadRequest,
                    taskStatus: false,
                    message: "กรุณาระบุชื่อผู้ใช้, อีเมล หรือ เบอร์โทร"
                );
            }

            var name = loginRequestDTO.Identifier.Trim().ToLower();

            var user = await _db.Users.FirstOrDefaultAsync(u =>
                    u.Name.ToLower() == name ||
                    u.Email.ToLower() == name);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequestDTO.Password, user.Password))
            {
                return new ResponseData(
                    statusCode: HttpStatusCode.Unauthorized,
                    taskStatus: false,
                    message: "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง"
                );
            }

            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            var accessToken = await GetAccessToken(user, jwtTokenId);
            var refreshToken = await CreateNewRefreshToken(user.UserId, jwtTokenId);

            return new ResponseData(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "เข้าสู่ระบบสำเร็จ",
                data: new TokenDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            );
        }

        public async Task<ResponseData> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            var exists = await _db.Users.AnyAsync(u => u.Name.ToLower() == registerationRequestDTO.Name.ToLower());
            if (exists)
            {
                return new ResponseData(
                    statusCode: HttpStatusCode.Conflict,
                    taskStatus: false,
                    message: "ชื่อนี้ถูกใช้งานแล้ว"
                );
            }

            var user = _mapper.Map<User>(registerationRequestDTO);
            user.UserId = await GenerateUniqueUserIdAsync();
            user.Role = "user";
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.Password = BCrypt.Net.BCrypt.HashPassword(registerationRequestDTO.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var jwtTokenId = $"JTI{Guid.NewGuid()}";
            var accessToken = await GetAccessToken(user, jwtTokenId);
            var refreshToken = await CreateNewRefreshToken(user.UserId, jwtTokenId);

            return new ResponseData(
                statusCode: HttpStatusCode.Created,
                taskStatus: true,
                message: "ลงทะเบียนสำเร็จ",
                data: new TokenDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            );
        }

        public async Task<ResponseData> RefreshAccessToken(TokenDTO tokenDTO)
        {
            var existingRefreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(u => u.Token == tokenDTO.RefreshToken);
            if (existingRefreshToken == null)
            {
                return new ResponseData(
                    statusCode: HttpStatusCode.BadRequest,
                    taskStatus: false,
                    message: "Refresh token ไม่ถูกต้องหรือหมดอายุ"
                );
            }

            #region A ตรวจสอบความถูกต้องของโทเคนที่ส่งมาเข้า
            var accessTokenData = GetAccessTokenData(tokenDTO.AccessToken);
            if (!accessTokenData.isSuccessful ||
                accessTokenData.userId != existingRefreshToken.UserId ||
                accessTokenData.tokenId != existingRefreshToken.JwtTokenId)
            {
                existingRefreshToken.IsValid = false;
                _db.SaveChanges();

                return new ResponseData(
                    statusCode: HttpStatusCode.Unauthorized,
                    taskStatus: false,
                    message: "Access token ไม่ถูกต้อง"
                );
            }

            if (existingRefreshToken.IsValid != true)
            {
                _db.RefreshTokens.Where(u =>
                    u.UserId == existingRefreshToken.UserId &&
                    u.JwtTokenId == existingRefreshToken.JwtTokenId)
                .ExecuteUpdate(u => u.SetProperty(rt => rt.IsValid, false));

                return new ResponseData(
                    statusCode: HttpStatusCode.Unauthorized,
                    taskStatus: false,
                    message: "Refresh token ถูกปิดใช้งานแล้ว"
                );
            }

            if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                existingRefreshToken.IsValid = false;
                _db.SaveChanges();

                return new ResponseData(
                    statusCode: HttpStatusCode.Unauthorized,
                    taskStatus: false,
                    message: "Refresh token หมดอายุแล้ว"
                );
            }
            #endregion

            #region B สร้าง accessToken refreshToken ให้ใหม่
            var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);

            existingRefreshToken.IsValid = false;
            _db.SaveChanges();

            var applicationUser = _db.Users.FirstOrDefault(u => u.UserId == existingRefreshToken.UserId);
            if (applicationUser == null)
            {
                return new ResponseData(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบผู้ใช้งาน"
                );
            }

            var newAccessToken = await GetAccessToken(applicationUser, existingRefreshToken.JwtTokenId);

            return new ResponseData(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้าง access token สำเร็จ",
                new TokenDTO
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                }
            );
            #endregion
        }

        protected async Task<string> GetAccessToken(User user, string jwtTokenId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("email",     user.Email      ?? string.Empty),
                new Claim("phone",     user.Phone      ?? string.Empty),
                new Claim("image_url", user.ImageUrl   ?? string.Empty),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_accessTokenMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            // 4. สร้างและเขียน token ออกมาเป็น string
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        protected async Task<string> CreateNewRefreshToken(string userId, string tokenId)
        {
            RefreshToken refreshToken = new()
            {
                IsValid = true,
                UserId = userId,
                JwtTokenId = tokenId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_refreshTokenMinutes),
                Token = Guid.NewGuid() + "-" + Guid.NewGuid(),
            };

            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
            return refreshToken.Token;
        }

        protected async Task<string> GenerateUniqueUserIdAsync()
        {
            string newUserId;
            bool exists;

            do
            {
                newUserId = Guid.NewGuid().ToString();
                exists = await _db.Users.AnyAsync(u => u.UserId == newUserId);
            } while (exists);

            return newUserId;
        }

        protected (bool isSuccessful, string userId, string tokenId) GetAccessTokenData(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);
                var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Jti).Value;
                var userId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value;
                return (true, userId, jwtTokenId);
            }
            catch (Exception ex)
            {
                return (false, string.Empty, string.Empty);
            }
        }
    }
}
