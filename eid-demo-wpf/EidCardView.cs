using Egelke.Eid.Client.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace eid_demo_wpf
{
    public class EidCardView : INotifyPropertyChanged
    {
        private BitmapImage _picture;
        private Address _address;
        private Identity _identity;

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapImage Picture
        {
            get
            {
                return _picture;
            }

        }

        public Address Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        public Identity Identity
        {
            get
            {
                return _identity;
            }
            set
            {
                _identity = value;
                OnPropertyChanged();
            }
        }

        public void UpdatePicture(byte[] value)
        {
            _picture = new BitmapImage();
            _picture.BeginInit();
            _picture.StreamSource = new MemoryStream(value);
            _picture.EndInit();

            OnPropertyChanged("Picture");
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
