using AndroidUI.Applications;
using SkiaSharp;

namespace AndroidUI.Graphics
{
    public class BaseCanvas : LoggingCanvas
    {
        internal Context context;
        internal int width, height;

        internal bool hardwareAccelerated;
        internal GRContext graphicsContext;
        internal SKSurface surface;

        private int densityDPI;
        private bool ownsSurface;

        public int DensityDPI { get => densityDPI; set => densityDPI = value; }

        /// <summary>
        /// if true, the surface will be disposed, otherwise the surface is not disposed even if DisposeSurface is called
        /// </summary>
        public bool OwnsSurface { get => ownsSurface; set => ownsSurface = value; }

        public BaseCanvas()
        {
        }

        public BaseCanvas(Context context, SKCanvas canvas) : base(canvas)
        {
            this.context = context;
            if (canvas is BaseCanvas baseCanvas)
            {
                width = baseCanvas.width;
                height = baseCanvas.height;
            }
            densityDPI = context.densityManager.ScreenDpi;
        }

        

        public BaseCanvas(Context context, SKCanvas canvas, bool ownsCanvas) : base(canvas, ownsCanvas)
        {
            this.context = context;
            if (canvas is BaseCanvas baseCanvas)
            {
                width = baseCanvas.width;
                height = baseCanvas.height;
            }
            densityDPI = context.densityManager.ScreenDpi;
        }

        public void SetContext(Context context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected override void OnDispose()
        {
            DisposeSurface();
            base.OnDispose();
        }

        /// <summary>
        /// Disposes a surface created from CreateHardwareCanvas
        /// <br></br>
        /// if disposing the canvas, the surface automatically disposed as well
        /// </summary>
        public void DisposeSurface()
        {
            if (surface != null)
            {
                if (ownsSurface) surface.Dispose();
                surface = null;
            }
        }

        // TODO: should this be public?
        internal void setWidthHeight(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>Returns true if this canvas is Hardware Accelerated.</summary>
        public bool isHardwareAccelerated()
        {
            return hardwareAccelerated;
        }

        internal void setIsHardwareAccelerated(bool value)
        {
            hardwareAccelerated = value;
        }

        /// <summary>Adopts a Hardware Accelerated canvas. Can be used to store the values required to create a Hardwate Accelerated canvas inside of another canvas</summary>
        public void AdoptHardwareAcceleratedCanvas(BaseCanvas hardwareAcceleratedCanvas, int width, int height)
        {
            AdoptHardwareAcceleratedCanvas(hardwareAcceleratedCanvas);
            setWidthHeight(width, height);
        }

        /// <summary>Adopts a Hardware Accelerated canvas. Can be used to store the values required to create a Hardwate Accelerated canvas inside of another canvas</summary>
        public void AdoptHardwareAcceleratedCanvas(BaseCanvas hardwareAcceleratedCanvas)
        {
            if (hardwareAcceleratedCanvas.graphicsContext == null)
            {
                throw new ArgumentNullException(nameof(context), "attempting to adopt a Hardware Accelerated canvas that has no GRContext associated with it");
            }
            hardwareAccelerated = hardwareAcceleratedCanvas.hardwareAccelerated;
            graphicsContext = hardwareAcceleratedCanvas.graphicsContext;
            surface = hardwareAcceleratedCanvas.surface;
            ownsSurface = false;
        }

        /// <summary>Creates a Hardware Accelerated canvas.</summary>
        public static T CreateHardwareAcceleratedCanvas<T>(Context context, GRContext graphicsContext, int width, int height)
            where T : BaseCanvas, new()
        {
            T canvas = CreateHardwareAcceleratedCanvas<T>(graphicsContext, width, height);
            if (canvas == null)
            {
                return null;
            }
            canvas.SetContext(context);
            return canvas;
        }

        /// <summary>Creates a Hardware Accelerated canvas.</summary>
        internal static T CreateHardwareAcceleratedCanvas<T>(GRContext graphicsContext, int width, int height)
            where T : BaseCanvas, new()
        {
            if (width == 0 || height == 0 || graphicsContext == null)
            {
                return null;
            }

            SKSurface s = SKSurface.Create(graphicsContext, false, new SKImageInfo(width, height));

            if (s == null)
            {
                return null;
            }

            T c = new();
            c.SetNativeObject(s.Canvas);

            c.setWidthHeight(width, height);

            c.graphicsContext = graphicsContext;
            c.surface = s;
            c.hardwareAccelerated = true;
            c.ownsSurface = true;

            return c;
        }

        /// <summary>Creates a Hardware Accelerated canvas from an existing surface.</summary>
        internal static T CreateHardwareAcceleratedCanvas<T>(GRContext graphicsContext, SKSurface surface, int width, int height)
            where T : BaseCanvas, new()
        {
            if (width == 0 || height == 0 || surface == null)
            {
                return null;
            }

            T c = new();

            // we do not own the surface canvas
            c.SetNativeObject(surface.Canvas, false);

            c.setWidthHeight(width, height);

            c.graphicsContext = graphicsContext;
            c.surface = surface;
            c.hardwareAccelerated = true;
            c.ownsSurface = false;

            return c;
        }

        /// <summary>Creates a Hardware Accelerated canvas.</summary>
        public T CreateHardwareAcceleratedCanvas<T>(int width, int height) where T : BaseCanvas, new()
        {
            return CreateHardwareAcceleratedCanvas<T>(context, graphicsContext, width, height);
        }

        /// <summary>Creates a software canvas, this is not Hardware Accelerated.</summary>
        public static T CreateSoftwareCanvas<T>(Context context, int width, int height) where T : BaseCanvas, new()
        {
            if (width == 0 || height == 0 || context == null)
            {
                return null;
            }

            var bitmap = new SKBitmap(width, height);
            if (bitmap == null)
            {
                return null;
            }

            SKCanvas canvas = new(bitmap);
            if (canvas == null)
            {
                bitmap.Dispose();
                return null;
            }

            T c = new();
            c.SetNativeObject(canvas);
            c.SetContext(context);
            c.setWidthHeight(width, height);
            c.hardwareAccelerated = false;
            return c;
        }

        public T CreateSoftwareCanvas<T>(int width, int height) where T : BaseCanvas, new()
        {
            return CreateSoftwareCanvas<T>(context, width, height);
        }

        /// <summary>Creates a canvas, whether the created canvas is Hardware Accelerated or not depends
        /// on the canvas of which this method is invoked from.</summary>
        public T CreateCanvas<T>(int width, int height) where T : BaseCanvas, new()
        {
            if (width == 0 || height == 0) return null;

            if (isHardwareAccelerated())
            {
                return CreateHardwareAcceleratedCanvas<T>(width, height);
            }
            else
            {
                return CreateSoftwareCanvas<T>(width, height);
            }
        }

        public int getWidth()
        {
            return width;
        }

        public int getHeight()
        {
            return height;
        }
    }
}