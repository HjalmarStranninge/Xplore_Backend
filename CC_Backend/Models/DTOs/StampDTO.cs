namespace CC_Backend.Models.DTOs
{
    public class StampDTO
    {
        public int StampId { get; set; }
        public string Name { get; set; }
        public string? Facts { get; set; }
        public double? Rarity { get; set; }
        public byte[]? Icon { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public CategoryDTO Category { get; set; }
    }
}
