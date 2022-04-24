using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HRTools_v2.DPI
{
    public class DpiDecorator : Decorator
    {
        public DpiDecorator()
        {
            this.Loaded += (s, e) =>
            {

                Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                //if (m.M11 > 1.5 && m.M22 > 1.5) return;
                ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
                if (dpiTransform.CanFreeze)
                    dpiTransform.Freeze();
                this.LayoutTransform = dpiTransform;
            };
        }
    }
}
