using AndroidUI.Applications;
using SkiaSharp;

namespace AndroidUI.Graphics
{
    public class RecordingCanvas : Canvas
    {
        SKPictureRecorder recorder = null;

        public RecordingCanvas(Context context)
        {
            SetContext(context);
        }

        public void BeginRecording(int width, int height)
        {
            if (recorder != null)
            {
                throw new Exception("attempting to begin a recording on an already began recording");
            }
            recorder = new();
            base.SetNativeObject(recorder.BeginRecording(new SKRect(0, 0, width, height)));
        }

        public override void SetNativeObject(SKCanvas canvas)
        {
            throw new NotSupportedException();
        }

        public override SKCanvas ReleaseNativeObject()
        {
            throw new NotSupportedException();
        }

        public SKPicture EndRecording()
        {
            if (recorder == null)
            {
                throw new Exception("attempting to end a recording on an already ended recording");
            }
            var picture = recorder.EndRecording();
            base.ReleaseNativeObject().Dispose();
            recorder.Dispose();
            recorder = null;
            return picture;
        }

        public SKDrawable EndRecordingAsDrawable()
        {
            if (recorder == null)
            {
                throw new Exception("attempting to end a recording on an already ended recording");
            }
            var drawable = recorder.EndRecordingAsDrawable();
            base.ReleaseNativeObject().Dispose();
            recorder.Dispose();
            recorder = null;
            return drawable;
        }

        protected override void OnDispose()
        {
            if (recorder != null)
            {
                EndRecording();
            }
            base.OnDispose();
        }
    }
}