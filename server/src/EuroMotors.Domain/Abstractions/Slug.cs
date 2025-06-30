using System.Text.RegularExpressions;

namespace EuroMotors.Domain.Abstractions;

public sealed class Slug : ValueObject
{
    public string Value { get; }

    private Slug(string value) => Value = value;

    public static Slug GenerateSlug(string input)
    {
#pragma warning disable CA1308
        string slug = input.ToLowerInvariant()
#pragma warning restore CA1308
                .Replace("ї", "yi")
                .Replace("і", "i")
                .Replace("ы", "y")
                .Replace("є", "ye")
                .Replace("ґ", "g")
                .Replace("й", "i")
                .Replace("ю", "yu")
                .Replace("я", "ya")
                .Replace("ш", "sh")
                .Replace("щ", "shch")
                .Replace("ч", "ch")
                .Replace("ц", "ts")
                .Replace("ж", "zh")
                .Replace("а", "a")
                .Replace("б", "b")
                .Replace("в", "v")
                .Replace("г", "h")
                .Replace("д", "d")
                .Replace("е", "e")
                .Replace("з", "z")
                .Replace("и", "y")
                .Replace("к", "k")
                .Replace("л", "l")
                .Replace("м", "m")
                .Replace("н", "n")
                .Replace("о", "o")
                .Replace("п", "p")
                .Replace("р", "r")
                .Replace("с", "s")
                .Replace("т", "t")
                .Replace("у", "u")
                .Replace("ф", "f")
                .Replace("х", "kh")
                .Replace("ь", "")
                .Replace("'", "")
                .Replace("’", "")
                .Replace("/", "-")
                .Replace(">", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(",", "")
                .Replace(".", "")
            ;

        slug = Regex.Replace(slug, @"\s+", " ");
        slug = slug.Replace(" ", "-");
        slug = Regex.Replace(slug, "-{2,}", "-");
        slug = slug.Trim('-');

        return new Slug(slug);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
