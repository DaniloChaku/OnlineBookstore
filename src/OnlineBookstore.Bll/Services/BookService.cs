using OnlineBookstore.Bll.Dtos;
using OnlineBookstore.Bll.ServiceContracts;
using OnlineBookstore.Dal.Data.Repositories;
using OnlineBookstore.Dal.Entities;
using OnlineBookstore.Dal.Exceptions;

namespace OnlineBookstore.Bll.Services;

public class BookService : IBookService
{
    private readonly IRepository<Book> _bookRepository;

    public BookService(IRepository<Book> bookRepository)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
    }

    public async Task<List<BookDto>> GetAllAsync()
    {
        var books = await _bookRepository.GetAllAsync(b => b.Author, b => b.Genre);
        return books.ConvertAll(MapToDto);
    }

    public async Task<BookDto> GetByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id, b => b.Author, b => b.Genre);
        if (book == null)
            throw new NotFoundException($"Book with ID {id} not found");
        return MapToDto(book);
    }

    public async Task<List<BookDto>> SearchAsync(string? title, string? author, string? genre)
    {
        var books = await _bookRepository.FindAsync(b =>
            (string.IsNullOrEmpty(title) || b.Title.Contains(title)) &&
            (string.IsNullOrEmpty(author) || b.Author.Name.Contains(author)) &&
            (string.IsNullOrEmpty(genre) || b.Genre.Name.Contains(genre)),
            b => b.Author, b => b.Genre
        );
        return books.ConvertAll(MapToDto);
    }

    public async Task<int> AddAsync(BookCreateDto bookDto)
    {
        var book = new Book
        {
            Title = bookDto.Title,
            AuthorId = bookDto.AuthorId,
            GenreId = bookDto.GenreId,
            Price = bookDto.Price,
            QuantityAvailable = bookDto.QuantityAvailable
        };
        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();
        return book.Id;
    }

    public async Task UpdateAsync(int id, BookUpdateDto bookDto)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            throw new NotFoundException($"Book with ID {id} not found");

        book.Title = bookDto.Title;
        book.AuthorId = bookDto.AuthorId;
        book.GenreId = bookDto.GenreId;
        book.Price = bookDto.Price;
        book.QuantityAvailable = bookDto.QuantityAvailable;
        _bookRepository.Update(book);
        await _bookRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            throw new NotFoundException($"Book with ID {id} not found");

        _bookRepository.Delete(book);
        await _bookRepository.SaveChangesAsync();
    }

    private static BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            AuthorId = book.AuthorId,
            AuthorName = book.Author?.Name ?? string.Empty,
            GenreId = book.GenreId,
            GenreName = book.Genre?.Name ?? string.Empty,
            Price = book.Price,
            QuantityAvailable = book.QuantityAvailable
        };
    }
}