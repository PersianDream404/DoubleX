using SharedKernel.Base;

namespace Modules.Personnel.Domain.Entities;

public sealed class Salon: BaseEntityIdentity
{
    public string Name { get; set; } = null!;


    private Salon(string name)
    {
   
        Name = name;
    }
    public static Salon Create(string name)
    {
        return new Salon(name);
    }
    public ICollection<Personnel> Personnels { get; set; } = [];
}