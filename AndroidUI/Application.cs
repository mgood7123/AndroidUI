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
using SkiaSharp;

namespace AndroidUI
{
    public class Application : Parent
    {
        internal Context context;

        public Context Context => context;

        Handler handler;
        Looper looper;
        public Handler Handler => handler;
        public Looper Looper => looper;

        public Application()
        {
            context = new(this);
            context.mAttachInfo.mViewRootImpl = new(context);
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
            if (isVisible)
            {
                if (looper == null)
                {
                    looper = Looper.myLooper(context);
                    if (looper == null)
                    {
                        Looper.prepare(context);
                        looper = Looper.myLooper(context);
                    }
                    handler = new Handler(looper);
                }
            }
            else
            {
                if (looper != null)
                {
                    looper.quitSafely();
                    Looper.loopUI(context);
                    looper = null;
                    handler = null;
                }
            }
            context.mAttachInfo.mViewRootImpl.handleAppVisibility(isVisible);
        }

        internal void Draw(SKCanvas canvas)
        {
            if (looper != null)
            {
                Looper.loopUI(context);
            }
            if (context.mAttachInfo.mViewRootImpl.hasContent())
            {
                context.mAttachInfo.mViewRootImpl.draw(canvas);
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

        public Parent getParent()
        {
            return ((Parent)context.mAttachInfo.mViewRootImpl).getParent();
        }

        public void requestChildFocus(View view1, View view2)
        {
            ((Parent)context.mAttachInfo.mViewRootImpl).requestChildFocus(view1, view2);
        }

        public View focusSearch(View view, int direction)
        {
            return ((Parent)context.mAttachInfo.mViewRootImpl).focusSearch(view, direction);
        }

        public void focusableViewAvailable(View view)
        {
            ((Parent)context.mAttachInfo.mViewRootImpl).focusableViewAvailable(view);
        }

        public bool isLayoutRequested()
        {
            return ((Parent)context.mAttachInfo.mViewRootImpl).isLayoutRequested();
        }

        public void requestLayout()
        {
            ((Parent)context.mAttachInfo.mViewRootImpl).requestLayout();
        }

        public bool isLayoutDirectionResolved()
        {
            return ((Parent)context.mAttachInfo.mViewRootImpl).isLayoutDirectionResolved();
        }

        public int getLayoutDirection()
        {
            return ((Parent)context.mAttachInfo.mViewRootImpl).getLayoutDirection();
        }

        public void onTouch(Touch ev)
        {
            context.mAttachInfo.mViewRootImpl.onTouch(ev);
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

        public Parent invalidateChildInParent(int[] location, Rect r)
        {
            return null;
        }

        public static void SetDensity(float density, int dpi)
        {
            DensityManager.INTERNAL_USE_ONLY____SET_DENSITY(density, dpi);
        }

        public void childDrawableStateChanged(View child)
        {
        }
    }
}