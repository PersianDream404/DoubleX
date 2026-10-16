using Mapster;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Enums;
using Modules.Personnel.Domain.ValueObjects;

namespace Modules.Personnel.Application.Common.Mapping;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<PersonalInfoRequestDto, PersonalInfo>
            .NewConfig()
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.NationalCode, src => NationalCode.Create(src.NationalCode))
            .Map(dest => dest.BirthDate, src => src.BirthDate)

            .Map(dest => dest.ChildrenCount, src => src.ChildrenCount)
            .Map(dest => dest.MobilePhone, src => PhoneNumber.Create(src.MobilePhone))
            .Map(dest => dest.LandlinePhone, src => string.IsNullOrWhiteSpace(src.LandlinePhone) ? null : PhoneNumber.Create(src.LandlinePhone!))
            ;

        TypeAdapterConfig<JobInfoRequestDto, JobInfo>
            .NewConfig()
            .Map(dest => dest.JobTitle, src => src.JobTitle)
            .Map(dest => dest.PersonnelCode, src => src.PersonnelCode)
            .Map(dest => dest.CooperationStartDate, src => src.CooperationStartDate)
            .Map(dest => dest.Skills, src => src.Skills.ToList())
            .Map(dest => dest.WorkHistory, src => src.WorkHistory);

        TypeAdapterConfig<FinancialInfoRequestDto, FinancialInfo>
            .NewConfig()
            .Map(dest => dest.CardNumber, src => CardNumber.Create(src.CardNumber))
            .Map(dest => dest.AccountNumber, src => src.AccountNumber);

        TypeAdapterConfig<HealthInfoRequestDto, HealthInfo>
            .NewConfig();

        TypeAdapterConfig<GuarantorInfoRequestDto, GuarantorInfo>
            .NewConfig()
            .Map(dest => dest.GuarantorName, src => src.GuarantorName)
            .Map(dest => dest.Relationship, src => src.Relationship)
            .Map(dest => dest.GuarantorPhone, src => PhoneNumber.Create(src.GuarantorPhone));

        TypeAdapterConfig<DocumentRequestDto, PersonnelDocument>
            .NewConfig()
            .Map(dest => dest.DocumentType, src => src.DocumentType)
            .Map(dest => dest.FileName, src => src.FileName)
            .Map(dest => dest.Url, src => src.Url);

        TypeAdapterConfig<CreatePersonnelRequestDto, Domain.Entities.Personnel>
            .NewConfig()
            .Map(dest => dest.PersonalInfo, src => src.PersonalInfo.Adapt<PersonalInfo>())
            .Map(dest => dest.JobInfo, src => src.JobInfo.Adapt<JobInfo>())
            .Map(dest => dest.FinancialInfo, src => src.FinancialInfo.Adapt<FinancialInfo>())
            .Map(dest => dest.HealthInfo, src => src.HealthInfo.Adapt<HealthInfo>())
            .Map(dest => dest.GuarantorInfo, src => src.GuarantorInfo.Adapt<GuarantorInfo>())
            .Map(dest => dest.Documents, src => src.Documents.Select(x => x.Adapt<PersonnelDocument>()).ToList());

        TypeAdapterConfig< Domain.Entities.Personnel, GetPersonnelDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.PersonalInfo, src => src.PersonalInfo.Adapt<PersonalInfoDto>())
            .Map(dest => dest.JobInfo, src => src.JobInfo.Adapt<JobInfoDto>())
            .Map(dest => dest.FinancialInfo, src => src.FinancialInfo.Adapt<FinancialInfoDto>())
            .Map(dest => dest.HealthInfo, src => src.HealthInfo.Adapt<HealthInfoDto>())
            .Map(dest => dest.GuarantorInfo, src => src.GuarantorInfo.Adapt<GuarantorInfoDto>())
            .Map(dest => dest.Documents, src => src.Documents.Select(x => x.Adapt<DocumentDto>()).ToList());

        TypeAdapterConfig<PersonalInfo, PersonalInfoDto>
            .NewConfig()
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.NationalCode, src => src.NationalCode.Value)
            .Map(dest => dest.BirthDate, src => src.BirthDate)
            .Map(dest => dest.MaritalStatus, src => src.MaritalStatus.ToString())
            .Map(dest => dest.ChildrenCount, src => src.ChildrenCount)
            .Map(dest => dest.MobilePhone, src => src.MobilePhone.Value)
            .Map(dest => dest.LandlinePhone, src => src.LandlinePhone!.Value)
            .Map(dest => dest.AddressText, src => src.Address.Text)
            .Map(dest => dest.IsCloseToSalon, src => src.Address.IsCloseToSalon);

        TypeAdapterConfig<JobInfo, JobInfoDto>
            .NewConfig()
            .Map(dest => dest.JobTitle, src => src.JobTitle)
            .Map(dest => dest.PersonnelCode, src => src.PersonnelCode)
            .Map(dest => dest.CooperationStartDate, src => src.CooperationStartDate)
            .Map(dest => dest.WorkingShiftType, src => src.WorkingShiftType.ToString())
            .Map(dest => dest.Skills, src => src.Skills)
            .Map(dest => dest.WorkHistory, src => src.WorkHistory);

        TypeAdapterConfig<FinancialInfo, FinancialInfoDto>
            .NewConfig()
            .Map(dest => dest.SalaryType, src => src.SalaryType.ToString())
            .Map(dest => dest.CardNumber, src => src.CardNumber.Value)
            .Map(dest => dest.AccountNumber, src => src.AccountNumber);

        TypeAdapterConfig<HealthInfo, HealthInfoDto>
            .NewConfig();

        TypeAdapterConfig<GuarantorInfo, GuarantorInfoDto>
            .NewConfig()
            .Map(dest => dest.GuarantorName, src => src.GuarantorName)
            .Map(dest => dest.Relationship, src => src.Relationship)
            .Map(dest => dest.GuarantorPhone, src => src.GuarantorPhone.Value);
    }
}
