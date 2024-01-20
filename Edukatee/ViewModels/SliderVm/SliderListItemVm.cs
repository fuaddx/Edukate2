namespace Edukatee.ViewModels.SliderVm
{
    public class SliderListItemVm
    {
        public int Id { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? CreatedTime { get; set; }
        public bool IsDeleted { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; }
        public string Profession { get; set; }
    }
}
