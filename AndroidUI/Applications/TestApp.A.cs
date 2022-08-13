/*
 * this contains some basic test applications that test basic components of AndroidUI
 */

using AndroidUI.Extensions;
using AndroidUI.Graphics;
using AndroidUI.Widgets;
using SkiaSharp;

namespace AndroidUI.Applications
{
    public partial class TestApp
    {
        class A : View
        {

            public A()
            {
                setWillDraw(true);
            }

            protected override void onDraw(Canvas canvas)
            {
                base.onDraw(canvas);
                var bm = BitmapFactory.decodeFile(Context, image_path);
                using var p = new Paint();
                p.setColor(Color.WHITE);
                canvas.DrawBitmap(bm, 0, 0, p);
                bm.recycle();
            }
        }
    }
}
