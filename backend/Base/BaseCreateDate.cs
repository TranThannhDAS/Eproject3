namespace webapi.Base
{
    public class BaseCreateDate
    {
        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
