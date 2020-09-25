﻿using System;
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
            const float a = 0.00073814f;
            const float b = -0.012449f;
            const float c = 0.71826f;
            const float d = -0.011906f;
            const float e = -0.00015285f;
            const float f = 0.69555f;
            const float g = 2.3365e-06f;
            const float h = 2.3884e-07f;
            const float i = 0.0034217f;

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
            const float a = -5.5156e-05f;
            const float b = 0.0034323f;
            const float c = -0.68613f;
            const float d = 0.0033991f;
            const float e = 6.8197e-05f;
            const float f = -0.72737f;
            const float g = -2.0012e-07f;
            const float h = -2.3404e-06f;
            const float i = -0.011905f;

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
            const float a = -0.0049183f;
            const float b = -0.0011707f;
            const float c = 0.79832f;
            const float d = -0.0013386f;
            const float e = 0.0056305f;
            const float f = 0.60216f;
            const float g = 4.1358e-07f;
            const float h = 6.719e-07f;
            const float i = 0.0048531f;

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
            const float a = -0.0048642f;
            const float b = -0.0011238f;
            const float c = 0.93966f;
            const float d = -0.0012179f;
            const float e = 0.004371f;
            const float f = -0.34199f;
            const float g = 4.7421e-07f;
            const float h = -5.4006e-07f;
            const float i = 0.0053076f;

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

        protected virtual Point2d VRl2MMl(Point2d value)
        {
            // variable to save the coordinates in the new units
            Point2d newCoord;

            // Conversion Matrix Values: 3x3 Matrix [a,b,c; d,e,f; g,h,i]
            // (set 'a','e'&'i' to 1 and the rest to 0 - identity matrix - if there is no transformation)
            const float a = 0.00046338f;
            const float b = -0.016657f;
            const float c = 0.76363f;
            const float d = -0.017203f;
            const float e = -0.00036936f;
            const float f = 0.63595f;
            const float g = -1.2362e-06f;
            const float h = -1.1074e-05f;
            const float i = -0.10898f;

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
            const float a = -0.002758f;
            const float b = 0.10644f;
            const float c = 0.60186f;
            const float d = 0.10945f;
            const float e = 0.0028918f;
            const float f = 0.78369f;
            const float g = -1.1103e-05f;
            const float h = -1.4985e-06f;
            const float i = 0.016733f;

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
