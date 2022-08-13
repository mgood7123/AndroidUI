/*
 * this contains some basic test applications that test basic components of AndroidUI
 */

using AndroidUI.AnimationFramework.Animator;

namespace AndroidUI.Applications
{
    public partial class TestApp
    {
        class AL : AnimatorListenerAdapter
        {
            public override void onAnimationEnd(Animator animation)
            {
                animation.start();
            }
        }
    }
}
