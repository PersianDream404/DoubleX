using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Base;

public class BaseEntityEmpty
{
    public int Id { get; set; }


}
public class BaseEntitySoftEmpty
{
    public bool Deleted { get; set; } = false;
    public int Id { get; set; }

    public bool Status { get; set; } = true;

}
public class BaseEntityIdentity : BaseEntity
{
    public int Id { get; set; }


}
public class BaseEntity
{

    public bool Deleted { get; set; } = false;

    ///// <summary>
    ///// تاریخ ایجاد
    ///// </summary>
    //public DateTime CreateDate { get; set; } = DateTime.Now;

    ///// <summary>
    ///// تاریخ بروزرسانی
    ///// </summary>
    //public DateTime UpdateDate { get; set; } = DateTime.Now;

    /// <summary>
    /// وضعیت
    /// </summary>
    public bool Status { get; set; } = true;
}
public class BaseEntityPerson : BaseEntityLocation
{
   // public GenderModel? Gender { get; set; }
    public string? FatherName { get; set; } = string.Empty;
    public string? BirthDate { get; set; }
    public string? NationalCode { get; set; } = string.Empty;

}
public class BaseEntityLocation : BaseEntityIdentity
{    /// <summary>
     /// شناسه استان محل فعالیت مرکز (اختیاری).
     /// </summary>
    public int? ProvinceId { get; set; }
   // public IranProvince? IranProvince { get; set; }
    /// <summary>
    /// شناسه شهر محل فعالیت مرکز (اختیاری).
    /// برای دسته‌بندی جغرافیایی مراکز استفاده می‌شود.
    /// </summary>
    public int? CityId { get; set; }
   // public IranCity? IranCity { get; set; }

    /// <summary>
    /// منطقه یا ناحیه جغرافیایی مرکز (اختیاری).
    /// می‌تواند برای فیلترهای منطقه‌ای استفاده شود.
    /// </summary>
    public string? Region { get; set; } = string.Empty;
}