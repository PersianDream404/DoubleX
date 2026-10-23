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
    public int? ChildrenCount { get; private set; }
    public Picture? Picture { get; private set; }
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
  //  public List<string> Skills { get; private set; }
    public List<string> JobPictures { get; private set; }
    public string? WorkHistory { get; private set; }

    private JobInfo(
        string jobTitle,
        string personnelCode,
        DateOnly cooperationStartDate,
        WorkingShiftType workingShiftType,
      //  List<string> skills,
      List<string> jobPictures,
        string? workHistory)
    {
        JobTitle = jobTitle;
        PersonnelCode = personnelCode;
        CooperationStartDate = cooperationStartDate;
        WorkingShiftType = workingShiftType;
        JobPictures = jobPictures;
     //   Skills = skills;
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
        JobPictures = result.Value.JobPictures;
        WorkHistory = result.Value.WorkHistory;

        return Result.Success();
    }

    public JobInfo()
    {
        JobTitle = default!;
        PersonnelCode = default!;
        JobPictures = new List<string>();
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
    public bool HealthStatus { get; private set; }
    public bool AllergyStatus { get; private set; }
    public string Diseases { get; private set; } = string.Empty;
    public string Allergies { get; private set; } = string.Empty;
    public List<Picture> PictureCourse { get; private set; } = [];
    public List<Picture> PictureTest { get; private set; } = [];

    private HealthInfo(
        bool healthStatus,
        bool allergyStatus,
        string? diseases,
        string? allergies,
        List<Picture>? pictureCourse,
        List<Picture>? pictureTest)
    {
        HealthStatus = healthStatus;
        AllergyStatus = allergyStatus;
        Diseases = Normalize(diseases);
        Allergies = Normalize(allergies);
        PictureCourse = pictureCourse ?? [];
        PictureTest = pictureTest ?? [];
    }

    public static Result<HealthInfo> Create(
        bool healthStatus,
        bool allergyStatus,
        string? diseases,
        string? allergies,
        List<Picture>? pictureCourse,
        List<Picture>? pictureTest)
    {
        return Result.Success(new HealthInfo(
            healthStatus,
            allergyStatus,
            diseases,
            allergies,
            pictureCourse,
            pictureTest));
    }

    public Result Update(
        bool healthStatus,
        bool allergyStatus,
        string? diseases,
        string? allergies,
        List<Picture>? pictureCourse,
        List<Picture>? pictureTest)
    {
        var result = Create(
            healthStatus,
            allergyStatus,
            diseases,
            allergies,
            pictureCourse,
            pictureTest);

        if (!result.IsSuccess)
            return Result.Invalid(result.ValidationErrors);

        HealthStatus = result.Value.HealthStatus;
        AllergyStatus = result.Value.AllergyStatus;
        Diseases = result.Value.Diseases;
        Allergies = result.Value.Allergies;
        PictureCourse = result.Value.PictureCourse;
        PictureTest = result.Value.PictureTest;

        return Result.Success();
    }

    private static string Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    public HealthInfo()
    {
    }
}


public sealed record GuarantorInfo
{
    public string GuarantorName { get; private set; }
    public string Relationship { get; private set; }
    public PhoneNumber GuarantorMobilePhone { get; private set; }
    public PhoneNumber GuarantorLandlinePhone { get; private set; }
    public List<Picture> PictureGuaranties { get; private set; } = [];

    private GuarantorInfo(
        string guarantorName,
        string relationship,
        PhoneNumber guarantorMobilePhone,
        PhoneNumber guarantorLandlinePhone,
        List<Picture>? pictureGuaranties)
    {
        GuarantorName = guarantorName;
        Relationship = relationship;
        GuarantorMobilePhone = guarantorMobilePhone;
        GuarantorLandlinePhone = guarantorLandlinePhone;
        PictureGuaranties = pictureGuaranties ?? [];
    }

    public static Result<GuarantorInfo> Create(
        string guarantorName,
        string relationship,
        string guarantorMobilePhone,
        string guarantorLandlinePhone,
        List<Picture>? pictureGuaranties)
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

        // اعتبارسنجی شماره موبایل
        var mobileResult = PhoneNumber.Create(guarantorMobilePhone);
        if (!mobileResult.IsSuccess)
            validationErrors.AddRange(mobileResult.ValidationErrors);

        // اعتبارسنجی تلفن ثابت (اگر اختیاری است می‌توانید شرط بگذارید، در غیر این صورت مثل موبایل بررسی می‌شود)
        var landlineResult = PhoneNumber.Create(guarantorLandlinePhone);
        if (!landlineResult.IsSuccess)
            validationErrors.AddRange(landlineResult.ValidationErrors);

        if (validationErrors.Count > 0)
            return Result.Invalid(validationErrors);

        return Result.Success(new GuarantorInfo(
            guarantorName.Trim(),
            relationship.Trim(),
            mobileResult.Value,
            landlineResult.Value,
            pictureGuaranties));
    }

    public Result Update(
        string guarantorName,
        string relationship,
        string guarantorMobilePhone,
        string guarantorLandlinePhone,
        List<Picture>? pictureGuaranties)
    {
        var result = Create(
            guarantorName,
            relationship,
            guarantorMobilePhone,
            guarantorLandlinePhone,
            pictureGuaranties);

        if (!result.IsSuccess)
            return Result.Invalid(result.ValidationErrors);

        GuarantorName = result.Value.GuarantorName;
        Relationship = result.Value.Relationship;
        GuarantorMobilePhone = result.Value.GuarantorMobilePhone;
        GuarantorLandlinePhone = result.Value.GuarantorLandlinePhone;
        PictureGuaranties = result.Value.PictureGuaranties;

        return Result.Success();
    }

    public GuarantorInfo()
    {
        GuarantorName = default!;
        Relationship = default!;
        GuarantorMobilePhone = default!;
        GuarantorLandlinePhone = default!;
    }
}

