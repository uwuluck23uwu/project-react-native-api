using LinqKit;
using System.Net;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace ProjectReactNative.Services
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Service(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public virtual async Task<ResponsePagination> GetAllAsync(
            int pageSize,
            int currentPage,
            string search,
            Expression<Func<T, object>>[]? includes
        )
        {
            var query = dbSet.AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            var props = typeof(T).GetProperties();

            if (!string.IsNullOrEmpty(search))
            {
                var filters = new[] { "Id", "Name" }
                    .Select(suffix => props.FirstOrDefault(p => p.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
                    .Where(p => p != null)
                    .Distinct()
                    .ToList();

                if (filters.Any())
                {
                    var predicate = filters
                        .Select(prop => (Expression<Func<T, bool>>)(e =>
                            EF.Functions.Like(EF.Property<string>(e, prop.Name), $"%{search}%")))
                        .Aggregate((a, b) => a.Or(b));

                    query = query.Where(predicate);
                }
            }

            var idProperty = props.FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));

            if (idProperty == null)
            {
                return new ResponsePagination(
                    statusCode: HttpStatusCode.BadRequest,
                    taskStatus: false,
                    message: "ไม่พบฟิลด์ที่ลงท้ายด้วย 'Id'",
                    pagin: new { CurrentPage = 0, PageSize = 0, TotalRows = 0, TotalPages = 0 },
                    data: new List<object>()
                );
            }

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            var items = await query
                .OrderByDescending(e => EF.Property<object>(e, idProperty.Name))
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ResponsePagination(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สำเร็จ",
                pagin: new
                {
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalRows = totalRecords,
                    TotalPages = totalPages
                },
                data: items.Cast<object>().ToList()
            );
        }


        public async Task<ResponseData> GetAsync(Expression<Func<T, bool>> id)
        {
            var query = dbSet.AsQueryable();

            var item = await query.FirstOrDefaultAsync(id);

            if (item == null)
            {
                return new ResponseData(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข้อมูล",
                    data: null
                );
            }

            return new ResponseData(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สำเร็จ",
                data: item
            );
        }

        public async Task<ResponseMessage> CreateAsync(T model)
        {
            await dbSet.AddAsync(model);
            await SaveAsync();

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "สร้างข้อมูลสำเร็จ"
            );
        }

        public async Task<ResponseMessage> UpdateAsync(T model)
        {
            dbSet.Attach(model);
            _db.Entry(model).State = EntityState.Modified;

            await SaveAsync();

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "อัปเดตข้อมูลสำเร็จ"
            );
        }

        public async Task<ResponseMessage> DeleteAsync(IEnumerable<string> ids)
        {
            var query = dbSet.AsQueryable();

            var idProperty = typeof(T).GetProperties().FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));

            if (idProperty == null)
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.BadRequest,
                    taskStatus: false,
                    message: "ไม่พบฟิลด์ที่ลงท้ายด้วย 'Id'"
                );
            }

            var entities = await query.Where(e => ids.Contains(EF.Property<string>(e, idProperty.Name))).ToListAsync();

            if (!entities.Any())
            {
                return new ResponseMessage(
                    statusCode: HttpStatusCode.NotFound,
                    taskStatus: false,
                    message: "ไม่พบข้อมูลที่ต้องการลบ"
                );
            }

            dbSet.RemoveRange(entities);
            await SaveAsync();

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "ลบข้อมูลสำเร็จ"
            );
        }

        public async Task<ResponseMessage> DeleteImagesAsync(IEnumerable<string> ids)
        {
            var images = await _db.Images
                .Where(i => ids.Contains(i.RefId))
                .ToListAsync();

            foreach (var img in images)
            {
                var filePath = Path.Combine("wwwroot", img.ImageUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            _db.Images.RemoveRange(images);

            var response = await DeleteAsync(ids);

            if (!response.TaskStatus)
            {
                return response;
            }

            return new ResponseMessage(
                statusCode: HttpStatusCode.OK,
                taskStatus: true,
                message: "ลบข้อมูลสำเร็จ"
            );
        }

        protected async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        protected async Task<string> GenerateRunningIdAsync(string idPropertyName, string prefixLetters)
        {
            string datePart = DateTime.UtcNow.ToString("yyMMdd");
            string prefix = $"{prefixLetters}{datePart}";

            var param = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(param, idPropertyName);

            var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            var prefixConstant = Expression.Constant(prefix, typeof(string));
            var startsWithCall = Expression.Call(property, startsWithMethod!, prefixConstant);

            var whereLambda = Expression.Lambda<Func<T, bool>>(startsWithCall, param);
            var query = dbSet.Where(whereLambda);

            var orderByLambda = Expression.Lambda<Func<T, string>>(property, param);
            var lastEntity = await query.OrderByDescending(orderByLambda).FirstOrDefaultAsync();

            string lastId = lastEntity?.GetType().GetProperty(idPropertyName)?.GetValue(lastEntity)?.ToString();
            int nextNumber = 1;

            if (!string.IsNullOrEmpty(lastId))
            {
                string numberPart = lastId.Substring(prefix.Length);
                int.TryParse(numberPart, out nextNumber);
                nextNumber++;
            }

            return $"{prefix}{nextNumber.ToString("D2")}";
        }
    }
}
