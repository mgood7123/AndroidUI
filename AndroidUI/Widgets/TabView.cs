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

using AndroidUI.Graphics;
using AndroidUI.Utils;

namespace AndroidUI.Widgets
{
    public class TabView : LinearLayout
    {
        FlywheelScrollView tabContainer;
        LinearLayout tabs;
        FrameLayout tabContent;

        public TabView()
        {
            setOrientation(OrientationMode.HORIZONTAL);
            InitTabView();
        }

        public void addTab(string title, View content)
        {
            Topten_RichTextKit_TextView a = new();
            a.setText(title);
            a.setTextSize(16);
            a.setTag(content);
            a.setOnClickListener(v =>
            {
                tabContent.removeAllViews();
                tabContent.addView((View)v.getTag(), MATCH_PARENT_W__MATCH_PARENT_H);
            });
            tabs.addView(a, new LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.WRAP_CONTENT));
        }

        public void addTab(string title, RunnableWithReturn<View> builder)
        {
            addTab(title, builder.Invoke());
        }

        private void InitTabView()
        {
            tabs = new();
            tabContainer = new();
            tabContent = new();

            tabs.setOrientation(OrientationMode.VERTICAL);

            tabContainer.SmoothScroll = true;

            tabContainer.addView(tabs);

            addView(tabContainer, new LayoutParams(View.LayoutParams.WRAP_CONTENT, View.LayoutParams.MATCH_PARENT));
            addView(tabContent, new LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1));
            tabContainer.setZ(1);
            tabContainer.setBackgroundColor(Color.BLACK);
        }
    }
}
