using System.Collections.Generic;
using System.ComponentModel;

namespace CandidateProject.ViewModels
{
    public class CartonDetailsViewModel
    {
        public int CartonId { get; set; }

        [DisplayName("Carton Number")]
        public string CartonNumber { get; set; }
        public IEnumerable<EquipmentViewModel> Equipment { get; set; }
        public CartonDetailsViewModel()
        {
            Equipment = new List<EquipmentViewModel>();
        }
        public string MessageText { get; set; }
        public string CartonEquipmentCount { get; set; }
    }
}