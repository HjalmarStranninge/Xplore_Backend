namespace CC_Backend.Models
{
    public class Icon
    {
        private int _iconId { get; set; }
        public virtual Stamp Stamp { get; set; }
        public byte[] IconData { get; set; }
    }
}
