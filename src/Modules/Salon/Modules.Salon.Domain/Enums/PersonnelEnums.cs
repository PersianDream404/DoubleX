using System.ComponentModel.DataAnnotations;

namespace Modules.Personnel.Domain.Enums;

public enum MaritalStatus
{
    [Display(Name = "مجرد")]
    Single = 0,
    [Display(Name = "متاهل")]
    Married = 1
}

public enum WorkingShiftType
{


    [Display(Name = "پورسانتی")]
    Commission = 1,

    [Display(Name = "ثابت و پورسانت")]
    FixedAndCommission = 2,

    [Display(Name = "توافقی")]
    Negotiable = 3,

    [Display(Name = "اجاره‌ای")]
    Rental = 4,

    [Display(Name = "تمام وقت")]
    FullTime = 5,

    [Display(Name = "نیمه وقت (صبح)")]
    PartTimeMorning = 6,

    [Display(Name = "نیمه وقت (عصر)")]
    PartTimeEvening = 7
}

public enum SalaryType
{
    [Display(Name = "ثابت")]
    Fixed = 0,

    [Display(Name = "پورسانتی")]
    Commission = 1,

    [Display(Name = "اجاره صندلی")]
    ChairRental = 2,

    [Display(Name = "اجاره اتاق")]
    RoomRental = 3
}

public enum DocumentType
{
    Photo = 0,
    TrainingCertificate = 1,
    MedicalDocument = 2,
    GuarantorDocument = 3,
    HealthCourse = 4,
    HealthTest = 5
}
