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
            .Replace(" ", "-")
            .Replace("ї", "yi")
            .Replace("й", "j")
            .Replace("ь", "")
            .Replace("ш", "sh")
            .Replace("щ", "sch")
            .Replace("ч", "ch")
            .Replace("ц", "ts")
            .Replace("є", "e")
            .Replace("ю", "yu")
            .Replace("я", "ya")
            .Trim();

        return new Slug(slug);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
