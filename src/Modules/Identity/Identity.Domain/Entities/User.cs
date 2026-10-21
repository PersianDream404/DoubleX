using Microsoft.AspNetCore.Identity;
using SharedKernel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Identity.Domain.Entities;

public class ApplicationUser : IdentityUser<long>
{
    [MaxLength(200)]
    public string? FullName { get; set; } 
    public long SalonId { get; private set; }

    public ApplicationUser SetSalonId(long salonId)
    {
        SalonId = salonId;
        return this;
    }

    public static ApplicationUser Register(string phone,string fullName,long salonId)
    {
        var user = new ApplicationUser
        {
        
            FullName = fullName,
            PhoneNumber = phone,
            UserName = phone,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        user.SetSalonId(salonId);
        return user;
    }
}
