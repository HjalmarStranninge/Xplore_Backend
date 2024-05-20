namespace CC_Backend.Models.DTOs
{
    public class CategoryDTO
    {
        public string Title { get; set; }
        public List<StampDTO> Stamps { get; set; } = new List<StampDTO>();

    }
}
