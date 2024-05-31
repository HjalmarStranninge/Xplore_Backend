using CC_Backend.Models.DTOs;

namespace CC_Backend.Models.Viewmodels
{
    public class SelectedStampViewModel
    {
        public int StampId { get; set; }
        public string Name { get; set; }
        public string? Facts { get; set; }
        public string? Rarity { get; set; }
        public byte[]? Icon { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public CategoryDTO Category { get; set; }

    }
}
