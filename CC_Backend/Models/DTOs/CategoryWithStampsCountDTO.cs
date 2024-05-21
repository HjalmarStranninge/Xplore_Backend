namespace CC_Backend.Models.DTOs
{
    public class CategoryWithStampsCountDTO
    {
        public string Title { get; set; }
        public int CollectedStamps { get; set; }
        public int TotalStamps { get; set; }
    }
}
