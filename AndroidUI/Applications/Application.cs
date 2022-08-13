/*
 * Copyright (C) 2006 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using AndroidUI.Execution;
using AndroidUI.Input;
using AndroidUI.Utils;
using AndroidUI.Utils.Widgets;
using AndroidUI.Widgets;
using SkiaSharp;
using static System.Net.Mime.MediaTypeNames;

namespace AndroidUI.Applications
{
    public class Application : ViewParent
    {
        internal Context context;

        public HandlerActionQueue getRunQueue()
        {
            return getViewRootImpl()?.getRunQueue();
        }

        public Context Context => context;

        Handler handler;
        Looper looper;
        public Handler Handler => handler;
        public Looper Looper => looper;

        public Application()
        {
            context = new(this);
            if (looper == null)
            {
                looper = Looper.getMainLooper(context);
                if (looper == null)
                {
                    Looper.prepareMainLooper(context);
                    looper = Looper.getMainLooper(context);
                }
                handler = new Handler(looper);
            }
            context.mAttachInfo.mViewRootImpl = new(context, looper);
        }

        ~Application()
        {
            context.mAttachInfo.mViewRootImpl.DestroyHandler();
            if (looper != null)
            {
                looper.quitSafely();
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Looper.loopUI(context);
                }
                else
                {
                    try
                    {
                        Looper.loopUI(context);
                    }
                    catch (Exception e)
                    {
                        Log.v("Application", "Caught exception while looping UI: " + e);
                    }
                }
                looper = null;
                handler = null;
            }
        }

        public void OnScreenDensityChanged()
        {
            context.mAttachInfo.mViewRootImpl.OnScreenDensityChanged();
        }

        private ViewRootImpl getViewRootImpl()
        {
            return context.mAttachInfo.mViewRootImpl;
        }

        private ApplicationDelegate applicationDelegate;

        internal ApplicationDelegate GetDelegate()
        {
            return applicationDelegate;
        }

        internal void SetDelegate(ApplicationDelegate value)
        {
            applicationDelegate = value;
        }

        public void SetContentView(View view)
        {
            SetContentView(view, new View.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT));
        }

        public View GetContentView()
        {
            return context.mAttachInfo.mViewRootImpl.getContentView();
        }

        public void SetContentView(View view, View.LayoutParams layoutParams)
        {
            context.mAttachInfo.mViewRootImpl.setContentView(view, layoutParams);
        }

        public virtual void OnCreate()
        {

        }

        public virtual void OnPause()
        {

        }

        public virtual void OnResume()
        {

        }

        public void invalidate() => applicationDelegate?.invalidate();

        public void INTERNAL_ERROR(string error) => applicationDelegate?.INTERNAL_ERROR(error);

        internal void handleAppVisibility(bool isVisible)
        {
            context.mAttachInfo.mViewRootImpl.handleAppVisibility(isVisible);
        }

        internal void Draw(Graphics.Canvas canvas)
        {
            if (looper != null)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Looper.loopUI(context);
                }
                else
                {
                    try
                    {
                        Looper.loopUI(context);
                    }
                    catch (Exception e)
                    {
                        Log.v("Application", "Caught exception while looping UI: " + e);
                    }
                }
            }
            if (context.mAttachInfo.mViewRootImpl.hasContent())
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    context.mAttachInfo.mViewRootImpl.draw(canvas);
                }
                else
                {
                    try
                    {
                        context.mAttachInfo.mViewRootImpl.draw(canvas);
                    }
                    catch (Exception e)
                    {
                        Log.v("Application", "Caught exception while drawing: " + e);
                    }
                }
            }
        }

        internal void onSizeChanged(int width, int height)
        {
            context.mAttachInfo.mViewRootImpl.onSizeChanged(width, height);
        }

        public bool isVisible()
        {
            return context.mAttachInfo.mViewRootImpl.mAppVisible;
        }

        public ViewParent getParent()
        {
            return ((ViewParent)context.mAttachInfo.mViewRootImpl).getParent();
        }

        public void requestChildFocus(View view1, View view2)
        {
            ((ViewParent)context.mAttachInfo.mViewRootImpl).requestChildFocus(view1, view2);
        }

        public View focusSearch(View view, int direction)
        {
            return ((ViewParent)context.mAttachInfo.mViewRootImpl).focusSearch(view, direction);
        }

        public void focusableViewAvailable(View view)
        {
            ((ViewParent)context.mAttachInfo.mViewRootImpl).focusableViewAvailable(view);
        }

        public bool isLayoutRequested()
        {
            return ((ViewParent)context.mAttachInfo.mViewRootImpl).isLayoutRequested();
        }

        public void requestLayout()
        {
            ((ViewParent)context.mAttachInfo.mViewRootImpl).requestLayout();
        }

        public bool isLayoutDirectionResolved()
        {
            return ((ViewParent)context.mAttachInfo.mViewRootImpl).isLayoutDirectionResolved();
        }

        public int getLayoutDirection()
        {
            return ((ViewParent)context.mAttachInfo.mViewRootImpl).getLayoutDirection();
        }

        public void onTouch(Touch ev)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                context.mAttachInfo.mViewRootImpl.onTouch(ev);
            }
            else
            {
                try
                {
                    context.mAttachInfo.mViewRootImpl.onTouch(ev);
                }
                catch (Exception e)
                {
                    Log.v("Application", "Caught exception while executing touch event: " + e);
                }
            }
        }

        public void onDescendantInvalidated(View view, View target)
        {
            invalidate();
        }

        public void clearChildFocus(View child)
        {
        }

        public void onDescendantUnbufferedRequested()
        {
            if (getParent() != null)
            {
                getParent().onDescendantUnbufferedRequested();
            }
        }

        public bool canResolveLayoutDirection()
        {
            return false;
        }

        public void requestDisallowInterceptTouchEvent(bool disallowIntercept)
        {
        }

        public void invalidateChild(View view, Rect damage)
        {
        }

        public ViewParent invalidateChildInParent(int[] location, Rect r)
        {
            return null;
        }

        public void SetDensity(float density, int dpi)
        {
            var densityChanged = Context.densityManager.ScreenDensity != density;
            var dpiChanged = Context.densityManager.ScreenDpi != dpi;

            if (densityChanged || dpiChanged)
            {
                Context.densityManager.Set(density, dpi);
                OnScreenDensityChanged();
            }
        }

        public void childDrawableStateChanged(View child)
        {
        }

        public void childHasTransientStateChanged(View child, bool hasTransientState)
        {
        }

        /**
         * Implement this interface to receive a callback when a new display frame is
         * being rendered.  The callback is invoked on the {@link Looper} thread to
         * which the {@link Choreographer} is attached.
         */
        public interface FrameCallback
        {
            /**
             * Called when a new display frame is being rendered.
             * <p>
             * This method provides the time in nanoseconds when the frame started being rendered.
             * The frame time provides a stable time base for synchronizing animations
             * and drawing.  It should be used instead of {@link SystemClock#uptimeMillis()}
             * or {@link System#nanoTime()} for animations and drawing in the UI.  Using the frame
             * time helps to reduce inter-frame jitter because the frame time is fixed at the time
             * the frame was scheduled to start, regardless of when the animations or drawing
             * callback actually runs.  All callbacks that run as part of rendering a frame will
             * observe the same frame time so using the frame time also helps to synchronize effects
             * that are performed by different callbacks.
             * </p><p>
             * Please note that the framework already takes care to process animations and
             * drawing using the frame time as a stable time base.  Most applications should
             * not need to use the frame time information directly.
             * </p>
             *
             * @param frameTimeNanos The time in nanoseconds when the frame started being rendered,
             * in the {@link System#nanoTime()} timebase.  Divide this value by {@code 1000000}
             * to convert it to the {@link SystemClock#uptimeMillis()} time base.
             */
            public abstract void doFrame(long frameTimeNanos);

            public class ActionFrameCallback : FrameCallback
            {
                Runnable<FrameCallback, object, long> action;
                object data;

                public ActionFrameCallback(object data, Runnable<FrameCallback, object, long> action)
                {
                    this.action = action;
                    this.data = data;
                }

                public void doFrame(long frameTimeNanos)
                {
                    action?.Invoke(this, data, frameTimeNanos);
                }
            }

            public static FrameCallback Create(object data, Runnable<FrameCallback, object, long> value)
            {
                return new ActionFrameCallback(data, value);
            }
        }
    }
}