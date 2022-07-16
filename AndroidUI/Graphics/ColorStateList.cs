/*
 * Copyright (C) 2021 The Android Open Source Project
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

using AndroidUI.Utils;
using AndroidUI.Utils.Arrays;
using AndroidUI.Utils.Graphics;
using SkiaSharp;

namespace AndroidUI.Graphics
{
    /**
     *
     * Lets you map {@link android.view.View} state sets to colors.
     * <p>
     * {@link android.content.res.ColorStateList}s are created from XML resource files defined in the
     * "color" subdirectory directory of an application's resource directory. The XML file contains
     * a single "selector" element with a number of "item" elements inside. For example:
     * <pre>
     * &lt;selector xmlns:android="http://schemas.android.com/apk/res/android"&gt;
     *   &lt;item android:state_focused="true"
     *           android:color="@color/sample_focused" /&gt;
     *   &lt;item android:state_pressed="true"
     *           android:state_enabled="false"
     *           android:color="@color/sample_disabled_pressed" /&gt;
     *   &lt;item android:state_enabled="false"
     *           android:color="@color/sample_disabled_not_pressed" /&gt;
     *   &lt;item android:color="@color/sample_default" /&gt;
     * &lt;/selector&gt;
     * </pre>
     *
     * This defines a set of state spec / color pairs where each state spec specifies a set of
     * states that a view must either be in or not be in and the color specifies the color associated
     * with that spec.
     *
     * <a name="StateSpec"></a>
     * <h3>State specs</h3>
     * <p>
     * Each item defines a set of state spec and color pairs, where the state spec is a series of
     * attributes set to either {@code true} or {@code false} to represent inclusion or exclusion. If
     * an attribute is not specified for an item, it may be any value.
     * <p>
     * For example, the following item will be matched whenever the focused state is set; any other
     * states may be set or unset:
     * <pre>
     * &lt;item android:state_focused="true"
     *         android:color="@color/sample_focused" /&gt;
     * </pre>
     * <p>
     * Typically, a color state list will reference framework-defined state attributes such as
     * {@link android.R.attr#state_focused android:state_focused} or
     * {@link android.R.attr#state_enabled android:state_enabled}; however, app-defined attributes may
     * also be used.
     * <p>
     * <strong>Note:</strong> The list of state specs will be matched against in the order that they
     * appear in the XML file. For this reason, more-specific items should be placed earlier in the
     * file. An item with no state spec is considered to match any set of states and is generally
     * useful as a final item to be used as a default.
     * <p>
     * If an item with no state spec is placed before other items, those items
     * will be ignored.
     *
     * <a name="ItemAttributes"></a>
     * <h3>Item attributes</h3>
     * <p>
     * Each item must define an {@link android.R.attr#color android:color} attribute, which may be
     * an HTML-style hex color, a reference to a color resource, or -- in API 23 and above -- a theme
     * attribute that resolves to a color.
     * <p>
     * Starting with API 23, items may optionally define an {@link android.R.attr#alpha android:alpha}
     * attribute to modify the base color's opacity. This attribute takes a either floating-point value
     * between 0 and 1 or a theme attribute that resolves as such. The item's overall color is
     * calculated by multiplying by the base color's alpha channel by the {@code alpha} value. For
     * example, the following item represents the theme's accent color at 50% opacity:
     * <pre>
     * &lt;item android:state_enabled="false"
     *         android:color="?android:attr/colorAccent"
     *         android:alpha="0.5" /&gt;
     * </pre>
     * <p>
     * Starting with API 31, items may optionally define an {@link android.R.attr#lStar android:lStar}
     * attribute to modify the base color's perceptual luminance. This attribute takes either a
     * floating-point value between 0 and 100 or a theme attribute that resolves as such. The item's
     * overall color is calculated by converting the base color to an accessibility friendly color space
     * and setting its L* to the value specified on the {@code lStar} attribute. For
     * example, the following item represents the theme's accent color at 50% perceptual luminance:
     * <pre>
     * &lt;item android:state_enabled="false"
     *         android:color="?android:attr/colorAccent"
     *         android:lStar="50" /&gt;
     * </pre>
     *
     * <a name="DeveloperGuide"></a>
     * <h3>Developer guide</h3>
     * <p>
     * For more information, see the guide to
     * <a href="{@docRoot}guide/topics/resources/color-list-resource.html">Color State
     * List Resource</a>.
     *
     * @attr ref android.R.styleable#ColorStateListItem_alpha
     * @attr ref android.R.styleable#ColorStateListItem_color
     * @attr ref android.R.styleable#ColorStateListItem_lStar
     */
    public class ColorStateList : ComplexColor
    {
        private const string TAG = "ColorStateList";

        private static readonly SKColor DEFAULT_COLOR = SKColors.Red;
        private static readonly int[][] EMPTY = new int[][] { Array.Empty<int>() };

        /** Thread-safe cache of single-color ColorStateLists. */
        private static readonly SparseArray<WeakReference<ColorStateList>> sCache = new();

        /** Lazily-created factory for this color state list. */
        private ColorStateListFactory mFactory;

        private int[][] mThemeAttrs;
        private int mChangingConfigurations;

        private int[][] mStateSpecs;
        private int[] mColors;
        private int mDefaultColor;
        private bool mIsOpaque;

        private ColorStateList()
        {
            // Not publicly instantiable.
        }

        /**
         * Creates a ColorStateList that returns the specified mapping from
         * states to colors.
         */
        public ColorStateList(int[][] states, int[] colors)
        {
            mStateSpecs = states;
            mColors = colors;

            onColorsChanged();
        }

        /**
         * @return A ColorStateList containing a single color.
         */
        public static ColorStateList valueOf(int color)
        {
            lock (sCache)
            {
                int index = sCache.indexOfKey(color);
                if (index >= 0)
                {
                    ColorStateList cached;
                    sCache.valueAt(index).TryGetTarget(out cached);
                    if (cached != null)
                    {
                        return cached;
                    }

                    // Prune missing entry.
                    sCache.removeAt(index);
                }

                // Prune the cache before adding new items.
                int N = sCache.size();
                for (int i = N - 1; i >= 0; i--)
                {
                    ColorStateList cached;
                    sCache.valueAt(index).TryGetTarget(out cached);
                    if (cached == null)
                    {
                        sCache.removeAt(i);
                    }
                }

                ColorStateList csl = new(EMPTY, new int[] { color });
                sCache.put(color, new(csl));
                return csl;
            }
        }

        /**
         * Creates a ColorStateList with the same properties as another
         * ColorStateList.
         * <p>
         * The properties of the new ColorStateList can be modified without
         * affecting the source ColorStateList.
         *
         * @param orig the source color state list
         */
        private ColorStateList(ColorStateList orig)
        {
            if (orig != null)
            {
                mChangingConfigurations = orig.mChangingConfigurations;
                mStateSpecs = orig.mStateSpecs;
                mDefaultColor = orig.mDefaultColor;
                mIsOpaque = orig.mIsOpaque;

                // Deep copy, these may change due to applyTheme().
                mThemeAttrs = (int[][])orig.mThemeAttrs.Clone();
                mColors = (int[])orig.mColors.Clone();
            }
        }

        /**
            * Creates a new ColorStateList that has the same states and colors as this
            * one but where each color has the specified alpha value (0-255).
            *
            * @param alpha The new alpha channel value (0-255).
            * @return A new color state list.
            */
        public ColorStateList withAlpha(int alpha)
        {
            int[] colors = new int[mColors.Length];
            int len = colors.Length;
            for (int i = 0; i < len; i++)
            {
                colors[i] = mColors[i] & 0xFFFFFF | alpha << 24;
            }

            return new ColorStateList(mStateSpecs, colors);
        }

        /**
         * Creates a new ColorStateList that has the same states and colors as this
         * one but where each color has the specified perceived luminosity value (0-100).
         *
         * @param lStar Target perceptual luminance (0-100).
         * @return A new color state list.
         */
        public ColorStateList withLStar(float lStar)
        {
            int[] colors = new int[mColors.Length];
            int len = colors.Length;
            for (int i = 0; i < len; i++)
            {
                colors[i] = modulateColor(mColors[i], 1.0f /* alphaMod */, lStar);
            }

            return new ColorStateList(mStateSpecs, colors);
        }

        /**
         * Returns whether a theme can be applied to this color state list, which
         * usually indicates that the color state list has unresolved theme
         * attributes.
         *
         * @return whether a theme can be applied to this color state list
         * @hide only for resource preloading
         */
        public override bool canApplyTheme()
        {
            return mThemeAttrs != null;
        }

        /**
         * Applies a theme to this color state list.
         * <p>
         * <strong>Note:</strong> Applying a theme may affect the changing
         * configuration parameters of this color state list. After calling this
         * method, any dependent configurations must be updated by obtaining the
         * new configuration mask from {@link #getChangingConfigurations()}.
         *
         * @param t the theme to apply
         */
        private void applyTheme(Theme t)
        {
            //if (mThemeAttrs == null)
            //{
            return;
            //}

            //bool hasUnresolvedAttrs = false;

            //int[][] themeAttrsList = mThemeAttrs;
            //int N = themeAttrsList.Length;
            //for (int i = 0; i < N; i++)
            //{
            //    if (themeAttrsList[i] != null)
            //    {
            //        TypedArray a = t.resolveAttributes(themeAttrsList[i],
            //                R.styleable.ColorStateListItem);

            //        float defaultAlphaMod;
            //        if (themeAttrsList[i][R.styleable.ColorStateListItem_color] != 0)
            //        {
            //            // If the base color hasn't been resolved yet, the current
            //            // color's alpha channel is either full-opacity (if we
            //            // haven't resolved the alpha modulation yet) or
            //            // pre-modulated. Either is okay as a default value.
            //            defaultAlphaMod = Color.alpha(mColors[i]) / 255.0f;
            //        }
            //        else
            //        {
            //            // Otherwise, the only correct default value is 1. Even if
            //            // nothing is resolved during this call, we can apply this
            //            // multiple times without losing of information.
            //            defaultAlphaMod = 1.0f;
            //        }

            //        // Extract the theme attributes, if any, before attempting to
            //        // read from the typed array. This prevents a crash if we have
            //        // unresolved attrs.
            //        themeAttrsList[i] = a.extractThemeAttrs(themeAttrsList[i]);
            //        if (themeAttrsList[i] != null)
            //        {
            //            hasUnresolvedAttrs = true;
            //        }

            //        int baseColor = a.getColor(
            //                R.styleable.ColorStateListItem_color, mColors[i]);
            //        float alphaMod = a.getFloat(
            //                R.styleable.ColorStateListItem_alpha, defaultAlphaMod);
            //        float lStar = a.getFloat(
            //                R.styleable.ColorStateListItem_lStar, -1.0f);
            //        mColors[i] = modulateColor(baseColor, alphaMod, lStar);

            //        // Account for any configuration changes.
            //        mChangingConfigurations |= a.getChangingConfigurations();

            //        a.recycle();
            //    }
            //}

            //if (!hasUnresolvedAttrs)
            //{
            //    mThemeAttrs = null;
            //}

            //onColorsChanged();
        }

        /**
         * Returns an appropriately themed color state list.
         *
         * @param t the theme to apply
         * @return a copy of the color state list with the theme applied, or the
         *         color state list itself if there were no unresolved theme
         *         attributes
         * @hide only for resource preloading
         */
        public override ComplexColor obtainForTheme(Theme t)
        {
            if (t == null || !canApplyTheme())
            {
                return this;
            }

            ColorStateList clone = new(this);
            clone.applyTheme(t);
            return clone;
        }

        /**
         * Returns a mask of the configuration parameters for which this color
         * state list may change, requiring that it be re-created.
         *
         * @return a mask of the changing configuration parameters, as defined by
         *         {@link android.content.pm.ActivityInfo}
         *
         * @see android.content.pm.ActivityInfo
         */
        override public int getChangingConfigurations()
        {
            return base.getChangingConfigurations() | mChangingConfigurations;
        }

        private int modulateColor(int baseColor, float alphaMod, float lStar)
        {
            bool validLStar = lStar >= 0.0f && lStar <= 100.0f;
            if (alphaMod == 1.0f && !validLStar)
            {
                return baseColor;
            }

            int baseAlpha = Color.alpha(baseColor);
            int alpha = MathUtils.constrain((int)(baseAlpha * alphaMod + 0.5f), 0, 255);

            if (validLStar)
            {
                Cam baseCam = ColorUtils.colorToCAM(baseColor);
                baseColor = ColorUtils.CAMToColor(baseCam.getHue(), baseCam.getChroma(), lStar);
            }

            return baseColor & 0xFFFFFF | alpha << 24;
        }

        /**
         * Indicates whether this color state list contains at least one state spec
         * and the first spec is not empty (e.g. match-all).
         *
         * @return True if this color state list changes color based on state, false
         *         otherwise.
         * @see #getColorForState(int[], int)
         */
        override public bool isStateful()
        {
            return mStateSpecs.Length >= 1 && mStateSpecs[0].Length > 0;
        }

        /**
         * Return whether the state spec list has at least one item explicitly specifying
         * {@link android.R.attr#state_focused}.
         * @hide
         */
        public bool hasFocusStateSpecified()
        {
            return StateSet.containsAttribute(mStateSpecs, 0); // R.attr.state_focused);
        }

        /**
         * Indicates whether this color state list is opaque, which means that every
         * color returned from {@link #getColorForState(int[], int)} has an alpha
         * value of 255.
         *
         * @return True if this color state list is opaque.
         */
        public bool isOpaque()
        {
            return mIsOpaque;
        }

        /**
         * Return the color associated with the given set of
         * {@link android.view.View} states.
         *
         * @param stateSet an array of {@link android.view.View} states
         * @param defaultColor the color to return if there's no matching state
         *                     spec in this {@link ColorStateList} that matches the
         *                     stateSet.
         *
         * @return the color associated with that set of states in this {@link ColorStateList}.
         */
        public int getColorForState(int[] stateSet, int defaultColor)
        {
            int setLength = mStateSpecs.Length;
            for (int i = 0; i < setLength; i++)
            {
                int[] stateSpec = mStateSpecs[i];
                if (StateSet.stateSetMatches(stateSpec, stateSet))
                {
                    return mColors[i];
                }
            }
            return defaultColor;
        }

        /**
         * Return the default color in this {@link ColorStateList}.
         *
         * @return the default color in this {@link ColorStateList}.
         */
        public override int getDefaultColor()
        {
            return mDefaultColor;
        }

        /**
         * Return the states in this {@link ColorStateList}. The returned array
         * should not be modified.
         *
         * @return the states in this {@link ColorStateList}
         * @hide
         */
        public int[][] getStates()
        {
            return mStateSpecs;
        }

        /**
         * Return the colors in this {@link ColorStateList}. The returned array
         * should not be modified.
         *
         * @return the colors in this {@link ColorStateList}
         * @hide
         */
        public int[] getColors()
        {
            return mColors;
        }

        /**
         * Returns whether the specified state is referenced in any of the state
         * specs contained within this ColorStateList.
         * <p>
         * Any reference, either positive or negative {ex. ~R.attr.state_enabled},
         * will cause this method to return {@code true}. Wildcards are not counted
         * as references.
         *
         * @param state the state to search for
         * @return {@code true} if the state if referenced, {@code false} otherwise
         * @hide Use only as directed. For internal use only.
         */
        public bool hasState(int state)
        {
            int[][] stateSpecs = mStateSpecs;
            int specCount = stateSpecs.Length;
            for (int specIndex = 0; specIndex < specCount; specIndex++)
            {
                int[] states = stateSpecs[specIndex];
                int stateCount = states.Length;
                for (int stateIndex = 0; stateIndex < stateCount; stateIndex++)
                {
                    if (states[stateIndex] == state || states[stateIndex] == ~state)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        override public string ToString()
        {
            return "ColorStateList{" +
                   "mThemeAttrs=" + Arrays.deepToString(mThemeAttrs) +
                   "mChangingConfigurations=" + mChangingConfigurations +
                   "mStateSpecs=" + Arrays.deepToString(mStateSpecs) +
                   "mColors=" + Arrays.toString(mColors) +
                   "mDefaultColor=" + mDefaultColor + '}';
        }

        /**
         * Updates the default color and opacity.
         */
        private void onColorsChanged()
        {
            int defaultColor = (int)(uint)DEFAULT_COLOR;
            bool isOpaque = true;

            int[][] states = mStateSpecs;
            int[] colors = mColors;
            int N = states.Length;
            if (N > 0)
            {
                defaultColor = colors[0];

                for (int i = N - 1; i > 0; i--)
                {
                    if (states[i].Length == 0)
                    {
                        defaultColor = colors[i];
                        break;
                    }
                }

                for (int i = 0; i < N; i++)
                {
                    if (Color.alpha(colors[i]) != 0xFF)
                    {
                        isOpaque = false;
                        break;
                    }
                }
            }

            mDefaultColor = defaultColor;
            mIsOpaque = isOpaque;
        }

        /**
         * @return a factory that can create new instances of this ColorStateList
         * @hide only for resource preloading
         */
        public override ConstantState<ComplexColor> getConstantState()
        {
            if (mFactory == null)
            {
                mFactory = new ColorStateListFactory(this);
            }
            return mFactory;
        }

        private class ColorStateListFactory : ConstantState<ComplexColor>
        {
            private ColorStateList mSrc;

            public ColorStateListFactory(ColorStateList src)
            {
                mSrc = src;
            }


            override public int getChangingConfigurations()
            {
                return mSrc.mChangingConfigurations;
            }

            override public ComplexColor newInstance()
            {
                return mSrc;
            }

            override public ComplexColor newInstance(Theme theme)
            {
                return mSrc.obtainForTheme(theme);
            }
        }
    }
}