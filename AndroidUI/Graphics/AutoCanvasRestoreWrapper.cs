using AndroidUI.Utils;

namespace AndroidUI.Graphics
{
    public class AutoCanvasRestoreWrapper : Disposable
    {
        private CanvasWrapper canvas;
        private readonly int saveCount;

        public AutoCanvasRestoreWrapper(CanvasWrapper canvas)
            : this(canvas, true)
        {
        }

        public AutoCanvasRestoreWrapper(CanvasWrapper canvas, bool doSave)
        {
            this.canvas = canvas;
            this.saveCount = 0;

            if (canvas != null)
            {
                saveCount = canvas.SaveCount;
                if (doSave)
                {
                    canvas.Save();
                }
            }
        }

        protected override void OnDispose()
        {
            Restore();
        }

        /// <summary>
        /// Perform the restore now, instead of waiting for the Dispose.
        /// Will only do this once.
        /// </summary>
        public virtual void Restore()
        {
            if (canvas != null)
            {
                canvas.RestoreToCount(saveCount);
                canvas = null;
            }
        }
    }
}