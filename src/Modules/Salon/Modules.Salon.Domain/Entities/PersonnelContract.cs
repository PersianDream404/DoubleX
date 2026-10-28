using Ardalis.Result;
using Modules.Personnel.Domain.Enums;
using SharedKernel.Base;

namespace Modules.Personnel.Domain.Entities;

public sealed class PersonnelContract : BaseEntityIdentity
{
    public long PersonnelId { get; private set; }
    public CooperationType CooperationType { get; private set; }
    public decimal? FixedSalaryAmount { get; private set; }
    public decimal? DefaultCommissionRate { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public bool IsActive { get; private set; }
    public Personnel Personnel { get; private set; } = null!;

    private PersonnelContract() { }

    private PersonnelContract(
        Personnel personnel,
        CooperationType cooperationType,
        decimal? fixedSalaryAmount,
        decimal? defaultCommissionRate,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo)
    {
        Personnel = personnel;
        PersonnelId = personnel.Id;
        CooperationType = cooperationType;
        FixedSalaryAmount = fixedSalaryAmount;
        DefaultCommissionRate = defaultCommissionRate;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        IsActive = true;
    }

    public static Result<PersonnelContract> Create(
        Personnel personnel,
        CooperationType cooperationType,
        decimal? fixedSalaryAmount,
        decimal? defaultCommissionRate,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo)
    {
        var errors = new List<ValidationError>();
        if (!Enum.IsDefined(cooperationType))
        {
            errors.Add(new ValidationError
            {
                Identifier = nameof(CooperationType),
                ErrorMessage = "نوع همکاری نامعتبر است."
            });
        }

        if (effectiveFrom == default ||
            effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
        {
            errors.Add(new ValidationError
            {
                Identifier = nameof(EffectiveFrom),
                ErrorMessage = "بازهٔ اعتبار قرارداد نامعتبر است."
            });
        }

        if (fixedSalaryAmount is < 0 ||
            fixedSalaryAmount.HasValue && decimal.Round(fixedSalaryAmount.Value, 2) != fixedSalaryAmount.Value)
        {
            errors.Add(new ValidationError
            {
                Identifier = nameof(FixedSalaryAmount),
                ErrorMessage = "مبلغ حقوق ثابت نامعتبر است."
            });
        }

        if (defaultCommissionRate is < 0 or > 100)
        {
            errors.Add(new ValidationError
            {
                Identifier = nameof(DefaultCommissionRate),
                ErrorMessage = "نرخ پورسانت باید بین صفر تا صد باشد."
            });
        }

        if (cooperationType is CooperationType.FixedSalary or CooperationType.FixedAndCommission &&
            (!fixedSalaryAmount.HasValue || fixedSalaryAmount.Value <= 0))
        {
            errors.Add(new ValidationError
            {
                Identifier = nameof(FixedSalaryAmount),
                ErrorMessage = "مبلغ حقوق برای این نوع قرارداد الزامی است."
            });
        }

        if (cooperationType is CooperationType.Commission or CooperationType.FixedAndCommission &&
            !defaultCommissionRate.HasValue)
        {
            errors.Add(new ValidationError
            {
                Identifier = nameof(DefaultCommissionRate),
                ErrorMessage = "نرخ پورسانت برای این نوع قرارداد الزامی است."
            });
        }

        if (errors.Count > 0)
        {
            return Result.Invalid(errors);
        }

        return Result.Success(new PersonnelContract(
            personnel,
            cooperationType,
            fixedSalaryAmount,
            defaultCommissionRate,
            effectiveFrom,
            effectiveTo));
    }

    public bool Close(DateOnly effectiveTo)
    {
        if (!IsActive || effectiveTo < EffectiveFrom)
        {
            return false;
        }

        if (!EffectiveTo.HasValue || effectiveTo < EffectiveTo.Value)
        {
            EffectiveTo = effectiveTo;
        }
        IsActive = false;
        return true;
    }
}
