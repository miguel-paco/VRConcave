using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OpenCV.Net;

namespace FlyVRena2._0.External
{
    public class AngleVal : INotifyPropertyChanged
    {
        float _radians;

        public virtual float Radians
        {
            get => _radians;
            set
            {
                if (_radians != value)
                {
                    _radians = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public virtual float Degrees
        {
            get => Rad2Deg(Radians);
            set
            {
                Radians = Deg2Rad(value);
                NotifyPropertyChanged();
            }
        }

        protected virtual float Deg2Rad(float value)
        {
            float angle;

            angle = (float)(value * Math.PI / 180);

            return angle;
        }
        protected virtual float Rad2Deg(float value)
        {
            float angle;

            angle = (float)(value * 180 / Math.PI);

            return angle;
        }

        // Used for Graphical Interface (Unused for now)
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
