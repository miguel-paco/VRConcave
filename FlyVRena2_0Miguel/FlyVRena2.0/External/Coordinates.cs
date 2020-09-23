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

        public virtual Point2d PixelsLine
        {
            

            get => MMl2PXl(MMc2MMl(MillimetersCurve));
            set
            {
                MillimetersCurve = MMl2MMc(PXl2MMl(value));
                NotifyPropertyChanged();
            }
        }

        public virtual Point2d VirtualRealityLine
        {


            get => PXl2VRl(MMl2PXl(MMc2MMl(MillimetersCurve)));
            set
            {
                MillimetersCurve = MMl2MMc(PXl2MMl(VRl2PXl(value)));
                NotifyPropertyChanged();
            }
        }

        protected virtual Point2d MMc2PXc(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = 0.00032428f;
            const float b = -0.012138f;
            const float c = 0.7292f;
            const float d = -0.011639f;
            const float e = 0.00022908f;
            const float f = 0.68407f;
            const float g = 4.3019e-06f;
            const float h = 4.6682e-07f;
            const float i = 0.0066826f;

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
            const float a = 9.9966e-05f;
            const float b = 0.0067039f;
            const float c = -0.69716f;
            const float d = 0.0066433f;
            const float e = -7.9659e-05f;
            const float f = -0.71676f;
            const float g = -5.2483e-07f;
            const float h = -4.3019e-06f;
            const float i = -0.011623f;

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
            const float a = -0.0049851f;
            const float b = -0.0011127f;
            const float c = 0.80155f;
            const float d = -0.0011787f;
            const float e = 0.0055754f;
            const float f = 0.59786f;
            const float g = 2.7028e-08f;
            const float h = 4.8385e-08f;
            const float i = 0.0048524f;

            // If there is a displacement between the current image and the calibrated image (set to 0 if there is no displacement)
            const float dispX = 0f;
            const float dispY = 0f;

            newCoord.X = (a * (value.X + dispX) + b * (value.Y + dispY) + c) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);
            newCoord.Y = (d * (value.X + dispX) + e * (value.Y + dispY) + f) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);

            // Voltage can't be higher then 255 or lower then 0
            if (newCoord.X > 255)
            {
                newCoord.X = 255;
            }
            else if (newCoord.X < 0)
            {
                newCoord.X = 0;
            }
            if (newCoord.Y > 255)
            {
                newCoord.Y = 255;
            }
            else if (newCoord.Y < 0)
            {
                newCoord.Y = 0;
            }

            return newCoord;
        }

        protected virtual Point2d VLc2MMc(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = -0.0048928f;
            const float b = -0.00098515f;
            const float c = 0.9296f;
            const float d = -0.0010372f;
            const float e = 0.0043811f;
            const float f = -0.36846f;
            const float g = 1.3285e-08f;
            const float h = -3.9787e-08f;
            const float i = 0.0052722f;

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
                newCoord.X = fl * value.X * Math.Asin(a / fl) / a;
                newCoord.Y = fl * value.Y * Math.Asin(a / fl) / a;
            }

            return newCoord;
        }

        protected virtual Point2d MMl2PXl(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = 0.00076584f;
            const float b = -0.011619f;
            const float c = 0.72296f;
            const float d = -0.011063f;
            const float e = -0.00014624f;
            const float f = 0.69067f;
            const float g = 4.9195e-06f;
            const float h = 4.3975e-07f;
            const float i = 0.0066826f;

            // If there is a displacement between the current image and the calibrated image (set to 0 if there is no displacement)
            const float dispX = 0f;
            const float dispY = 0f;

            newCoord.X = (a * (value.X + dispX) + b * (value.Y + dispY) + c) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);
            newCoord.Y = (d * (value.X + dispX) + e * (value.Y + dispY) + f) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);

            return newCoord;
        }

        protected virtual Point2d PXl2MMl(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = 0.00011013f;
            const float b = -0.0066985f;
            const float c = 0.68038f;
            const float d = -0.0066436f;
            const float e = -0.00013484f;
            const float f = 0.73272f;
            const float g = 3.4988e-07f;
            const float h = 4.9462e-06f;
            const float i = 0.011054f;

            // If there is a displacement between the current image and the calibrated image (set to 0 if there is no displacement)
            const float dispX = 0f;
            const float dispY = 0f;

            newCoord.X = (a * (value.X + dispX) + b * (value.Y + dispY) + c) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);
            newCoord.Y = (d * (value.X + dispX) + e * (value.Y + dispY) + f) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);

            return newCoord;
        }

        protected virtual Point2d VRl2PXl(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = -0.0016511f;
            const float b = 0.00014096f;
            const float c = 0.71491f;
            const float d = 3.4698e-05f;
            const float e = -0.0014769f;
            const float f = 0.69919f;
            const float g = 2.2642e-07f;
            const float h = 1.2989e-06f;
            const float i = 0.0060523f;

            // If there is a displacement between the current image and the calibrated image (set to 0 if there is no displacement)
            const float dispX = 0f;
            const float dispY = 0f;

            newCoord.X = (a * (value.X + dispX) + b * (value.Y + dispY) + c) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);
            newCoord.Y = (d * (value.X + dispX) + e * (value.Y + dispY) + f) / (g * (value.X + dispX) + h * (value.Y + dispY) + i);

            return newCoord;
        }

        protected virtual Point2d PXl2VRl(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = -0.0059665f;
            const float b = 4.6024e-05f;
            const float c = 0.69947f;
            const float d = -3.1603e-05f;
            const float e = -0.0061535f;
            const float f = 0.71461f;
            const float g = 2.2862e-07f;
            const float h = 1.3212e-06f;
            const float i = 0.0014746f;

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
