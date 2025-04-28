namespace OnlineBookstore.Dal.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
    public decimal Price { get; set; }
    public int QuantityAvailable { get; set; }
}
