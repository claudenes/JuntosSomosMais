using JSM.Application.Dtos;

namespace JSM.Application.Interfaces
{
    public interface IUserTransformerService
    {
        public UserDto TransformCSV(dynamic rawUser);
        public UserDto TransformJSON(dynamic rawUser);
    }
}
