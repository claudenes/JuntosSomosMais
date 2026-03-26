using JSM.Application.Dtos;

namespace JSM.Application.Interfaces
{
    public interface IUserService
    {
            public IEnumerable<UserDto> GetUsers();
            public void Initialize(List<UserDto> users);
    }
}
