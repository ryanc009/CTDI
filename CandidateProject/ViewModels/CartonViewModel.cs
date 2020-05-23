using System.ComponentModel;

namespace CandidateProject.ViewModels
{
    public class CartonViewModel
    {
        public int Id { get; set; }
        [DisplayName("Carton Number")]
        public string CartonNumber { get; set; }
        [DisplayName("Item Count")]
        public int ItemCount { get; set; }
    }
}