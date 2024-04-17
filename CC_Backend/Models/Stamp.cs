namespace CC_Backend.Models
{
    public class Stamp
    {
        private int _stampId { get; }
        public string Name { get; }
        public string[] Facts { get; }
        public double Rarity { get; }
        public virtual Icon Icon { get; }
        public virtual Category Category { get;}

    }
}
