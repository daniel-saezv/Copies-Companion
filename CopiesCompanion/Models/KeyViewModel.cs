using System.Collections.ObjectModel;

namespace CopiesCompanion.Models
{
    public class KeyViewModel
    {
        public ObservableCollection<KeyModel> Keys { get; private set; }

        public KeyViewModel() => Keys = new ObservableCollection<KeyModel>();
    }
}