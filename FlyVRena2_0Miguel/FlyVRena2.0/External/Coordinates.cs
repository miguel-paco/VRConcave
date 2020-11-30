using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OpenCV.Net;

namespace FlyVRena2._0.External
{
    public class Coordinates : INotifyPropertyChanged
    {
        Point2d _millimeters_curve;

        const float fl = 220f; // focal length of the high resolution camera (in millimeters)
        const float zo = 0f; // z coordinate of the projection plane (in millimeters)

        public virtual Point2d MillimetersCurve
        {
            get => _millimeters_curve;
            set
            {
                if (_millimeters_curve != value)
                {
                    _millimeters_curve = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public virtual Point2d PixelsCurve
        {
            get => MMc2PXc(MillimetersCurve);
            set
            {
                MillimetersCurve = PXc2MMc(value);
                NotifyPropertyChanged();
            }
        }

        public virtual Point2d VoltageCurve
        {
            get => MMc2VLc(MillimetersCurve);
            set
            {
                MillimetersCurve = VLc2MMc(value);
                NotifyPropertyChanged();
            }
        }

        public virtual Point2d MillimetersLine
        {
            get => MMc2MMl(MillimetersCurve);
            set
            {
                MillimetersCurve = MMl2MMc(value);
                NotifyPropertyChanged();
            }
        }

        public virtual Point2d VirtualRealityLine
        {
            get => MMl2VRl(MMc2MMl(MillimetersCurve));
            set
            {
                MillimetersCurve = MMl2MMc(VRl2MMl(value));
                NotifyPropertyChanged();
            }
        }

        protected virtual Point2d MMc2PXc(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = 0.00077075f;
            const float b = -0.012556f;
            const float c = 0.71356f;
            const float d = -0.011993f;
            const float e = -0.00016853f;
            const float f = 0.70037f;
            const float g = 2.4161e-06f;
            const float h = 2.6058e-07f;
            const float i = 0.0034603f;

            // If there is a displacement between the current image and the calibrated image (set to 0 if there is no displacement)
            const float dispX = 0f;
            const float dispY = 0f;

            newCoord.X = (a * (value.X + dispX) + b * (value.Y + dispY) + c) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);
            newCoord.Y = (d * (value.X + dispX) + e * (value.Y + dispY) + f) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);

            return newCoord;
        }

        protected virtual Point2d PXc2MMc(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = -6.0758e-05f;
            const float b = 0.0034711f;
            const float c = -0.69004f;
            const float d = 0.0034354f;
            const float e = 7.5192e-05f;
            const float f = -0.72365f;
            const float g = -2.1675e-07f;
            const float h = -2.4211e-06f;
            const float i = -0.011991f;

            // If there is a displacement between the current image and the calibrated image (set to 0 if there is no displacement)
            const float dispX = 0f;
            const float dispY = 0f;

            newCoord.X = (a * (value.X + dispX) + b * (value.Y + dispY) + c) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);
            newCoord.Y = (d * (value.X + dispX) + e * (value.Y + dispY) + f) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);

            return newCoord;
        }

        protected virtual Point2d MMc2VLc(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = -0.0049679f;
            const float b = -0.0012067f;
            const float c = 0.79842f;
            const float d = -0.0013699f;
            const float e = 0.0056002f;
            const float f = 0.60203f;
            const float g = 1.3113e-07f;
            const float h = 4.4305e-07f;
            const float i = 0.0048541f;

            // If there is a displacement between the current image and the calibrated image (set to 0 if there is no displacement)
            const float dispX = 0f;
            const float dispY = 0f;

            newCoord.X = (a * (value.X + dispX) + b * (value.Y + dispY) + c) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);
            newCoord.Y = (d * (value.X + dispX) + e * (value.Y + dispY) + f) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);

            return newCoord;
        }

        protected virtual Point2d VLc2MMc(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = -0.004864f;
            const float b = -0.0011228f;
            const float c = 0.93935f;
            const float d = -0.0012151f;
            const float e = 0.0043757f;
            const float f = -0.34284f;
            const float g = 1.6624e-07f;
            const float h = -3.9287e-07f;
            const float i = 0.005341f;

            // If there is a displacement between the current image and the calibrated image (set to 0 if there is no displacement)
            const float dispX = 0f;
            const float dispY = 0f;

            newCoord.X = (a * (value.X + dispX) + b * (value.Y + dispY) + c) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);
            newCoord.Y = (d * (value.X + dispX) + e * (value.Y + dispY) + f) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);

            return newCoord;
        }

        protected virtual Point2d MMc2MMl(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            float h = Convert.ToSingle(Math.Sqrt(value.X * value.X + value.Y * value.Y));

            if (h == 0)
            {
                newCoord.X = 0;
                newCoord.Y = 0;
            }
            else
            {
                newCoord.X = fl * value.X * Math.Sin(h / fl) / h;
                newCoord.Y = fl * value.Y * Math.Sin(h / fl) / h;
            }

            return newCoord;
        }

        protected virtual Point2d MMl2MMc(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            float a = Convert.ToSingle(Math.Sqrt(value.X * value.X + value.Y * value.Y));

            if (a == 0)
            {
                newCoord.X = 0;
                newCoord.Y = 0;
            }
            else
            {
                newCoord.X = fl * value.X * Math.Asin(a / fl) / (a);
                newCoord.Y = fl * value.Y * Math.Asin(a / fl) / (a);
            }

            return newCoord;
        }

        protected virtual Point2d VRl2MMl(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = -0.00044876f;
            const float b = 0.016953f;
            const float c = -0.75324f;
            const float d = 0.017465f;
            const float e = 0.00038315f;
            const float f = -0.64795f;
            const float g = 9.0218e-07f;
            const float h = 1.0749e-05f;
            const float i = 0.11044f;

            // If there is a displacement between the current image and the calibrated image (set to 0 if there is no displacement)
            const float dispX = 0f;
            const float dispY = 0f;

            newCoord.X = (a * (value.X + dispX) + b * (value.Y + dispY) + c) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);
            newCoord.Y = (d * (value.X + dispX) + e * (value.Y + dispY) + f) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);

            return newCoord;
        }

        protected virtual Point2d MMl2VRl(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = 0.0028354f;
            const float b = -0.10811f;
            const float c = -0.61484f;
            const float d = -0.11093f;
            const float e = -0.0028089f;
            const float f = -0.7731f;
            const float g = 1.0772e-05f;
            const float h = 1.162e-06f;
            const float i = -0.017033f;

            // If there is a displacement between the current image and the calibrated image (set to 0 if there is no displacement)
            const float dispX = 0f;
            const float dispY = 0f;

            newCoord.X = (a * (value.X + dispX) + b * (value.Y + dispY) + c) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);
            newCoord.Y = (d * (value.X + dispX) + e * (value.Y + dispY) + f) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);

            return newCoord;
        }

        // Used for Graphical Interface (Unused for now)
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
