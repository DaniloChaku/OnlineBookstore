using OnlineBookstore.Bll.Dtos;

namespace OnlineBookstore.Bll.ServiceContracts;

public interface IBookService
{
    Task<List<BookDto>> GetAllAsync();
    Task<BookDto> GetByIdAsync(int id);
    Task<List<BookDto>> SearchAsync(string? title, string? author, string? genre);
    Task<int> AddAsync(BookCreateDto bookDto);
    Task UpdateAsync(int id, BookUpdateDto bookDto);
    Task DeleteAsync(int id);
}
