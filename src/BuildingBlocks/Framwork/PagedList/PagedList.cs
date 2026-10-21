namespace Framwork.PagedList
{
    public class PagedList<T>
    {
        public List<T> List { get; set; } = [];

        public PagedListInfo Pagination { get; set; } = new();

        public object? ExtraData { get; set; }
    }

    public class PagedList<T, TExtra> : PagedList<T>
    {
        public new TExtra? ExtraData { get; set; }
    }

}
