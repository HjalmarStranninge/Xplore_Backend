namespace CC_Backend.Models.DTOs
{
    public class CategoryWithStampListDTO
    {
        public string Title { get; set; }
        public List<StampDTO> Stamps { get; set; } = new List<StampDTO>();

    }
}
