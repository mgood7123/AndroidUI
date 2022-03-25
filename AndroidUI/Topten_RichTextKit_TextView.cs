using SkiaSharp;
using Topten.RichTextKit;

namespace AndroidUI
{
    public class Topten_RichTextKit_TextView : View
    {
        TextBlock textBlock;
        Style style;
        string text;
        float textSize;
        Unit textSizeUnit;
        SKColor textColor;

        // rebuilding the text might be expensive, so cache changes
        bool textChanged;
        bool textSizeChanged;
        bool textColorChanged;

        public Topten_RichTextKit_TextView() : base()
        {
            textBlock = new();
            style = new();
            setText("Topten_RichTextKit_TextView");
            setTextSize(12);
            setTextColor(SKColors.Aqua);
            setWillDraw(true);
        }

        public void setText(string text)
        {
            if (this.text != text)
            {
                this.text = text;
                textChanged = true;
                update();
            }
        }

        public string getText()
        {
            return text;
        }

        public void setTextSize(float size)
        {
            setTextSize(Unit.DeviceIndependantPixels, size);
        }

        public enum Unit
        {
            Pixels,
            DeviceIndependantPixels
        }

        public void setTextSize(Unit unit, float size)
        {
            textSizeUnit = unit;
            switch (unit)
            {
                case Unit.Pixels:
                    // this can happen when the screen density is 1
                    if (textSize != size)
                    {
                        textSize = size;
                        textSizeChanged = true;
                        update();
                    }
                    break;
                case Unit.DeviceIndependantPixels:
                    {
                        // this can happen when the screen density is 1
                        float newSize = size * DensityManager.ScreenDensityAsFloat;
                        if (textSize != newSize)
                        {
                            textSize = newSize;
                            textSizeChanged = true;
                            update();
                        }
                        break;
                    }
            }
        }

        public int length()
        {
            return text.Length;
        }

        public void setTextColor(SKColor color)
        {
            if (textColor != color)
            {
                textColor = color;
                textColorChanged = true;
                update();
            }
        }

        public SKColor getTextColor()
        {
            return textColor;
        }

        void update()
        {
            if (textChanged || textSizeChanged || textColorChanged)
            {
                style.FontSize = textSize;
                style.TextColor = textColor;
                if (textChanged)
                {
                    lock (textBlock)
                    {
                        textBlock.Clear();
                        textBlock.AddText(text, style);
                    }
                }
                else
                {
                    lock (textBlock)
                    {
                        textBlock.ApplyStyle(0, textBlock.Length, style);
                    }
                }

                textChanged = false;
                textSizeChanged = false;
                textColorChanged = false;
            }
        }

        protected override void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            lock (textBlock)
            {
                int w = 0;
                int widthMode = MeasureSpec.getMode(widthMeasureSpec);

                switch (widthMode)
                {
                    case MeasureSpec.UNSPECIFIED:
                        w = textBlock.MeasuredWidth.toPixel();
                        break;
                    case MeasureSpec.AT_MOST:
                        {
                            int t = textBlock.MeasuredWidth.toPixel();
                            int s = MeasureSpec.getSize(widthMeasureSpec);
                            w = Math.Min(s, t);
                            break;
                        }
                    case MeasureSpec.EXACTLY:
                        w = MeasureSpec.getSize(widthMeasureSpec);
                        break;
                }

                int h = 0;
                int heightMode = MeasureSpec.getMode(heightMeasureSpec);

                switch (heightMode)
                {
                    case MeasureSpec.UNSPECIFIED:
                        h = textBlock.MeasuredHeight.toPixel();
                        break;
                    case MeasureSpec.AT_MOST:
                        {
                            int t = textBlock.MeasuredHeight.toPixel();
                            int s = MeasureSpec.getSize(heightMeasureSpec);
                            h = Math.Min(s, t);
                            break;
                        }
                    case MeasureSpec.EXACTLY:
                        h = MeasureSpec.getSize(heightMeasureSpec);
                        break;
                }

                setMeasuredDimension(w, h);
            }
        }

        protected override void onLayout(bool changed, int l, int t, int r, int b)
        {
            base.onLayout(changed, l, t, r, b);
            lock (textBlock)
            {
                textBlock.MaxWidth = getWidth();
                textBlock.MaxHeight = getHeight();
            }
        }

        protected override void onDraw(SKCanvas canvas)
        {
            lock (textBlock)
            {
                textBlock.Paint(canvas);
            }
        }

        public override void OnScreenDensityChanged()
        {
            if (textSizeUnit == Unit.DeviceIndependantPixels)
            {

            }
            base.OnScreenDensityChanged();
        }
    }
}
