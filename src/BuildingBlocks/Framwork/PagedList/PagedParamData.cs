using System.ComponentModel.DataAnnotations;

namespace Framwork.PagedList;


public abstract class PagedParamData
{
    private int _pageSize = 50;
    private int _pageNumber = 1;

    [Range(1, int.MaxValue)]
    public int? PageNumber
    {
        get => _pageNumber;
        set
        {
            _pageNumber = (value.HasValue && value.Value > 0)
                ? value.Value
                : 1;
        }
    }

    public int? PageSize
    {
        get => _pageSize;
        set
        {
            if (!value.HasValue)
            {
                _pageSize = 50;
                return;
            }

            _pageSize = Math.Clamp(value.Value, 50, 501);
        }
    }
}
