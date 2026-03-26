using JSM.Application.Dtos;
using JSM.Application.Interfaces;

namespace JSM.Application.Services
{
    public class UserService : IUserService
    {
        private List<UserDto> _users = new();

        public void Initialize(List<UserDto> users) => _users = users;

        public IEnumerable<UserDto> GetUsers() => _users;
    }
}
