namespace FavouriteBooks.Api.Models;

public class Address
{
    public string RecipientName { get; set; } = string.Empty;
    public string StreetLine1 { get; set; } = string.Empty;
    public string? StreetLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public bool IsValid() => GetValidationErrors().Count == 0;

    public IReadOnlyList<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(RecipientName))
        {
            errors.Add("Recipient name is required.");
        }

        if (string.IsNullOrWhiteSpace(StreetLine1))
        {
            errors.Add("Street line 1 is required.");
        }

        if (string.IsNullOrWhiteSpace(City))
        {
            errors.Add("City is required.");
        }

        if (string.IsNullOrWhiteSpace(State))
        {
            errors.Add("State is required.");
        }

        if (string.IsNullOrWhiteSpace(Postcode))
        {
            errors.Add("Postcode is required.");
        }

        if (string.IsNullOrWhiteSpace(Country))
        {
            errors.Add("Country is required.");
        }

        return errors;
    }
}
