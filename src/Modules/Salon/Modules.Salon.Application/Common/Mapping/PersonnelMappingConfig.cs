using Mapster;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Enums;
using Modules.Personnel.Domain.ValueObjects;

namespace Modules.Personnel.Application.Common.Mapping;

public static class PersonnelMappingConfig
{
    public static void RegisterMappings()
    {

        // نحوه ساخت NationalCode از روی string
        TypeAdapterConfig<string, NationalCode>
            .NewConfig()
            .MapWith(src => NationalCode.Create(src));

        // نحوه ساخت PhoneNumber از روی string
        TypeAdapterConfig<string, PhoneNumber>
            .NewConfig()
            .MapWith(src => PhoneNumber.Create(src));

        // نحوه ساخت CardNumber از روی string
        TypeAdapterConfig<string, CardNumber>
            .NewConfig()
            .MapWith(src => CardNumber.Create(src));

        // در صورت لزوم، تبدیل برعکس (تبدیل شیء به string در زمان خروجی گرفتن)
        TypeAdapterConfig<NationalCode, string>.NewConfig().MapWith(src => src.Value);
        TypeAdapterConfig<PhoneNumber, string>.NewConfig().MapWith(src => src.Value);
        TypeAdapterConfig<CardNumber, string>.NewConfig().MapWith(src => src.Value);

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
          //  .Map(dest => dest.Skills, src => src.Skills.ToList())
            .Map(dest => dest.WorkHistory, src => src.WorkHistory);

        TypeAdapterConfig<FinancialInfoRequestDto, FinancialInfo>
            .NewConfig()
            .Map(dest => dest.CardNumber, src => CardNumber.Create(src.CardNumber))
            .Map(dest => dest.AccountNumber, src => src.AccountNumber);

        TypeAdapterConfig<GuarantorInfoRequestDto, GuarantorInfo>
            .NewConfig()
            .Map(dest => dest.GuarantorName, src => src.GuarantorName)
            .Map(dest => dest.Relationship, src => src.Relationship)
            .Map(dest => dest.GuarantorMobilePhone, src => PhoneNumber.Create(src.GuarantorPhone));

        TypeAdapterConfig<DocumentRequestDto, PersonnelDocument>
            .NewConfig();

        TypeAdapterConfig<CreatePersonnelRequestDto, Domain.Entities.Personnel>
            .NewConfig()
            .Map(dest => dest.PersonalInfo, src => src.PersonalInfo.Adapt<PersonalInfo>())
            .Map(dest => dest.JobInfo, src => src.JobInfo.Adapt<JobInfo>())
            .Map(dest => dest.FinancialInfo, src => src.FinancialInfo.Adapt<FinancialInfo>())
            .Map(dest => dest.HealthInfo, src => src.HealthInfo.Adapt<HealthInfo>())
            .Map(dest => dest.GuarantorInfo, src => src.GuarantorInfo.Adapt<GuarantorInfo>());
        //  .Map(dest => dest.Documents, src => src.Documents.Select(x => x.Adapt<PersonnelDocument>()).ToList());

    }
}
