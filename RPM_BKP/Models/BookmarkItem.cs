namespace RPM_BKP.Models
{
    public class BookmarkItem
    {
        public long Id { get; set; }
        public long Parent { get; set; }
        public int Type { get; set; } // 1 = URL, 2 = Pasta
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
