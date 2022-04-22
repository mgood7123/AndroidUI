#if ANDROID
using Microsoft.Maui.Handlers;

namespace OtherPlatforms.Platforms.Android
{
    public interface ISkiaGLView : IView
    {
    }

    public class SkiaGLView : View, ISkiaGLView
    {
    }

	public partial class SkiaGLViewHandler : ViewHandler<ISkiaGLView, SkiaGL>
	{
		public static IPropertyMapper<ISkiaGLView, SkiaGLViewHandler> Mapper = new PropertyMapper<ISkiaGLView, SkiaGLViewHandler>(ViewMapper)
		{
		};

		public static CommandMapper<ISkiaGLView, SkiaGLViewHandler> CommandMapper = new(ViewCommandMapper)
		{
		};

		public SkiaGLViewHandler() : base(Mapper, CommandMapper)
		{

		}

		public SkiaGLViewHandler(IPropertyMapper mapper) : base(mapper ?? Mapper)
		{

		}

        protected override SkiaGL CreatePlatformView() => new SkiaGL(Context);
    }
}
#endif