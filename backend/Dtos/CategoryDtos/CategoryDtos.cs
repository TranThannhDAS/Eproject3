using webapi.Base;

namespace backend.Dtos.CategoryDtos
{
    public class CategoryDtos:BaseCreateDate
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
