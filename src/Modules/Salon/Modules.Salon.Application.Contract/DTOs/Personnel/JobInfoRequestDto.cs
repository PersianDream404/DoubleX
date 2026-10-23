using Modules.Personnel.Domain.Enums;

namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

using System;
using System.Collections.Generic;

public sealed class JobInfoRequestDto
{
    public string JobTitle { get; init; }
    public string PersonnelCode { get; init; }
    public DateOnly CooperationStartDate { get; init; }
    public WorkingShiftType WorkingShiftType { get; init; }
    public List<string> Skills { get; init; } = [];
    public List<string> JobPictures { get; init; } = [];
    public string? WorkHistory { get; init; }

    public JobInfoRequestDto(
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
        Skills = skills ?? [];
        JobPictures = skills ?? [];
        WorkHistory = workHistory;
    }

    public JobInfoRequestDto()
    {
        
    }
}
