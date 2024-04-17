namespace CC_Backend.Models
{
    public class Geodata
    {
        private int _geodataId { get; set; }
        public string[] Coordinates { get; set; }
        public DateTime DateWhenCollected { get; set; }
        public virtual StampCollected StampCollected { get; set; }
    }
}
