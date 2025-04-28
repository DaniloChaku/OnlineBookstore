using System.ComponentModel.DataAnnotations;
using OnlineBookstore.Dal.Constants;

namespace OnlineBookstore.Bll.Dtos;

public class AuthorUpdateDto
{
    [Required]
    [StringLength(ValidationConstants.AuthorNameMaxLength)]
    public string Name { get; set; } = string.Empty;
}
