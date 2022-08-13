using AndroidUI.Graphics;
using SkiaSharp;

namespace AndroidUI.Widgets
{
    public class ColorView : View
    {
        public ColorView()
        {
            setWillDraw(true);
        }

        public ColorView(SKColor color)
        {
            setWillDraw(true);
            Color = color;
        }

        public ColorView(Color color)
        {
            setWillDraw(true);
            Color = color.toSKColor();
        }

        public ColorView(int color)
        {
            setWillDraw(true);
            Color = Graphics.Color.toSKColor(color);
        }

        private SKPaint paint = new() { Color = new SKColor(50, 50, 50) };

        public SKColor Color
        {
            get => paint.Color; set
            {
                paint.Color = value;
                invalidate();
            }
        }

        SKRect rect;

        protected override void onSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.onSizeChanged(w, h, oldw, oldh);
            rect = new(0, 0, w, h);
        }

        protected override void onDraw(Canvas canvas)
        {
            canvas.DrawRect(rect, paint);
        }
    }
}
