﻿using System.Collections.ObjectModel;

namespace WpfApp1
{
    public class KeyViewModel
    {
        public ObservableCollection<KeyModel> Keys { get; private set; }

        public KeyViewModel() => Keys = new ObservableCollection<KeyModel>();
    }
}