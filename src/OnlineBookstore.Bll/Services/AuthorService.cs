using OnlineBookstore.Bll.Dtos;
using OnlineBookstore.Bll.ServiceContracts;
using OnlineBookstore.Dal.Data.Repositories;
using OnlineBookstore.Dal.Entities;
using OnlineBookstore.Dal.Exceptions;

namespace OnlineBookstore.Bll.Services;

public class AuthorService : IAuthorService
{
    private readonly IRepository<Author> _authorRepository;

    public AuthorService(IRepository<Author> authorRepository)
    {
        _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
    }

    public async Task<List<AuthorDto>> GetAllAsync()
    {
        var authors = await _authorRepository.GetAllAsync();
        return authors.ConvertAll(MapToDto);
    }

    public async Task<AuthorDto> GetByIdAsync(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author == null)
            throw new NotFoundException($"Author with ID {id} not found");
        return MapToDto(author);
    }

    public async Task<int> AddAsync(AuthorCreateDto dto)
    {
        var author = new Author { Name = dto.Name };
        await _authorRepository.AddAsync(author);
        await _authorRepository.SaveChangesAsync();
        return author.Id;
    }

    public async Task UpdateAsync(int id, AuthorUpdateDto dto)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author == null)
            throw new NotFoundException($"Author with ID {id} not found");

        author.Name = dto.Name;
        _authorRepository.Update(author);
        await _authorRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author == null)
            throw new NotFoundException($"Author with ID {id} not found");

        _authorRepository.Delete(author);
        await _authorRepository.SaveChangesAsync();
    }

    private static AuthorDto MapToDto(Author author)
    {
        return new AuthorDto
        {
            Id = author.Id,
            Name = author.Name
        };
    }
}
