using SkiaSharp;

namespace AndroidUI.Extensions
{
    public static class SKBitmapExtensions
    {
        public static void UpdateInfo(this SKBitmap bitmap, SKImageInfo info)
        {
            SKImageInfo old = bitmap.Info;
            SKPixelRef pixelRef = bitmap.PixelRef;
            if (old.BytesSize >= info.BytesSize)
            {
                bitmap.SetInfo(info);
                bitmap.SetPixelRef(pixelRef, 0, 0);
            }
            else
            {
                bitmap.SetInfo(info);
                Console.WriteLine("updating info results in a larger storage");
                // TODO: update pixelmap to reflect new info
            }
        }
    }
}