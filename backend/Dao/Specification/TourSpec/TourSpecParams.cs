namespace backend.Dao.Specification.TourSpec
{
    public class TourSpecParams
    {
        private const int MaxPageSize = 10;
        public int PageIndex { get; set; } = 1;

        private int pageSize = 5;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

        private string? search;

        public string? Location { get; set; }
        public string? Category_Tour { get; set; }
        public int? Rating { get; set; }
        public DateTime? Departure_Time { get; set; }
        public double? Price { get; set; }

        public string? Search
        {
            get { return search; }
            set
            {
                search = value.ToLower();
            }
        }
    }
}
