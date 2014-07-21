
namespace Zbang.Zbox.Store.Dto
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int ParentId { get; set; }
        public int Order { get; set; }

    }
}
