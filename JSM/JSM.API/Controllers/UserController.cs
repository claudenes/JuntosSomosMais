using JSM.Application.Dtos;
using JSM.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace JSM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet(Name = "GetUsers")]
        public async Task<List<UserDto>> GetUsers(string? region, string? type, int? pageNumber, int? pageSize, UserService service)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var filtered =  service.GetUsers()
                                    .Where(u => string.IsNullOrEmpty(region) || u.Location.Region.Equals(region, StringComparison.OrdinalIgnoreCase))
                                    .Where(u => string.IsNullOrEmpty(type) || u.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                                    .ToList();

            var total = filtered.Count;
            var paged = filtered
                        .Skip((page - 1) * size)
                        .Take(size)
                        .ToList();

            return filtered;

        }
       
    }
}
