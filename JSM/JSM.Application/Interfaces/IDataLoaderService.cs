using JSM.Application.Dtos;
using System.Text.Json;

namespace JSM.Application.Interfaces
{
    public interface IDataLoaderService 
    {
       public Task<List<UserDto>> LoadUsersAsync();

    }
}
