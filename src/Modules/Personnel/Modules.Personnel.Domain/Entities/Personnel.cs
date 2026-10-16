using Ardalis.Result;
using Modules.Personnel.Domain.Enums;
using Modules.Personnel.Domain.ValueObjects;
using SharedKernel.Base;

namespace Modules.Personnel.Domain.Entities;

public sealed class Personnel : BaseEntityIdentity
{
    private readonly List<PersonnelDocument> _documents = new();

    public Personnel()
    {
    }

    public PersonnelId PersonnelId => PersonnelId.Create(Id);

    public PersonalInfo PersonalInfo { get; private set; } = null!;
    public JobInfo JobInfo { get; private set; } = null!;
    public FinancialInfo FinancialInfo { get; private set; } = null!;
    public HealthInfo HealthInfo { get; private set; } = null!;
    public GuarantorInfo GuarantorInfo { get; private set; } = null!;

    public IReadOnlyCollection<PersonnelDocument> Documents => _documents.AsReadOnly();

    private Personnel(
        PersonalInfo personalInfo,
        JobInfo jobInfo,
        FinancialInfo financialInfo,
        HealthInfo healthInfo,
        GuarantorInfo guarantorInfo,
        IEnumerable<PersonnelDocument>? documents = null)
    {
        PersonalInfo = personalInfo;
        JobInfo = jobInfo;
        FinancialInfo = financialInfo;
        HealthInfo = healthInfo;
        GuarantorInfo = guarantorInfo;

        if (documents is not null)
        {
            _documents.AddRange(documents);
        }
    }

    public static Personnel Create(
        PersonalInfo personalInfo,
        JobInfo jobInfo,
        FinancialInfo financialInfo,
        HealthInfo healthInfo,
        GuarantorInfo guarantorInfo,
        IEnumerable<PersonnelDocument>? documents = null)
    {
        return new Personnel(
            personalInfo,
            jobInfo,
            financialInfo,
            healthInfo,
            guarantorInfo,
            documents);
    }



    public Result UpdatePersonalInfo(PersonalInfo personalInfo)
    {
        if (personalInfo is null)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(PersonalInfo),
                ErrorMessage = "اطلاعات شخصی الزامی است."
            });
        }

        PersonalInfo = personalInfo;
        return Result.Success();
    }


    public Result UpdateJobInfo(JobInfo jobInfo)
    {
        if (jobInfo is null)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(JobInfo),
                ErrorMessage = "اطلاعات شغلی الزامی است."
            });
        }

        JobInfo = jobInfo;
        return Result.Success();
    }

    public Result UpdateFinancialInfo(FinancialInfo financialInfo)
    {
        if (financialInfo is null)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(FinancialInfo),
                ErrorMessage = "اطلاعات مالی الزامی است."
            });
        }

        FinancialInfo = financialInfo;
        return Result.Success();
    }

    public Result UpdateHealthInfo(HealthInfo healthInfo)
    {
        if (healthInfo is null)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(HealthInfo),
                ErrorMessage = "اطلاعات سلامت الزامی است."
            });
        }

        HealthInfo = healthInfo;
        return Result.Success();
    }

    public Result UpdateGuarantorInfo(GuarantorInfo guarantorInfo)
    {
        if (guarantorInfo is null)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(GuarantorInfo),
                ErrorMessage = "اطلاعات ضامن الزامی است."
            });
        }

        GuarantorInfo = guarantorInfo;
        return Result.Success();
    }


    public void ReplaceDocuments(IEnumerable<PersonnelDocument> documents)
    {
        if (documents is null)
            throw new ArgumentNullException(nameof(documents));

        _documents.Clear();
        _documents.AddRange(documents);
    }

    public void AddDocument(PersonnelDocument document)
    {
        if (document is null)
            throw new ArgumentNullException(nameof(document));

        _documents.Add(document);
    }

    public void RemoveDocument(PersonnelDocument document)
    {
        if (document is null)
            throw new ArgumentNullException(nameof(document));

        _documents.Remove(document);
    }
}
