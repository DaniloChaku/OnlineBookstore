using System.ComponentModel.DataAnnotations;
using OnlineBookstore.Dal.Constants;

namespace OnlineBookstore.Bll.Dtos;

public class BookCreateDto
{
    [Required]
    [StringLength(ValidationConstants.BookTitleMaxLength)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int AuthorId { get; set; }

    [Required]
    public int GenreId { get; set; }

    [Range((double)ValidationConstants.BookPriceMin, (double)ValidationConstants.BookPriceMax)]
    public decimal Price { get; set; }

    [Range(ValidationConstants.BookQuantityMin, ValidationConstants.BookQuantityMax)]
    public int QuantityAvailable { get; set; }
}
