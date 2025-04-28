using System.ComponentModel.DataAnnotations;
using OnlineBookstore.Dal.Constants;

namespace OnlineBookstore.Bll.Dtos;

public class GenreCreateDto
{
    [Required]
    [StringLength(ValidationConstants.GenreNameMaxLength)]
    public string Name { get; set; } = string.Empty;
}