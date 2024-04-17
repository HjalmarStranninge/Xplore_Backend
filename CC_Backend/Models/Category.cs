namespace CC_Backend.Models
{
    public class Category
    {
        private int _categoryId { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Stamp> Stamps { get; set; }
    }
}
