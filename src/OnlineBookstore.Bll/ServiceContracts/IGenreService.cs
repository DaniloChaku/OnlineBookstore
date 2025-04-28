using OnlineBookstore.Bll.Dtos;

namespace OnlineBookstore.Bll.ServiceContracts;

public interface IGenreService
{
    Task<List<GenreDto>> GetAllAsync();
    Task<GenreDto> GetByIdAsync(int id);
    Task<int> AddAsync(GenreCreateDto dto);
    Task UpdateAsync(int id, GenreUpdateDto dto);
    Task DeleteAsync(int id);
}
