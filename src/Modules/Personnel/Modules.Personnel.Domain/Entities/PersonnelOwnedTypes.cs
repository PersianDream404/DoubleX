using Ardalis.Result;
using Modules.Personnel.Domain.Enums;
using Modules.Personnel.Domain.ValueObjects;

namespace Modules.Personnel.Domain.Entities;

public sealed record PersonalInfo
{
    public string FullName { get; private set; }
    public NationalCode NationalCode { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public MaritalStatus MaritalStatus { get; private set; }
    public int ChildrenCount { get; private set; }
    public PhoneNumber MobilePhone { get; private set; }
    public PhoneNumber? LandlinePhone { get; private set; }
    public Address Address { get; private set; }

    private PersonalInfo(
        string fullName,
        NationalCode nationalCode,
        DateOnly birthDate,
        MaritalStatus maritalStatus,
        int childrenCount,
        PhoneNumber mobilePhone,
        PhoneNumber? landlinePhone,
        Address address)
    {
        FullName = fullName;
        NationalCode = nationalCode;
        BirthDate = birthDate;
        MaritalStatus = maritalStatus;
        ChildrenCount = childrenCount;
        MobilePhone = mobilePhone;
        LandlinePhone = landlinePhone;
        Address = address;
    }

    public static Result<PersonalInfo> Create(
        string fullName,
        string nationalCode,
        DateOnly birthDate,
        MaritalStatus maritalStatus,
        int childrenCount,
        string mobilePhone,
        string? landlinePhone,
        string addressText,
        bool isCloseToSalon)
    {
        var validationErrors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(fullName))
        {
            validationErrors.Add(new ValidationError
            {
                Identifier = nameof(fullName),
                ErrorMessage = "نام و نام خانوادگی الزامی است."
            });
        }

        if (childrenCount < 0)
        {
            validationErrors.Add(new ValidationError
            {
                Identifier = nameof(childrenCount),
                ErrorMessage = "تعداد فرزندان نمی‌تواند کمتر از صفر باشد."
            });
        }

        var nationalCodeResult = NationalCode.Create(nationalCode);
        if (!nationalCodeResult.IsSuccess)
            validationErrors.AddRange(nationalCodeResult.ValidationErrors);

        var mobilePhoneResult = PhoneNumber.Create(mobilePhone);
        if (!mobilePhoneResult.IsSuccess)
            validationErrors.AddRange(mobilePhoneResult.ValidationErrors);

        PhoneNumber? landlinePhoneValue = null;
        if (!string.IsNullOrWhiteSpace(landlinePhone))
        {
            var landlinePhoneResult = PhoneNumber.Create(landlinePhone);
            if (!landlinePhoneResult.IsSuccess)
                validationErrors.AddRange(landlinePhoneResult.ValidationErrors);
            else
                landlinePhoneValue = landlinePhoneResult.Value;
        }

        var addressResult = Address.Create(addressText, isCloseToSalon);
        if (!addressResult.IsSuccess)
            validationErrors.AddRange(addressResult.ValidationErrors);

        if (validationErrors.Count > 0)
            return Result.Invalid(validationErrors);

        return Result.Success(new PersonalInfo(
            fullName.Trim(),
            nationalCodeResult.Value,
            birthDate,
            maritalStatus,
            childrenCount,
            mobilePhoneResult.Value,
            landlinePhoneValue,
            addressResult.Value));
    }



    public void Update(
                string fullName,
                NationalCode nationalCode,
                DateOnly birthDate,
                MaritalStatus maritalStatus,
                int childrenCount,
                PhoneNumber mobilePhone,
                PhoneNumber? landlinePhone,
                Address address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentNullException.ThrowIfNull(nationalCode);
        ArgumentNullException.ThrowIfNull(mobilePhone);
        ArgumentNullException.ThrowIfNull(address);

        if (childrenCount < 0)
            throw new ArgumentOutOfRangeException(nameof(childrenCount));

        FullName = fullName;
        NationalCode = nationalCode;
        BirthDate = birthDate;
        MaritalStatus = maritalStatus;
        ChildrenCount = childrenCount;
        MobilePhone = mobilePhone;
        LandlinePhone = landlinePhone;
        Address = address;
    }

    public PersonalInfo() { }
}


public sealed record JobInfo
{
    public string JobTitle { get; private set; }
    public string PersonnelCode { get; private set; }
    public DateOnly CooperationStartDate { get; private set; }
    public WorkingShiftType WorkingShiftType { get; private set; }
    public List<string> Skills { get; private set; }
    public string? WorkHistory { get; private set; }

    private JobInfo(
        string jobTitle,
        string personnelCode,
        DateOnly cooperationStartDate,
        WorkingShiftType workingShiftType,
        List<string> skills,
        string? workHistory)
    {
        JobTitle = jobTitle;
        PersonnelCode = personnelCode;
        CooperationStartDate = cooperationStartDate;
        WorkingShiftType = workingShiftType;
        Skills = skills;
        WorkHistory = workHistory;
    }

    public static Result<JobInfo> Create(
        string jobTitle,
        string personnelCode,
        DateOnly cooperationStartDate,
        WorkingShiftType workingShiftType,
        List<string>? skills,
        string? workHistory)
    {
        var validationErrors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(jobTitle))
        {
            validationErrors.Add(new ValidationError
            {
                Identifier = nameof(jobTitle),
                ErrorMessage = "عنوان شغلی الزامی است."
            });
        }

        if (string.IsNullOrWhiteSpace(personnelCode))
        {
            validationErrors.Add(new ValidationError
            {
                Identifier = nameof(personnelCode),
                ErrorMessage = "کد پرسنلی الزامی است."
            });
        }

        var normalizedSkills = skills?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct()
            .ToList() ?? new List<string>();

        if (validationErrors.Count > 0)
            return Result.Invalid(validationErrors);

        return Result.Success(new JobInfo(
            jobTitle.Trim(),
            personnelCode.Trim(),
            cooperationStartDate,
            workingShiftType,
            normalizedSkills,
            string.IsNullOrWhiteSpace(workHistory) ? null : workHistory.Trim()));
    }

    public Result Update(
        string jobTitle,
        string personnelCode,
        DateOnly cooperationStartDate,
        WorkingShiftType workingShiftType,
        List<string>? skills,
        string? workHistory)
    {
        var result = Create(
            jobTitle,
            personnelCode,
            cooperationStartDate,
            workingShiftType,
            skills,
            workHistory);

        if (!result.IsSuccess)
            return Result.Invalid(result.ValidationErrors);

        JobTitle = result.Value.JobTitle;
        PersonnelCode = result.Value.PersonnelCode;
        CooperationStartDate = result.Value.CooperationStartDate;
        WorkingShiftType = result.Value.WorkingShiftType;
        Skills = result.Value.Skills;
        WorkHistory = result.Value.WorkHistory;

        return Result.Success();
    }

    public JobInfo()
    {
        JobTitle = default!;
        PersonnelCode = default!;
        Skills = new List<string>();
    }
}

public sealed record FinancialInfo
{
    public SalaryType SalaryType { get; private set; }
    public CardNumber CardNumber { get; private set; }
    public string AccountNumber { get; private set; }

    private FinancialInfo(
        SalaryType salaryType,
        CardNumber cardNumber,
        string accountNumber)
    {
        SalaryType = salaryType;
        CardNumber = cardNumber;
        AccountNumber = accountNumber;
    }

    public static Result<FinancialInfo> Create(
        SalaryType salaryType,
        string cardNumber,
        string accountNumber)
    {
        var validationErrors = new List<ValidationError>();

        var cardNumberResult = CardNumber.Create(cardNumber);
        if (!cardNumberResult.IsSuccess)
            validationErrors.AddRange(cardNumberResult.ValidationErrors);

        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            validationErrors.Add(new ValidationError
            {
                Identifier = nameof(accountNumber),
                ErrorMessage = "شماره حساب الزامی است."
            });
        }

        if (validationErrors.Count > 0)
            return Result.Invalid(validationErrors);

        return Result.Success(new FinancialInfo(
            salaryType,
            cardNumberResult.Value,
            accountNumber.Trim()));
    }

    public Result Update(
        SalaryType salaryType,
        string cardNumber,
        string accountNumber)
    {
        var result = Create(
            salaryType,
            cardNumber,
            accountNumber);

        if (!result.IsSuccess)
            return Result.Invalid(result.ValidationErrors);

        SalaryType = result.Value.SalaryType;
        CardNumber = result.Value.CardNumber;
        AccountNumber = result.Value.AccountNumber;

        return Result.Success();
    }

    public FinancialInfo()
    {
        CardNumber = default!;
        AccountNumber = default!;
    }
}

public sealed record HealthInfo
{
    public string? HealthStatus { get; private set; }
    public string? AllergyStatus { get; private set; }

    private HealthInfo(string? healthStatus, string? allergyStatus)
    {
        HealthStatus = healthStatus;
        AllergyStatus = allergyStatus;
    }

    public static Result<HealthInfo> Create(string? healthStatus, string? allergyStatus)
    {
        return Result.Success(new HealthInfo(
            string.IsNullOrWhiteSpace(healthStatus) ? null : healthStatus.Trim(),
            string.IsNullOrWhiteSpace(allergyStatus) ? null : allergyStatus.Trim()));
    }

    public Result Update(string? healthStatus, string? allergyStatus)
    {
        var result = Create(healthStatus, allergyStatus);
        if (!result.IsSuccess)
            return Result.Invalid(result.ValidationErrors);

        HealthStatus = result.Value.HealthStatus;
        AllergyStatus = result.Value.AllergyStatus;

        return Result.Success();
    }

    public HealthInfo() { }
}

public sealed record GuarantorInfo
{
    public string GuarantorName { get; private set; }
    public string Relationship { get; private set; }
    public PhoneNumber GuarantorPhone { get; private set; }

    private GuarantorInfo(
        string guarantorName,
        string relationship,
        PhoneNumber guarantorPhone)
    {
        GuarantorName = guarantorName;
        Relationship = relationship;
        GuarantorPhone = guarantorPhone;
    }

    public static Result<GuarantorInfo> Create(
        string guarantorName,
        string relationship,
        string guarantorPhone)
    {
        var validationErrors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(guarantorName))
        {
            validationErrors.Add(new ValidationError
            {
                Identifier = nameof(guarantorName),
                ErrorMessage = "نام ضامن الزامی است."
            });
        }

        if (string.IsNullOrWhiteSpace(relationship))
        {
            validationErrors.Add(new ValidationError
            {
                Identifier = nameof(relationship),
                ErrorMessage = "نسبت ضامن الزامی است."
            });
        }

        var guarantorPhoneResult = PhoneNumber.Create(guarantorPhone);
        if (!guarantorPhoneResult.IsSuccess)
            validationErrors.AddRange(guarantorPhoneResult.ValidationErrors);

        if (validationErrors.Count > 0)
            return Result.Invalid(validationErrors);

        return Result.Success(new GuarantorInfo(
            guarantorName.Trim(),
            relationship.Trim(),
            guarantorPhoneResult.Value));
    }

    public Result Update(
        string guarantorName,
        string relationship,
        string guarantorPhone)
    {
        var result = Create(
            guarantorName,
            relationship,
            guarantorPhone);

        if (!result.IsSuccess)
            return Result.Invalid(result.ValidationErrors);

        GuarantorName = result.Value.GuarantorName;
        Relationship = result.Value.Relationship;
        GuarantorPhone = result.Value.GuarantorPhone;

        return Result.Success();
    }

    public GuarantorInfo()
    {
        GuarantorName = default!;
        Relationship = default!;
        GuarantorPhone = default!;
    }
}

