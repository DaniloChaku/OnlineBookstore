namespace OnlineBookstore.Dal.Constants;

public static class ValidationConstants
{
    public const int AuthorNameMaxLength = 255;
    public const int GenreNameMaxLength = 100;
    public const int BookTitleMaxLength = 255;

    public const decimal BookPriceMin = 0.01m;
    public const decimal BookPriceMax = 10000m;

    public const int BookQuantityMin = 0;
    public const int BookQuantityMax = 10000;
}
