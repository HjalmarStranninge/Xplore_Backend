using CC_Backend.Models.Viewmodels;

namespace CC_Backend.Models.Viewmodels
{
    public class CategoryWithStampListViewModel
    {
        public string Title { get; set; }
        public List<SelectedStampViewModel> Stamps { get; set; } = new List<SelectedStampViewModel>();
    }
}
