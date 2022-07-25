using AndroidUI.Applications;
using AndroidUI.Utils;
using SkiaSharp.Views.Desktop;
using System.Runtime.InteropServices;

namespace AndroidUI.Hosts.Windows
{
    internal class SkiaGL : SKGLControl
    {
        readonly Host host = new();

        public SkiaGL(Applications.Application application = null)
        {
            host.SetApplication(application);

            host.SetInvalidateCallback(Invalidate);

            DpiChangedAfterParent += SkiaGL_DpiChangedAfterParent;

            handleDpiChange();

            host.OnCreate();

            // VSync is disabled by default, who knows why, enable it
            VSync = true;
        }

        private void SkiaGL_DpiChangedAfterParent(object? sender, EventArgs e)
        {
            handleDpiChange();
        }

        void handleDpiChange()
        {
            int density = DeviceDpi;
            float dpi = 96 / density;
            host.setDensity(dpi, density);
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            host.OnPaint(GRContext, e.BackendRenderTarget, e.Surface);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            host.OnVisibilityChanged(Visible);
        }

        bool mouse_clicked = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouse_clicked = true;
            string identity = "OS_MOUSE";
            Point m = e.Location;
            float x = m.X;
            float y = m.Y;
            float normalized_X = x / Width;
            float normalized_Y = y / Height;
            host.getMultiTouch().addTouch(
                identity,
                x, y,
                normalized_X, normalized_Y
            );
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!mouse_clicked) return;
            string identity = "OS_MOUSE";
            Point m = e.Location;
            float x = m.X;
            float y = m.Y;
            float normalized_X = x / Width;
            float normalized_Y = y / Height;
            host.getMultiTouch().moveTouchBatched(
                identity,
                x, y,
                normalized_X, normalized_Y
            );
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouse_clicked = false;
            string identity = "OS_MOUSE";
            Point m = e.Location;
            float x = m.X;
            float y = m.Y;
            float normalized_X = x / Width;
            float normalized_Y = y / Height;
            host.getMultiTouch().removeTouch(
                identity,
                x, y,
                normalized_X, normalized_Y
            );
        }


        // MULTI TOUCH

        [DllImport("User32.dll", SetLastError = true)]
        private static extern bool GetCurrentInputMessageSource(out INPUT_MESSAGE_SOURCE inputMessageSource);

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT_MESSAGE_SOURCE
        {
            public INPUT_MESSAGE_DEVICE_TYPE deviceType;
            public INPUT_MESSAGE_ORIGIN_ID originId;
        }

        public enum INPUT_MESSAGE_DEVICE_TYPE
        {
            IMDT_UNAVAILABLE = 0x00000000,      // not specified
            IMDT_KEYBOARD = 0x00000001,      // from keyboard
            IMDT_MOUSE = 0x00000002,      // from mouse
            IMDT_TOUCH = 0x00000004,      // from touch
            IMDT_PEN = 0x00000008,      // from pen
            IMDT_TOUCHPAD = 0x00000010,      // from touchpad
        }

        public enum INPUT_MESSAGE_ORIGIN_ID
        {
            IMO_UNAVAILABLE = 0x00000000,  // not specified
            IMO_HARDWARE = 0x00000001,  // from a hardware device or injected by a UIAccess app
            IMO_INJECTED = 0x00000002,  // injected via SendInput() by a non-UIAccess app
            IMO_SYSTEM = 0x00000004,  // injected by the system
        }

        public const int WM_MOUSEFIRST = 0x0200;
        public const int WM_MOUSELAST = 0x020E;
        public const int WM_KEYFIRST = 0x0100;
        public const int WM_KEYLAST = 0x0109;
        public const int WM_TOUCH = 0x0240;
        public const int WM_POINTERWHEEL = 0x024E;

        protected override void WndProc(ref Message m)
        {
            if (
                m.Msg >= WM_MOUSEFIRST && m.Msg <= WM_MOUSELAST
                || m.Msg >= WM_KEYFIRST && m.Msg <= WM_KEYLAST
                || m.Msg >= WM_TOUCH && m.Msg <= WM_POINTERWHEEL
            )
            {
                INPUT_MESSAGE_SOURCE ims = new();
                GetCurrentInputMessageSource(out ims);
                if (ims.deviceType == INPUT_MESSAGE_DEVICE_TYPE.IMDT_MOUSE)
                {
                    base.WndProc(ref m);
                }
                // not tested
                else if (ims.deviceType == INPUT_MESSAGE_DEVICE_TYPE.IMDT_TOUCHPAD)
                {
                    Log.WriteLine("TOUCHPAD INPUT");
                    base.WndProc(ref m);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter)
            {
                Log.WriteLine("");
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
    }
}