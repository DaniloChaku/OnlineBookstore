using OnlineBookstore.Bll.Dtos;
using OnlineBookstore.Bll.ServiceContracts;
using OnlineBookstore.Dal.Data.Repositories;
using OnlineBookstore.Dal.Entities;
using OnlineBookstore.Dal.Exceptions;

namespace OnlineBookstore.Bll.Services;

public class GenreService : IGenreService
{
    private readonly IRepository<Genre> _genreRepository;

    public GenreService(IRepository<Genre> genreRepository)
    {
        _genreRepository = genreRepository ?? throw new ArgumentNullException(nameof(genreRepository));
    }

    public async Task<List<GenreDto>> GetAllAsync()
    {
        var genres = await _genreRepository.GetAllAsync();
        return genres.ConvertAll(MapToDto);
    }

    public async Task<GenreDto?> GetByIdAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
            throw new NotFoundException($"Genre with ID {id} not found");

        return genre == null ? null : MapToDto(genre);
    }

    public async Task<int> AddAsync(GenreCreateDto dto)
    {
        var genre = new Genre { Name = dto.Name };
        await _genreRepository.AddAsync(genre);
        await _genreRepository.SaveChangesAsync();

        return genre.Id;
    }

    public async Task UpdateAsync(int id, GenreUpdateDto dto)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
            throw new NotFoundException($"Genre with ID {id} not found");

        genre.Name = dto.Name;
        _genreRepository.Update(genre);
        await _genreRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var genre = await _genreRepository.GetByIdAsync(id);
        if (genre == null)
            throw new NotFoundException($"Genre with ID {id} not found");

        _genreRepository.Delete(genre);
        await _genreRepository.SaveChangesAsync();
    }

    private static GenreDto MapToDto(Genre genre)
    {
        return new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name
        };
    }
}
