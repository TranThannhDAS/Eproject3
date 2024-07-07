namespace backend.Dtos.OrderDtos
{
    public class OrderDtos
    {
        public int? Id { get; set; }
        public  double? Price { get; set; }
        public bool IsActive { get; set; }
        public int? Number_people { get; set; }

    }
}
