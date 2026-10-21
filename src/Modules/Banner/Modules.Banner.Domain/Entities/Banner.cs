using Modules.Banner.Domain.Enums;
using SharedKernel.Base;

namespace Modules.Banner.Domain.Entities
{
    public class BannerEntity : BaseEntityIdentity
    {
        public string Title { get; set; } = null;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Url { get; set; }
        public int Order { get; set; }
        public BannerType Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
