using Ardalis.Result;

namespace Modules.Personnel.Domain.ValueObjects;

public sealed record PersonnelId
{
    public long Value { get; }

    public static PersonnelId Empty { get; } = new(0);

    private PersonnelId(long value)
    {
        Value = value;
    }

    public static Result<PersonnelId> Create(long value)
    {
        if (value < 0)
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(PersonnelId),
                ErrorMessage = "PersonnelId must be greater than or equal to 0."
            });

        return Result.Success(new PersonnelId(value));
    }

    public override string ToString() => Value.ToString();
}

public sealed class NationalCode
{
    public string Value { get; }

    private NationalCode(string value)
    {
        Value = value;
    }

    public static Result<NationalCode> Create(string nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(NationalCode),
                ErrorMessage = "کد ملی الزامی است."
            });
        }

        var normalized = Normalize(nationalCode);

        if (!IsValid(normalized))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(NationalCode),
                ErrorMessage = "کد ملی نامعتبر است."
            });
        }

        return Result.Success(new NationalCode(normalized));
    }

    public static bool IsValid(string? nationalCode)
    {
        if (string.IsNullOrWhiteSpace(nationalCode))
            return false;

        var normalized = Normalize(nationalCode);

        if (normalized.Length != 10 || normalized.All(c => c == normalized[0]))
            return false;

        if (!normalized.All(char.IsDigit))
            return false;

        var digits = normalized.Select(c => c - '0').ToArray();
        var checksum = digits[9];
        var sum = Enumerable.Range(0, 9).Sum(i => digits[i] * (10 - i));
        var remainder = sum % 11;

        return remainder < 2
            ? checksum == remainder
            : checksum == 11 - remainder;
    }

    private static string Normalize(string value)
        => value.Trim().Replace(" ", string.Empty);

    public override string ToString() => Value;
}


public sealed record Picture
{
    public string Value { get; private set; }

    private Picture(string value)
    {
        Value = value;
    }

    public static Result<Picture> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(Picture),
                ErrorMessage = "ارسال تصویر الزامی است"
            });

        return new Picture(value);
    }
}
public sealed class PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(PhoneNumber),
                ErrorMessage = "شماره تلفن الزامی است."
            });
        }

        var normalized = Normalize(phoneNumber);

        if (!IsValid(normalized))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(PhoneNumber),
                ErrorMessage = "شماره تلفن نامعتبر است."
            });
        }

        return Result.Success(new PhoneNumber(normalized));
    }

    public static bool IsValid(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var normalized = Normalize(phoneNumber);

        if (!normalized.All(char.IsDigit))
            return false;

        if (normalized.Length == 11 && normalized.StartsWith("0"))
            return true;

        if (normalized.Length == 10 && normalized.StartsWith("1"))
            return true;

        return false;
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Trim()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        if (normalized.StartsWith("+98"))
            normalized = "0" + normalized[3..];

        return normalized;
    }

    public override string ToString() => Value;
}

public sealed class CardNumber
{
    public string Value { get; }

    private CardNumber(string value)
    {
        Value = value;
    }

    public static Result<CardNumber> Create(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(CardNumber),
                ErrorMessage = "شماره کارت الزامی است."
            });
        }

        var normalized = Normalize(cardNumber);

        if (!IsValid(normalized))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(CardNumber),
                ErrorMessage = "شماره کارت نامعتبر است."
            });
        }

        return Result.Success(new CardNumber(normalized));
    }

    public static bool IsValid(string? cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        var normalized = Normalize(cardNumber);
        return normalized.Length == 16 && normalized.All(char.IsDigit);
    }

    private static string Normalize(string value)
        => value.Trim()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

    public override string ToString() => Value;
}

public sealed record Address
{
    public string Text { get; private set; }
    public bool IsCloseToSalon { get; private set; }

    private Address(string text, bool isCloseToSalon)
    {
        Text = text;
        IsCloseToSalon = isCloseToSalon;
    }

    public static Result<Address> Create(string text, bool isCloseToSalon)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(Address),
                ErrorMessage = "آدرس الزامی است."
            });
        }

        return Result.Success(new Address(text.Trim(), isCloseToSalon));
    }

    public override string ToString() => Text;
}
