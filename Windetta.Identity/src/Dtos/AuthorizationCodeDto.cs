namespace Windetta.Identity.Dtos;

public class AuthorizationCodeDto
{
    public string Value { get; set; }

    public AuthorizationCodeDto(string value)
    {
        Value = value;
    }
}
