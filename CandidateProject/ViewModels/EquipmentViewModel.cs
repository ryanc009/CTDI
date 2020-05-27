using System.ComponentModel;

namespace CandidateProject.ViewModels
{
    public class EquipmentViewModel
    {
        public int Id { get; set; }
        [DisplayName("Model Type")]
        public string ModelType { get; set; }
        [DisplayName("Serial Number")]
        public string SerialNumber { get; set; }
    }
}