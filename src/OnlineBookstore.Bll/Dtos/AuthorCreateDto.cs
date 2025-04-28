using System.ComponentModel.DataAnnotations;
using OnlineBookstore.Dal.Constants;

namespace OnlineBookstore.Bll.Dtos;

public class AuthorCreateDto
{
    [Required]
    [StringLength(ValidationConstants.AuthorNameMaxLength)]
    public string Name { get; set; } = string.Empty;
}
