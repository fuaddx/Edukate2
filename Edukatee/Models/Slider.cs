namespace Edukatee.Models
{
    public class Slider:BaseEntity
    {
        public DateTime? CreatedTime { get; set; }
        public bool  IsDeleted { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; }
        public string Profession { get; set; }
    }
}
