using OnlineBookstore.Bll.Dtos;

namespace OnlineBookstore.Bll.ServiceContracts;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAllAsync();
    Task<AuthorDto?> GetByIdAsync(int id);
    Task<int> AddAsync(AuthorCreateDto dto);
    Task UpdateAsync(int id, AuthorUpdateDto dto);
    Task DeleteAsync(int id);
}
