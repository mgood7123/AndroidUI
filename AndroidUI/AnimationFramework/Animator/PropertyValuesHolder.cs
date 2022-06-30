/*
 * Copyright (C) 2010 The Android Open Source Project
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

using AndroidUI.Exceptions;
using System.Reflection;

namespace AndroidUI.AnimationFramework.Animator
{
    /**
     * This class holds information about a property and the values that that property
     * should take on during an animation. PropertyValuesHolder objects can be used to create
     * animations with ValueAnimator or ObjectAnimator that operate on several different properties
     * in parallel.
     */
    public class PropertyValuesHolder : ICloneable
    {

        /**
         * The name of the property associated with the values. This need not be a real property,
         * unless this object is being used with ObjectAnimator. But this is the name by which
         * aniamted values are looked up with getAnimatedValue(String) in ValueAnimator.
         */
        string mPropertyName;
        string mPropertyName2;
        string mPropertyName4;
        string mPropertyNameA;

        /**
         * @hide
         */
        protected IProperty mProperty;

        /**
         * The setter function, if needed. ObjectAnimator hands off this functionality to
         * PropertyValuesHolder, since it holds all of the per-property information. This
         * property is automatically
         * derived when the animation starts in setupSetterAndGetter() if using ObjectAnimator.
         */
        MethodInfo mSetter = null;

        /**
         * The setter function, if needed. ObjectAnimator hands off this functionality to
         * PropertyValuesHolder, since it holds all of the per-property information. This
         * property is automatically
         * derived when the animation starts in setupSetterAndGetter() if using ObjectAnimator.
         */
        MethodInfo mSetter2 = null;

        /**
         * The setter function, if needed. ObjectAnimator hands off this functionality to
         * PropertyValuesHolder, since it holds all of the per-property information. This
         * property is automatically
         * derived when the animation starts in setupSetterAndGetter() if using ObjectAnimator.
         */
        MethodInfo mSetter4 = null;

        /**
         * The setter function, if needed. ObjectAnimator hands off this functionality to
         * PropertyValuesHolder, since it holds all of the per-property information. This
         * property is automatically
         * derived when the animation starts in setupSetterAndGetter() if using ObjectAnimator.
         */
        MethodInfo mSetterA = null;

        /**
         * The getter function, if needed. ObjectAnimator hands off this functionality to
         * PropertyValuesHolder, since it holds all of the per-property information. This
         * property is automatically
         * derived when the animation starts in setupSetterAndGetter() if using ObjectAnimator.
         * The getter is only derived and used if one of the values is null.
         */
        private MethodInfo mGetter = null;

        /**
         * The type of values supplied. This information is used both in deriving the setter/getter
         * functions and in deriving the type of TypeEvaluator.
         */
        Type mValueType;

        /**
         * The set of keyframes (time/value pairs) that define this animation.
         */
        internal Keyframes mKeyframes = null;


        // type evaluators for the primitive types handled by this implementation
        private readonly ITypeEvaluator sIntEvaluator = new IntEvaluator();
        private readonly ITypeEvaluator sFloatEvaluator = new FloatEvaluator();

        // We try several different types when searching for appropriate setter/getter functions.
        // The caller may have supplied values in a type that does not match the setter/getter
        // functions (such as the ints 0 and 1 to represent floating point values for alpha).
        // Also, the use of generics in constructors means that we end up with the Object versions
        // of primitive types (Float vs. float). But most likely, the setter/getter functions
        // will take primitive types instead.
        // So we supply an ordered array of other types to try before giving up.
        private static Type[] FLOAT_VARIANTS = { typeof(float), typeof(double), typeof(int) };
        private static Type[] int_VARIANTS = { typeof(int), typeof(float), typeof(double) };
        private static Type[] DOUBLE_VARIANTS = { typeof(double), typeof(float), typeof(int) };

        // These maps hold all property entries for a particular class. This map
        // is used to speed up property/setter/getter lookups for a given class/property
        // combination. No need to use reflection on the combination more than once.
        private readonly Dictionary<Type, Dictionary<string, MethodInfo>> sSetterPropertyMap =
                new();
        private readonly Dictionary<Type, Dictionary<string, MethodInfo>> sGetterPropertyMap =
                new();

        // Used to pass single value to varargs parameter in setter invocation
        readonly object[] mTmpValueArray = new object[1];

        /**
         * The type evaluator used to calculate the animated values. This evaluator is determined
         * automatically based on the type of the start/end objects passed into the constructor,
         * but the system only knows about the primitive types int and float. Any other
         * type will need to set the evaluator to a custom evaluator for that type.
         */
        private ITypeEvaluator mEvaluator;

        /**
         * The value most recently calculated by calculateValue(). This is set during
         * that function and might be retrieved later either by ValueAnimator.animatedValue() or
         * by the property-setting logic in ObjectAnimator.animatedValue().
         */
        private object mAnimatedValue;

        /**
         * Converts from the source Object type to the setter Object type.
         */
        private ITypeConverter mConverter;

        /**
         * Internal utility constructor, used by the factory methods to set the property name.
         * @param propertyName The name of the property for this holder.
         */
        private PropertyValuesHolder(string propertyName)
        {
            mPropertyName = propertyName;
            if (mPropertyName != null)
            {
                mPropertyName2 = mPropertyName + "2";
                mPropertyName2 = mPropertyName + "4";
                mPropertyName2 = mPropertyName + "[]";
            }
        }

        /**
         * Internal utility constructor, used by the factory methods to set the property.
         * @param property The property for this holder.
         */
        private PropertyValuesHolder(IProperty property)
        {
            mProperty = property;
            if (property != null)
            {
                mPropertyName = property.getName();
                if (mPropertyName != null)
                {
                    mPropertyName2 = mPropertyName + "2";
                    mPropertyName2 = mPropertyName + "4";
                    mPropertyName2 = mPropertyName + "[]";
                }
            }
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property name and
         * set of int values.
         * @param propertyName The name of the property being animated.
         * @param values The values that the named property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         */
        public static PropertyValuesHolder ofInt(string propertyName, params int[] values)
        {
            return new IntPropertyValuesHolder(propertyName, values);
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property and
         * set of int values.
         * @param property The property being animated. Should not be null.
         * @param values The values that the property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         */
        public static PropertyValuesHolder ofInt(IProperty property, params int[] values)
        {
            return new IntPropertyValuesHolder(property, values);
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property name and
         * set of <code>int[]</code> values. At least two <code>int[]</code> values must be supplied,
         * a start and end value. If more values are supplied, the values will be animated from the
         * start, through all intermediate values to the end value. When used with ObjectAnimator,
         * the elements of the array represent the parameters of the setter function.
         *
         * @param propertyName The name of the property being animated. Can also be the
         *                     case-sensitive name of the entire setter method. Should not be null.
         * @param values The values that the property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         * @see IntArrayEvaluator#IntArrayEvaluator(int[])
         * @see ObjectAnimator#ofMultiInt(Object, String, TypeConverter, TypeEvaluator, Object[])
         */
        public static PropertyValuesHolder ofMultiInt(string propertyName, int[][] values)
        {
            if (values.Length < 2)
            {
                throw new IllegalArgumentException("At least 2 values must be supplied");
            }
            int numParameters = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == null)
                {
                    throw new IllegalArgumentException("values must not be null");
                }
                int length = values[i].Length;
                if (i == 0)
                {
                    numParameters = length;
                }
                else if (length != numParameters)
                {
                    throw new IllegalArgumentException("Values must all have the same length");
                }
            }
            IntArrayEvaluator evaluator = new IntArrayEvaluator(new int[numParameters]);
            return new MultiIntValuesHolder(propertyName, null, evaluator, (object[])values);
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property name to use
         * as a multi-int setter. The values are animated along the path, with the first
         * parameter of the setter set to the x coordinate and the second set to the y coordinate.
         *
         * @param propertyName The name of the property being animated. Can also be the
         *                     case-sensitive name of the entire setter method. Should not be null.
         *                     The setter must take exactly two <code>int</code> parameters.
         * @param path The Path along which the values should be animated.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         * @see ObjectAnimator#ofPropertyValuesHolder(Object, PropertyValuesHolder...)
         */
        public static PropertyValuesHolder ofMultiInt(string propertyName, Path path)
        {
            Keyframes keyframes = KeyframeSet.ofPath(path);
            SKPointToIntArray converter = new();
            return new MultiIntValuesHolder(propertyName, converter, null, keyframes);
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property and
         * set of Object values for use with ObjectAnimator multi-value setters. The Object
         * values are converted to <code>int[]</code> using the converter.
         *
         * @param propertyName The property being animated or complete name of the setter.
         *                     Should not be null.
         * @param converter Used to convert the animated value to setter parameters.
         * @param evaluator A TypeEvaluator that will be called on each animation frame to
         * provide the necessary interpolation between the Object values to derive the animated
         * value.
         * @param values The values that the property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         * @see ObjectAnimator#ofMultiInt(Object, String, TypeConverter, TypeEvaluator, Object[])
         * @see ObjectAnimator#ofPropertyValuesHolder(Object, PropertyValuesHolder...)
         */
        public static PropertyValuesHolder ofMultiInt<V>(string propertyName,
                TypeConverter<V, int[]> converter, TypeEvaluator<V> evaluator, params V[] values)
        {
            return new MultiIntValuesHolder(propertyName, converter, evaluator, values);
        }

        /**
         * Constructs and returns a PropertyValuesHolder object with the specified property name or
         * setter name for use in a multi-int setter function using ObjectAnimator. The values can be
         * of any type, but the type should be consistent so that the supplied
         * {@link android.animation.TypeEvaluator} can be used to to evaluate the animated value. The
         * <code>converter</code> converts the values to parameters in the setter function.
         *
         * <p>At least two values must be supplied, a start and an end value.</p>
         *
         * @param propertyName The name of the property to associate with the set of values. This
         *                     may also be the complete name of a setter function.
         * @param converter    Converts <code>values</code> into int parameters for the setter.
         *                     Can be null if the Keyframes have int[] values.
         * @param evaluator    Used to interpolate between values.
         * @param values       The values at specific fractional times to evaluate between
         * @return A PropertyValuesHolder for a multi-int parameter setter.
         */
        public static PropertyValuesHolder ofMultiInt<T>(string propertyName,
                TypeConverter<T, int[]> converter, TypeEvaluator<T> evaluator, params Keyframe[] values)
        {
            KeyframeSet keyframeSet = KeyframeSet.ofKeyframe(values);
            return new MultiIntValuesHolder(propertyName, converter, evaluator, keyframeSet);
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property name and
         * set of float values.
         * @param propertyName The name of the property being animated.
         * @param values The values that the named property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         */
        public static PropertyValuesHolder ofFloat(string propertyName, params float[] values)
        {
            return new FloatPropertyValuesHolder(propertyName, values);
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property and
         * set of float values.
         * @param property The property being animated. Should not be null.
         * @param values The values that the property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         */
        public static PropertyValuesHolder ofFloat(IProperty property, params float[] values)
        {
            return new FloatPropertyValuesHolder(property, values);
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property name and
         * set of <code>float[]</code> values. At least two <code>float[]</code> values must be supplied,
         * a start and end value. If more values are supplied, the values will be animated from the
         * start, through all intermediate values to the end value. When used with ObjectAnimator,
         * the elements of the array represent the parameters of the setter function.
         *
         * @param propertyName The name of the property being animated. Can also be the
         *                     case-sensitive name of the entire setter method. Should not be null.
         * @param values The values that the property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         * @see FloatArrayEvaluator#FloatArrayEvaluator(float[])
         * @see ObjectAnimator#ofMultiFloat(Object, String, TypeConverter, TypeEvaluator, Object[])
         */
        public static PropertyValuesHolder ofMultiFloat(string propertyName, float[][] values)
        {
            if (values.Length < 2)
            {
                throw new IllegalArgumentException("At least 2 values must be supplied");
            }
            int numParameters = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == null)
                {
                    throw new IllegalArgumentException("values must not be null");
                }
                int length = values[i].Length;
                if (i == 0)
                {
                    numParameters = length;
                }
                else if (length != numParameters)
                {
                    throw new IllegalArgumentException("Values must all have the same length");
                }
            }
            FloatArrayEvaluator evaluator = new FloatArrayEvaluator(new float[numParameters]);
            return new MultiFloatValuesHolder(propertyName, null, evaluator, (object[])values);
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property name to use
         * as a multi-float setter. The values are animated along the path, with the first
         * parameter of the setter set to the x coordinate and the second set to the y coordinate.
         *
         * @param propertyName The name of the property being animated. Can also be the
         *                     case-sensitive name of the entire setter method. Should not be null.
         *                     The setter must take exactly two <code>float</code> parameters.
         * @param path The Path along which the values should be animated.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         * @see ObjectAnimator#ofPropertyValuesHolder(Object, PropertyValuesHolder...)
         */
        public static PropertyValuesHolder ofMultiFloat(string propertyName, Path path)
        {
            Keyframes keyframes = KeyframeSet.ofPath(path);
            SKPointToFloatArray converter = new();
            return new MultiFloatValuesHolder(propertyName, converter, null, keyframes);
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property and
         * set of Object values for use with ObjectAnimator multi-value setters. The Object
         * values are converted to <code>float[]</code> using the converter.
         *
         * @param propertyName The property being animated or complete name of the setter.
         *                     Should not be null.
         * @param converter Used to convert the animated value to setter parameters.
         * @param evaluator A TypeEvaluator that will be called on each animation frame to
         * provide the necessary interpolation between the Object values to derive the animated
         * value.
         * @param values The values that the property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         * @see ObjectAnimator#ofMultiFloat(Object, String, TypeConverter, TypeEvaluator, Object[])
         */
        public static PropertyValuesHolder ofMultiFloat<V>(string propertyName,
                TypeConverter<V, float[]> converter, TypeEvaluator<V> evaluator, params V[] values)
        {
            return new MultiFloatValuesHolder(propertyName, converter, evaluator, values);
        }

        /**
         * Constructs and returns a PropertyValuesHolder object with the specified property name or
         * setter name for use in a multi-float setter function using ObjectAnimator. The values can be
         * of any type, but the type should be consistent so that the supplied
         * {@link android.animation.TypeEvaluator} can be used to to evaluate the animated value. The
         * <code>converter</code> converts the values to parameters in the setter function.
         *
         * <p>At least two values must be supplied, a start and an end value.</p>
         *
         * @param propertyName The name of the property to associate with the set of values. This
         *                     may also be the complete name of a setter function.
         * @param converter    Converts <code>values</code> into float parameters for the setter.
         *                     Can be null if the Keyframes have float[] values.
         * @param evaluator    Used to interpolate between values.
         * @param values       The values at specific fractional times to evaluate between
         * @return A PropertyValuesHolder for a multi-float parameter setter.
         */
        public static PropertyValuesHolder ofMultiFloat<T>(string propertyName,
                TypeConverter<T, float[]> converter, TypeEvaluator<T> evaluator, params Keyframe[] values)
        {
            KeyframeSet keyframeSet = KeyframeSet.ofKeyframe(values);
            return new MultiFloatValuesHolder(propertyName, converter, evaluator, keyframeSet);
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property name and
         * set of Object values. This variant also takes a TypeEvaluator because the system
         * cannot automatically interpolate between objects of unknown type.
         *
         * <p><strong>Note:</strong> The Object values are stored as references to the original
         * objects, which means that changes to those objects after this method is called will
         * affect the values on the PropertyValuesHolder. If the objects will be mutated externally
         * after this method is called, callers should pass a copy of those objects instead.
         *
         * @param propertyName The name of the property being animated.
         * @param evaluator A TypeEvaluator that will be called on each animation frame to
         * provide the necessary interpolation between the Object values to derive the animated
         * value.
         * @param values The values that the named property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         */
        public static PropertyValuesHolder ofObject(string propertyName, ITypeEvaluator evaluator,
                params object[] values)
        {
            PropertyValuesHolder pvh = new(propertyName);
            pvh.setObjectValues(values);
            pvh.setEvaluator(evaluator);
            return pvh;
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property name and
         * a Path along which the values should be animated. This variant supports a
         * <code>TypeConverter</code> to convert from <code>SkiaSharp.SKPoint</code> to the target
         * type.
         *
         * <p>The SkiaSharp.SKPoint passed to <code>converter</code> or <code>property</code>, if
         * <code>converter</code> is <code>null</code>, is reused on each animation frame and should
         * not be stored by the setter or TypeConverter.</p>
         *
         * @param propertyName The name of the property being animated.
         * @param converter Converts a SkiaSharp.SKPoint to the type associated with the setter. May be
         *                  null if conversion is unnecessary.
         * @param path The Path along which the values should be animated.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         */
        public static PropertyValuesHolder ofObject(string propertyName,
                ITypeConverter converter, Path path)
        {
            PropertyValuesHolder pvh = new(propertyName);
            pvh.mKeyframes = KeyframeSet.ofPath(path);
            pvh.mValueType = typeof(SkiaSharp.SKPoint);
            pvh.setConverter(converter);
            return pvh;
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property and
         * set of Object values. This variant also takes a TypeEvaluator because the system
         * cannot automatically interpolate between objects of unknown type.
         *
         * <p><strong>Note:</strong> The Object values are stored as references to the original
         * objects, which means that changes to those objects after this method is called will
         * affect the values on the PropertyValuesHolder. If the objects will be mutated externally
         * after this method is called, callers should pass a copy of those objects instead.
         *
         * @param property The property being animated. Should not be null.
         * @param evaluator A TypeEvaluator that will be called on each animation frame to
         * provide the necessary interpolation between the Object values to derive the animated
         * value.
         * @param values The values that the property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         */
        public static PropertyValuesHolder ofObject<V>(IProperty property,
                TypeEvaluator<V> evaluator, params V[] values)
        {
            PropertyValuesHolder pvh = new(property);
            pvh.setObjectValues(values);
            pvh.setEvaluator(evaluator);
            return pvh;
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property and
         * set of Object values. This variant also takes a TypeEvaluator because the system
         * cannot automatically interpolate between objects of unknown type. This variant also
         * takes a <code>TypeConverter</code> to convert from animated values to the type
         * of the property. If only one value is supplied, the <code>TypeConverter</code>
         * must be a {@link android.animation.BidirectionalTypeConverter} to retrieve the current
         * value.
         *
         * <p><strong>Note:</strong> The Object values are stored as references to the original
         * objects, which means that changes to those objects after this method is called will
         * affect the values on the PropertyValuesHolder. If the objects will be mutated externally
         * after this method is called, callers should pass a copy of those objects instead.
         *
         * @param property The property being animated. Should not be null.
         * @param converter Converts the animated object to the Property type.
         * @param evaluator A TypeEvaluator that will be called on each animation frame to
         * provide the necessary interpolation between the Object values to derive the animated
         * value.
         * @param values The values that the property will animate between.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         * @see #setConverter(TypeConverter)
         * @see TypeConverter
         */
        public static PropertyValuesHolder ofObject<T, V>(IProperty property,
                TypeConverter<T, V> converter, TypeEvaluator<T> evaluator, params T[] values)
        {
            PropertyValuesHolder pvh = new(property);
            pvh.setConverter(converter);
            pvh.setObjectValues(values);
            pvh.setEvaluator(evaluator);
            return pvh;
        }

        /**
         * Constructs and returns a PropertyValuesHolder with a given property and
         * a Path along which the values should be animated. This variant supports a
         * <code>TypeConverter</code> to convert from <code>SkiaSharp.SKPoint</code> to the target
         * type.
         *
         * <p>The SkiaSharp.SKPoint passed to <code>converter</code> or <code>property</code>, if
         * <code>converter</code> is <code>null</code>, is reused on each animation frame and should
         * not be stored by the setter or TypeConverter.</p>
         *
         * @param property The property being animated. Should not be null.
         * @param converter Converts a SkiaSharp.SKPoint to the type associated with the setter. May be
         *                  null if conversion is unnecessary.
         * @param path The Path along which the values should be animated.
         * @return PropertyValuesHolder The constructed PropertyValuesHolder object.
         */
        public static PropertyValuesHolder ofObject<V>(IProperty property,
                TypeConverter<SkiaSharp.SKPoint, V> converter, Path path)
        {
            PropertyValuesHolder pvh = new(property);
            pvh.mKeyframes = KeyframeSet.ofPath(path);
            pvh.mValueType = typeof(SkiaSharp.SKPoint);
            pvh.setConverter(converter);
            return pvh;
        }

        /**
         * Constructs and returns a PropertyValuesHolder object with the specified property name and set
         * of values. These values can be of any type, but the type should be consistent so that
         * an appropriate {@link android.animation.TypeEvaluator} can be found that matches
         * the common type.
         * <p>If there is only one value, it is assumed to be the end value of an animation,
         * and an initial value will be derived, if possible, by calling a getter function
         * on the object. Also, if any value is null, the value will be filled in when the animation
         * starts in the same way. This mechanism of automatically getting null values only works
         * if the PropertyValuesHolder object is used in conjunction
         * {@link ObjectAnimator}, and with a getter function
         * derived automatically from <code>propertyName</code>, since otherwise PropertyValuesHolder has
         * no way of determining what the value should be.
         * @param propertyName The name of the property associated with this set of values. This
         * can be the actual property name to be used when using a ObjectAnimator object, or
         * just a name used to get animated values, such as if this object is used with an
         * ValueAnimator object.
         * @param values The set of values to animate between.
         */
        public static PropertyValuesHolder ofKeyframe(string propertyName, params Keyframe[] values)
        {
            KeyframeSet keyframeSet = KeyframeSet.ofKeyframe(values);
            return ofKeyframes(propertyName, keyframeSet);
        }

        /**
         * Constructs and returns a PropertyValuesHolder object with the specified property and set
         * of values. These values can be of any type, but the type should be consistent so that
         * an appropriate {@link android.animation.TypeEvaluator} can be found that matches
         * the common type.
         * <p>If there is only one value, it is assumed to be the end value of an animation,
         * and an initial value will be derived, if possible, by calling the property's
         * {@link android.util.Property#get(Object)} function.
         * Also, if any value is null, the value will be filled in when the animation
         * starts in the same way. This mechanism of automatically getting null values only works
         * if the PropertyValuesHolder object is used in conjunction with
         * {@link ObjectAnimator}, since otherwise PropertyValuesHolder has
         * no way of determining what the value should be.
         * @param property The property associated with this set of values. Should not be null.
         * @param values The set of values to animate between.
         */
        public static PropertyValuesHolder ofKeyframe(IProperty property, params Keyframe[] values)
        {
            KeyframeSet keyframeSet = KeyframeSet.ofKeyframe(values);
            return ofKeyframes(property, keyframeSet);
        }

        internal static PropertyValuesHolder ofKeyframes(string propertyName, Keyframes keyframes)
        {
            if (keyframes is Keyframes.IntKeyframes)
            {
                return new IntPropertyValuesHolder(propertyName, (Keyframes.IntKeyframes)keyframes);
            }
            else if (keyframes is Keyframes.FloatKeyframes)
            {
                return new FloatPropertyValuesHolder(propertyName,
                        (Keyframes.FloatKeyframes)keyframes);
            }
            else
            {
                PropertyValuesHolder pvh = new(propertyName);
                pvh.mKeyframes = keyframes;
                pvh.mValueType = keyframes.GetType();
                return pvh;
            }
        }

        static internal PropertyValuesHolder ofKeyframes(IProperty property, Keyframes keyframes)
        {
            if (keyframes is Keyframes.IntKeyframes)
            {
                return new IntPropertyValuesHolder(property, (Keyframes.IntKeyframes)keyframes);
            }
            else if (keyframes is Keyframes.FloatKeyframes)
            {
                return new FloatPropertyValuesHolder(property, (Keyframes.FloatKeyframes)keyframes);
            }
            else
            {
                PropertyValuesHolder pvh = new(property);
                pvh.mKeyframes = keyframes;
                pvh.mValueType = keyframes.GetType();
                return pvh;
            }
        }

        /**
         * Set the animated values for this object to this set of ints.
         * If there is only one value, it is assumed to be the end value of an animation,
         * and an initial value will be derived, if possible, by calling a getter function
         * on the object. Also, if any value is null, the value will be filled in when the animation
         * starts in the same way. This mechanism of automatically getting null values only works
         * if the PropertyValuesHolder object is used in conjunction
         * {@link ObjectAnimator}, and with a getter function
         * derived automatically from <code>propertyName</code>, since otherwise PropertyValuesHolder has
         * no way of determining what the value should be.
         *
         * @param values One or more values that the animation will animate between.
         */
        virtual public void setIntValues(params int[] values)
        {
            mValueType = typeof(int);
            mKeyframes = KeyframeSet.ofInt(values);
        }

        /**
         * Set the animated values for this object to this set of floats.
         * If there is only one value, it is assumed to be the end value of an animation,
         * and an initial value will be derived, if possible, by calling a getter function
         * on the object. Also, if any value is null, the value will be filled in when the animation
         * starts in the same way. This mechanism of automatically getting null values only works
         * if the PropertyValuesHolder object is used in conjunction
         * {@link ObjectAnimator}, and with a getter function
         * derived automatically from <code>propertyName</code>, since otherwise PropertyValuesHolder has
         * no way of determining what the value should be.
         *
         * @param values One or more values that the animation will animate between.
         */
        virtual public void setFloatValues(params float[] values)
        {
            mValueType = typeof(float);
            mKeyframes = KeyframeSet.ofFloat(values);
        }

        /**
         * Set the animated values for this object to this set of Keyframes.
         *
         * @param values One or more values that the animation will animate between.
         */
        virtual public void setKeyframes(params Keyframe[] values)
        {
            int numKeyframes = values.Length;
            Keyframe[] keyframes = new Keyframe[Math.Max(numKeyframes, 2)];
            mValueType = ((Keyframe)values[0]).GetType();
            for (int i = 0; i < numKeyframes; ++i)
            {
                keyframes[i] = (Keyframe)values[i];
            }
            mKeyframes = new KeyframeSet(keyframes);
        }

        /**
         * Set the animated values for this object to this set of Objects.
         * If there is only one value, it is assumed to be the end value of an animation,
         * and an initial value will be derived, if possible, by calling a getter function
         * on the object. Also, if any value is null, the value will be filled in when the animation
         * starts in the same way. This mechanism of automatically getting null values only works
         * if the PropertyValuesHolder object is used in conjunction
         * {@link ObjectAnimator}, and with a getter function
         * derived automatically from <code>propertyName</code>, since otherwise PropertyValuesHolder has
         * no way of determining what the value should be.
         *
         * <p><strong>Note:</strong> The Object values are stored as references to the original
         * objects, which means that changes to those objects after this method is called will
         * affect the values on the PropertyValuesHolder. If the objects will be mutated externally
         * after this method is called, callers should pass a copy of those objects instead.
         *
         * @param values One or more values that the animation will animate between.
         */
        public void setObjectValues(params object[] values)
        {
            mValueType = values[0].GetType();
            mKeyframes = KeyframeSet.ofObject(values);
            if (mEvaluator != null)
            {
                mKeyframes.setEvaluator(mEvaluator);
            }
        }

        /**
         * Sets the converter to convert from the values type to the setter's parameter type.
         * If only one value is supplied, <var>converter</var> must be a
         * {@link android.animation.BidirectionalTypeConverter}.
         * @param converter The converter to use to convert values.
         */
        virtual public void setConverter(ITypeConverter converter)
        {
            mConverter = converter;
        }

        /**
         * Determine the setter or getter function using the JavaBeans convention of setFoo or
         * getFoo for a property named 'foo'. This function figures out what the name of the
         * function should be and uses reflection to find the MethodInfo with that name on the
         * target object.
         *
         * @param targetType The class to search for the method
         * @param prefix "set" or "get", depending on whether we need a setter or getter.
         * @param valueType The type of the parameter (in the case of a setter). This type
         * is derived from the values set on this PropertyValuesHolder. This type is used as
         * a first guess at the parameter type, but we check for methods with several different
         * types to avoid problems with slight mis-matches between supplied values and actual
         * value types used on the setter.
         * @return MethodInfo the method associated with mPropertyName.
         */
        private MethodInfo getPropertyFunction(Type targetType, string prefix, Type valueType)
        {
            // TODO: faster implementation...
            MethodInfo returnVal = null;
            string methodName = getMethodName(prefix, mPropertyName);
            if (valueType == null)
            {
                returnVal = targetType.GetMethod(methodName, Type.EmptyTypes);
            }
            else
            {
                Type[] typeVariants;
                if (valueType == typeof(float))
                {
                    typeVariants = FLOAT_VARIANTS;
                }
                else if (valueType == typeof(int))
                {
                    typeVariants = int_VARIANTS;
                }
                else if (valueType == typeof(double))
                {
                    typeVariants = DOUBLE_VARIANTS;
                }
                else
                {
                    typeVariants = new Type[1];
                    typeVariants[0] = valueType;
                }
                Type[] args = new Type[1];
                foreach (Type typeVariant in typeVariants)
                {
                    args[0] = typeVariant;
                    returnVal = targetType.GetMethod(methodName, args);
                    if (returnVal == null) continue;
                    if (mConverter == null)
                    {
                        // change the value type to suit
                        mValueType = typeVariant;
                    }
                    return returnVal;
                }
                // If we got here, then no appropriate function was found
            }

            if (returnVal == null)
            {
                Log.w("PropertyValuesHolder", "MethodInfo " +
                        methodName + "()" + (valueType == null ? "" : (" with type " + valueType.FullName)) +
                        " not found on target class " + targetType.FullName);
            }

            return returnVal;
        }

        /**
         * Determine the setter or getter function using the JavaBeans convention of setFoo or
         * getFoo for a property named 'foo'. This function figures out what the name of the
         * function should be and uses reflection to find the MethodInfo with that name on the
         * target object.
         *
         * @param targetType The class to search for the method
         * @param prefix "set" or "get", depending on whether we need a setter or getter.
         * @param valueType The type of the parameter (in the case of a setter). This type
         * is derived from the values set on this PropertyValuesHolder. This type is used as
         * a first guess at the parameter type, but we check for methods with several different
         * types to avoid problems with slight mis-matches between supplied values and actual
         * value types used on the setter.
         * @return MethodInfo the method associated with mPropertyName.
         */
        private MethodInfo getPropertyFunction2(Type targetType, string prefix, Type valueType)
        {
            // TODO: faster implementation...
            MethodInfo returnVal = null;
            string methodName = getMethodName(prefix, mPropertyName);
            if (valueType == null)
            {
                returnVal = targetType.GetMethod(methodName, Type.EmptyTypes);
            }
            else
            {
                Type[] args = new Type[2];
                args[0] = valueType;
                args[1] = valueType;
                returnVal = targetType.GetMethod(methodName, args);
                if (returnVal != null)
                {
                    return returnVal;
                }
                // If we got here, then no appropriate function was found
            }

            if (returnVal == null)
            {
                Log.w("PropertyValuesHolder", "MethodInfo " +
                        methodName + "()" + (valueType == null ? "" : (" with 2 argument types of " + valueType.FullName)) +
                        " not found on target class " + targetType.FullName);
            }

            return returnVal;
        }

        /**
         * Determine the setter or getter function using the JavaBeans convention of setFoo or
         * getFoo for a property named 'foo'. This function figures out what the name of the
         * function should be and uses reflection to find the MethodInfo with that name on the
         * target object.
         *
         * @param targetType The class to search for the method
         * @param prefix "set" or "get", depending on whether we need a setter or getter.
         * @param valueType The type of the parameter (in the case of a setter). This type
         * is derived from the values set on this PropertyValuesHolder. This type is used as
         * a first guess at the parameter type, but we check for methods with several different
         * types to avoid problems with slight mis-matches between supplied values and actual
         * value types used on the setter.
         * @return MethodInfo the method associated with mPropertyName.
         */
        private MethodInfo getPropertyFunction4(Type targetType, string prefix, Type valueType)
        {
            // TODO: faster implementation...
            MethodInfo returnVal = null;
            string methodName = getMethodName(prefix, mPropertyName);
            if (valueType == null)
            {
                returnVal = targetType.GetMethod(methodName, Type.EmptyTypes);
            }
            else
            {
                Type[] args = new Type[4];
                args[0] = valueType;
                args[1] = valueType;
                args[3] = valueType;
                args[4] = valueType;
                returnVal = targetType.GetMethod(methodName, args);
                if (returnVal != null)
                {
                    return returnVal;
                }
                // If we got here, then no appropriate function was found
            }

            if (returnVal == null)
            {
                Log.w("PropertyValuesHolder", "MethodInfo " +
                        methodName + "()" + (valueType == null ? "" : (" with 4 argument types of " + valueType.FullName)) +
                        " not found on target class " + targetType.FullName);
            }

            return returnVal;
        }

        /**
         * Determine the setter or getter function using the JavaBeans convention of setFoo or
         * getFoo for a property named 'foo'. This function figures out what the name of the
         * function should be and uses reflection to find the MethodInfo with that name on the
         * target object.
         *
         * @param targetType The class to search for the method
         * @param prefix "set" or "get", depending on whether we need a setter or getter.
         * @param valueType The type of the parameter (in the case of a setter). This type
         * is derived from the values set on this PropertyValuesHolder. This type is used as
         * a first guess at the parameter type, but we check for methods with several different
         * types to avoid problems with slight mis-matches between supplied values and actual
         * value types used on the setter.
         * @return MethodInfo the method associated with mPropertyName.
         */
        private MethodInfo getPropertyFunctionA(Type targetType, string prefix, Type valueType)
        {
            // TODO: faster implementation...
            MethodInfo returnVal = null;
            string methodName = getMethodName(prefix, mPropertyName);
            if (valueType == null)
            {
                returnVal = targetType.GetMethod(methodName, Type.EmptyTypes);
            }
            else
            {
                Type[] args = new Type[1];
                args[0] = valueType;
                returnVal = targetType.GetMethod(methodName, args);
                if (returnVal != null)
                {
                    return returnVal;
                }
                // If we got here, then no appropriate function was found
            }

            if (returnVal == null)
            {
                Log.w("PropertyValuesHolder", "MethodInfo " +
                        methodName + "()" + (valueType == null ? "" : (" with type " + valueType.FullName)) +
                        " not found on target class " + targetType.FullName);
            }

            return returnVal;
        }

        /**
         * Returns the setter or getter requested. This utility function checks whether the
         * requested method exists in the propertyMapMap cache. If not, it calls another
         * utility function to request the MethodInfo from the targetType directly.
         * @param targetType The Type on which the requested method should exist.
         * @param propertyMapMap The cache of setters/getters derived so far.
         * @param prefix "set" or "get", for the setter or getter.
         * @param valueType The type of parameter passed into the method (null for getter).
         * @return MethodInfo the method associated with mPropertyName.
         */
        private MethodInfo setupSetterOrGetter(Type targetType,
                Dictionary<Type, Dictionary<string, MethodInfo>> propertyMapMap,
                string prefix, Type valueType)
        {
            MethodInfo setterOrGetter = null;
            lock (propertyMapMap)
            {
                // Have to lock property map prior to reading it, to guard against
                // another thread putting something in there after we've checked it
                // but before we've added an entry to it
                Dictionary<string, MethodInfo> propertyMap = propertyMapMap.GetValueOrDefault(targetType, null);
                bool wasInMap = false;
                if (propertyMap != null)
                {
                    wasInMap = propertyMap.ContainsKey(mPropertyName);
                    if (wasInMap)
                    {
                        setterOrGetter = propertyMap[mPropertyName];
                    }
                }
                if (!wasInMap)
                {
                    setterOrGetter = getPropertyFunction(targetType, prefix, valueType);
                    if (propertyMap == null)
                    {
                        propertyMap = new Dictionary<string, MethodInfo>();
                        propertyMapMap[targetType] = propertyMap;
                    }
                    propertyMap[mPropertyName] = setterOrGetter;
                }
            }
            return setterOrGetter;
        }

        /**
         * Utility function to get the setter from targetType
         * @param targetType The Type on which the requested method should exist.
         */
        virtual protected void setupSetter(Type targetType)
        {
            Type propertyType = mConverter == null ? mValueType : mConverter.getTargetType();
            mSetter = setupSetterOrGetter(targetType, sSetterPropertyMap, "set", propertyType);
        }

        /**
         * Returns the setter or getter requested. This utility function checks whether the
         * requested method exists in the propertyMapMap cache. If not, it calls another
         * utility function to request the MethodInfo from the targetType directly.
         * @param targetType The Type on which the requested method should exist.
         * @param propertyMapMap The cache of setters/getters derived so far.
         * @param prefix "set" or "get", for the setter or getter.
         * @param valueType The type of parameter passed into the method (null for getter).
         * @return MethodInfo the method associated with mPropertyName.
         */
        private MethodInfo setupSetterOrGetter2(Type targetType,
                Dictionary<Type, Dictionary<string, MethodInfo>> propertyMapMap,
                string prefix, Type valueType)
        {
            MethodInfo setterOrGetter = null;
            lock (propertyMapMap)
            {
                // Have to lock property map prior to reading it, to guard against
                // another thread putting something in there after we've checked it
                // but before we've added an entry to it
                Dictionary<string, MethodInfo> propertyMap = propertyMapMap.GetValueOrDefault(targetType, null);
                bool wasInMap = false;
                if (propertyMap != null)
                {
                    wasInMap = propertyMap.ContainsKey(mPropertyName2);
                    if (wasInMap)
                    {
                        setterOrGetter = propertyMap[mPropertyName2];
                    }
                }
                if (!wasInMap)
                {
                    setterOrGetter = getPropertyFunction2(targetType, prefix, valueType);
                    if (propertyMap == null)
                    {
                        propertyMap = new Dictionary<string, MethodInfo>();
                        propertyMapMap[targetType] = propertyMap;
                    }
                    propertyMap[mPropertyName2] = setterOrGetter;
                }
            }
            return setterOrGetter;
        }

        /**
         * Utility function to get the setter from targetType
         * @param targetType The Type on which the requested method should exist.
         */
        protected void setupSetter2(Type targetType)
        {
            Type propertyType = mConverter == null ? mValueType : mConverter.getTargetType();
            mSetter2 = setupSetterOrGetter2(targetType, sSetterPropertyMap, "set", propertyType);
        }

        /**
         * Returns the setter or getter requested. This utility function checks whether the
         * requested method exists in the propertyMapMap cache. If not, it calls another
         * utility function to request the MethodInfo from the targetType directly.
         * @param targetType The Type on which the requested method should exist.
         * @param propertyMapMap The cache of setters/getters derived so far.
         * @param prefix "set" or "get", for the setter or getter.
         * @param valueType The type of parameter passed into the method (null for getter).
         * @return MethodInfo the method associated with mPropertyName.
         */
        private MethodInfo setupSetterOrGetter4(Type targetType,
                Dictionary<Type, Dictionary<string, MethodInfo>> propertyMapMap,
                string prefix, Type valueType)
        {
            MethodInfo setterOrGetter = null;
            lock (propertyMapMap)
            {
                // Have to lock property map prior to reading it, to guard against
                // another thread putting something in there after we've checked it
                // but before we've added an entry to it
                Dictionary<string, MethodInfo> propertyMap = propertyMapMap.GetValueOrDefault(targetType, null);
                bool wasInMap = false;
                if (propertyMap != null)
                {
                    wasInMap = propertyMap.ContainsKey(mPropertyName4);
                    if (wasInMap)
                    {
                        setterOrGetter = propertyMap[mPropertyName4];
                    }
                }
                if (!wasInMap)
                {
                    setterOrGetter = getPropertyFunction4(targetType, prefix, valueType);
                    if (propertyMap == null)
                    {
                        propertyMap = new Dictionary<string, MethodInfo>();
                        propertyMapMap[targetType] = propertyMap;
                    }
                    propertyMap[mPropertyName4] = setterOrGetter;
                }
            }
            return setterOrGetter;
        }

        /**
         * Utility function to get the setter from targetType
         * @param targetType The Type on which the requested method should exist.
         */
        protected void setupSetter4(Type targetType)
        {
            Type propertyType = mConverter == null ? mValueType : mConverter.getTargetType();
            mSetter4 = setupSetterOrGetter4(targetType, sSetterPropertyMap, "set", propertyType);
        }

        /**
         * Returns the setter or getter requested. This utility function checks whether the
         * requested method exists in the propertyMapMap cache. If not, it calls another
         * utility function to request the MethodInfo from the targetType directly.
         * @param targetType The Type on which the requested method should exist.
         * @param propertyMapMap The cache of setters/getters derived so far.
         * @param prefix "set" or "get", for the setter or getter.
         * @param valueType The type of parameter passed into the method (null for getter).
         * @return MethodInfo the method associated with mPropertyName.
         */
        private MethodInfo setupSetterOrGetterA(Type targetType,
                Dictionary<Type, Dictionary<string, MethodInfo>> propertyMapMap,
                string prefix, Type valueType)
        {
            MethodInfo setterOrGetter = null;
            lock (propertyMapMap)
            {
                // Have to lock property map prior to reading it, to guard against
                // another thread putting something in there after we've checked it
                // but before we've added an entry to it
                Dictionary<string, MethodInfo> propertyMap = propertyMapMap.GetValueOrDefault(targetType, null);
                bool wasInMap = false;
                if (propertyMap != null)
                {
                    wasInMap = propertyMap.ContainsKey(mPropertyNameA);
                    if (wasInMap)
                    {
                        setterOrGetter = propertyMap[mPropertyNameA];
                    }
                }
                if (!wasInMap)
                {
                    setterOrGetter = getPropertyFunctionA(targetType, prefix, valueType);
                    if (propertyMap == null)
                    {
                        propertyMap = new Dictionary<string, MethodInfo>();
                        propertyMapMap[targetType] = propertyMap;
                    }
                    propertyMap[mPropertyNameA] = setterOrGetter;
                }
            }
            return setterOrGetter;
        }

        /**
         * Utility function to get the setter from targetType
         * @param targetType The Type on which the requested method should exist.
         */
        protected void setupSetterA(Type targetType)
        {
            Type propertyType = mConverter == null ? mValueType : mConverter.getTargetType();
            mSetterA = setupSetterOrGetterA(targetType, sSetterPropertyMap, "set", propertyType);
        }

        /**
         * Utility function to get the getter from targetType
         */
        virtual protected void setupGetter(Type targetType)
        {
            mGetter = setupSetterOrGetter(targetType, sGetterPropertyMap, "get", null);
        }

        /**
         * Internal function (called from ObjectAnimator) to set up the setter and getter
         * prior to running the animation. If the setter has not been manually set for this
         * object, it will be derived automatically given the property name, target object, and
         * types of values supplied. If no getter has been set, it will be supplied iff any of the
         * supplied values was null. If there is a null value, then the getter (supplied or derived)
         * will be called to set those null values to the current value of the property
         * on the target object.
         * @param target The object on which the setter (and possibly getter) exist.
         */
        virtual internal void setupSetterAndGetter(object target)
        {
            if (mProperty != null)
            {
                // check to make sure that mProperty is on the class of target
                try
                {
                    object testValue = null;
                    List<Keyframe> keyframes = mKeyframes.getKeyframes();
                    int keyframeCount = keyframes == null ? 0 : keyframes.Count;
                    for (int i = 0; i < keyframeCount; i++)
                    {
                        Keyframe kf = keyframes.ElementAt(i);
                        if (!kf.hasValue() || kf.valueWasSetOnStart())
                        {
                            if (testValue == null)
                            {
                                testValue = convertBack(mProperty.get(target));
                            }
                            kf.setValue(testValue);
                            kf.setValueWasSetOnStart(true);
                        }
                    }
                    return;
                }
                catch (InvalidCastException e)
                {
                    Log.w("PropertyValuesHolder", "No such property (" + mProperty.getName() +
                            ") on target object " + target + ". Trying reflection instead");
                    mProperty = null;
                }
            }
            // We can't just say 'else' here because the catch statement sets mProperty to null.
            if (mProperty == null)
            {
                Type targetType = target.GetType();
                if (mSetter == null)
                {
                    setupSetter(targetType);
                }
                List<Keyframe> keyframes = mKeyframes.getKeyframes();
                int keyframeCount = keyframes == null ? 0 : keyframes.Count;
                for (int i = 0; i < keyframeCount; i++)
                {
                    Keyframe kf = keyframes.ElementAt(i);
                    if (!kf.hasValue() || kf.valueWasSetOnStart())
                    {
                        if (mGetter == null)
                        {
                            setupGetter(targetType);
                            if (mGetter == null)
                            {
                                // Already logged the error - just return to avoid NPE
                                return;
                            }
                        }
                        try
                        {
                            object value = convertBack(mGetter.Invoke(target, null));
                            kf.setValue(value);
                            kf.setValueWasSetOnStart(true);
                        }
                        catch (TargetInvocationException e)
                        {
                            Log.e("PropertyValuesHolder", e.ToString());
                        }
                        catch (MethodAccessException e)
                        {
                            Log.e("PropertyValuesHolder", e.ToString());
                        }
                    }
                }
            }
        }

        private object convertBack(object value)
        {
            if (mConverter != null)
            {
                if (!(mConverter is IBidirectionalTypeConverter))
                {
                    throw new IllegalArgumentException("Converter "
                            + mConverter.GetType().Name
                            + " must be a BidirectionalTypeConverter");
                }
                value = ((IBidirectionalTypeConverter)mConverter).convertBack(value);
            }
            return value;
        }

        /**
         * Utility function to set the value stored in a particular Keyframe. The value used is
         * whatever the value is for the property name specified in the keyframe on the target object.
         *
         * @param target The target object from which the current value should be extracted.
         * @param kf The keyframe which holds the property name and value.
         */
        private void setupValue(object target, Keyframe kf)
        {
            if (mProperty != null)
            {
                object value = convertBack(mProperty.get(target));
                kf.setValue(value);
            }
            else
            {
                try
                {
                    if (mGetter == null)
                    {
                        Type targetType = target.GetType();
                        setupGetter(targetType);
                        if (mGetter == null)
                        {
                            // Already logged the error - just return to avoid NPE
                            return;
                        }
                    }
                    object value = convertBack(mGetter.Invoke(target, Arrays.EMPTY_OBJECT_ARRAY));
                    kf.setValue(value);
                }
                catch (TargetInvocationException e)
                {
                    Log.e("PropertyValuesHolder", e.ToString());
                }
                catch (MethodAccessException e)
                {
                    Log.e("PropertyValuesHolder", e.ToString());
                }
            }
        }

        /**
         * This function is called by ObjectAnimator when setting the start values for an animation.
         * The start values are set according to the current values in the target object. The
         * property whose value is extracted is whatever is specified by the propertyName of this
         * PropertyValuesHolder object.
         *
         * @param target The object which holds the start values that should be set.
         */
        internal void setupStartValue(object target)
        {
            List<Keyframe> keyframes = mKeyframes.getKeyframes();
            if (keyframes.Count != 0)
            {
                setupValue(target, keyframes.ElementAt(0));
            }
        }

        /**
         * This function is called by ObjectAnimator when setting the end values for an animation.
         * The end values are set according to the current values in the target object. The
         * property whose value is extracted is whatever is specified by the propertyName of this
         * PropertyValuesHolder object.
         *
         * @param target The object which holds the start values that should be set.
         */
        internal void setupEndValue(object target)
        {
            List<Keyframe> keyframes = mKeyframes.getKeyframes();
            if (keyframes.Count != 0)
            {
                setupValue(target, keyframes.ElementAt(keyframes.Count - 1));
            }
        }

        public virtual PropertyValuesHolder Clone()
        {
            PropertyValuesHolder newPVH = (PropertyValuesHolder)ICloneable.Clone(this);
            newPVH.mPropertyName = mPropertyName;
            newPVH.mProperty = mProperty;
            newPVH.mKeyframes = (Keyframes)mKeyframes.Clone();
            newPVH.mEvaluator = mEvaluator;
            return newPVH;
        }

        /**
         * Internal function to set the value on the target object, using the setter set up
         * earlier on this PropertyValuesHolder object. This function is called by ObjectAnimator
         * to handle turning the value calculated by ValueAnimator into a value set on the object
         * according to the name of the property.
         * @param target The target object on which the value is set
         */
        virtual internal void setAnimatedValue(object target)
        {
            if (mProperty != null)
            {
                mProperty.set(target, getAnimatedValue());
            }
            if (mSetter != null)
            {
                try
                {
                    mTmpValueArray[0] = getAnimatedValue();
                    mSetter.Invoke(target, mTmpValueArray);
                }
                catch (TargetInvocationException e)
                {
                    Log.e("PropertyValuesHolder", e.ToString());
                }
                catch (MethodAccessException e)
                {
                    Log.e("PropertyValuesHolder", e.ToString());
                }
            }
        }

        protected void setAnimatedValues<T>(object target, T[] values)
        {
            int numParameters = values.Length;
            switch (numParameters)
            {
                case 1:
                    {
                        if (mSetter != null)
                        {
                            try
                            {
                                mSetter.Invoke(target, new object[1] { values[0] });
                            }
                            catch (TargetInvocationException e)
                            {
                                Log.e("PropertyValuesHolder", e.ToString());
                            }
                            catch (MethodAccessException e)
                            {
                                Log.e("PropertyValuesHolder", e.ToString());
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        if (mSetter2 != null)
                        {
                            try
                            {
                                mSetter2.Invoke(target, new object[2] { values[0], values[1] });
                            }
                            catch (TargetInvocationException e)
                            {
                                Log.e("PropertyValuesHolder", e.ToString());
                            }
                            catch (MethodAccessException e)
                            {
                                Log.e("PropertyValuesHolder", e.ToString());
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        if (mSetter4 != null)
                        {
                            try
                            {
                                mSetter4.Invoke(target, new object[4] { values[0], values[1], values[2], values[3] });
                            }
                            catch (TargetInvocationException e)
                            {
                                Log.e("PropertyValuesHolder", e.ToString());
                            }
                            catch (MethodAccessException e)
                            {
                                Log.e("PropertyValuesHolder", e.ToString());
                            }
                        }
                        break;
                    }
                default:
                    {
                        if (mSetterA != null)
                        {
                            try
                            {
                                mSetterA.Invoke(target, new object[1] { values });
                            }
                            catch (TargetInvocationException e)
                            {
                                Log.e("PropertyValuesHolder", e.ToString());
                            }
                            catch (MethodAccessException e)
                            {
                                Log.e("PropertyValuesHolder", e.ToString());
                            }
                        }
                        break;
                    }
            }
        }

        /**
         * Internal function, called by ValueAnimator, to set up the TypeEvaluator that will be used
         * to calculate animated values.
         */
        internal void init()
        {
            if (mEvaluator == null)
            {
                // We already handle int and float automatically, but not their Object
                // equivalents
                mEvaluator = (mValueType == typeof(int)) ? sIntEvaluator :
                        (mValueType == typeof(float)) ? sFloatEvaluator :
                        null;
            }
            if (mEvaluator != null)
            {
                // KeyframeSet knows how to evaluate the common types - only give it a custom
                // evaluator if one has been set on this class
                mKeyframes.setEvaluator(mEvaluator);
            }
        }

        /**
         * The TypeEvaluator will be automatically determined based on the type of values
         * supplied to PropertyValuesHolder. The evaluator can be manually set, however, if so
         * desired. This may be important in cases where either the type of the values supplied
         * do not match the way that they should be interpolated between, or if the values
         * are of a custom type or one not currently understood by the animation system. Currently,
         * only values of type float and int (and their Object equivalents: Float
         * and int) are  correctly interpolated; all other types require setting a TypeEvaluator.
         * @param evaluator
         */
        public void setEvaluator(ITypeEvaluator evaluator)
        {
            mEvaluator = evaluator;
            mKeyframes.setEvaluator(evaluator);
        }

        /**
         * Function used to calculate the value according to the evaluator set up for
         * this PropertyValuesHolder object. This function is called by ValueAnimator.animateValue().
         *
         * @param fraction The elapsed, interpolated fraction of the animation.
         */
        virtual public void calculateValue(float fraction)
        {
            object value = mKeyframes.getValue(fraction);
            mAnimatedValue = mConverter == null ? value : mConverter.convert(value);
        }

        /**
         * Sets the name of the property that will be animated. This name is used to derive
         * a setter function that will be called to set animated values.
         * For example, a property name of <code>foo</code> will result
         * in a call to the function <code>setFoo()</code> on the target object. If either
         * <code>valueFrom</code> or <code>valueTo</code> is null, then a getter function will
         * also be derived and called.
         *
         * <p>Note that the setter function derived from this property name
         * must take the same parameter type as the
         * <code>valueFrom</code> and <code>valueTo</code> properties, otherwise the call to
         * the setter function will fail.</p>
         *
         * @param propertyName The name of the property being animated.
         */
        public void setPropertyName(string propertyName)
        {
            mPropertyName = propertyName;
        }

        /**
         * Sets the property that will be animated.
         *
         * <p>Note that if this PropertyValuesHolder object is used with ObjectAnimator, the property
         * must exist on the target object specified in that ObjectAnimator.</p>
         *
         * @param property The property being animated.
         */
        virtual public void setProperty(IProperty property)
        {
            mProperty = property;
        }

        /**
         * Gets the name of the property that will be animated. This name will be used to derive
         * a setter function that will be called to set animated values.
         * For example, a property name of <code>foo</code> will result
         * in a call to the function <code>setFoo()</code> on the target object. If either
         * <code>valueFrom</code> or <code>valueTo</code> is null, then a getter function will
         * also be derived and called.
         */
        public string getPropertyName()
        {
            return mPropertyName;
        }

        /**
         * Internal function, called by ValueAnimator and ObjectAnimator, to retrieve the value
         * most recently calculated in calculateValue().
         * @return
         */
        virtual internal object getAnimatedValue()
        {
            return mAnimatedValue;
        }

        /**
         * PropertyValuesHolder is Animators use to hold internal animation related data.
         * Therefore, in order to replicate the animation behavior, we need to get data out of
         * PropertyValuesHolder.
         * @hide
         */
        internal void getPropertyValues(PropertyValues values)
        {
            init();
            values.propertyName = mPropertyName;
            values.type = mValueType;
            values.startValue = mKeyframes.getValue(0);
            if (values.startValue is PathParser.PathData)
            {
                // PathData evaluator returns the same mutable PathData object when query fraction,
                // so we have to make a copy here.
                values.startValue = new PathParser.PathData((PathParser.PathData)values.startValue);
            }
            values.endValue = mKeyframes.getValue(1);
            if (values.endValue is PathParser.PathData)
            {
                // PathData evaluator returns the same mutable PathData object when query fraction,
                // so we have to make a copy here.
                values.endValue = new PathParser.PathData((PathParser.PathData)values.endValue);
            }
            // TODO: We need a better way to get data out of keyframes.
            if (mKeyframes is PathKeyframes.FloatKeyframesBase
                    || mKeyframes is PathKeyframes.IntKeyframesBase
                    || (mKeyframes.getKeyframes() != null && mKeyframes.getKeyframes().Count > 2))
            {
                // When a pvh has more than 2 keyframes, that means there are intermediate values in
                // addition to start/end values defined for animators. Another case where such
                // intermediate values are defined is when animator has a path to animate along. In
                // these cases, a data source is needed to capture these intermediate values.
                Func<float, object> func = fraction => mKeyframes.getValue(fraction);
                values.dataSource = func;
            }
            else
            {
                values.dataSource = null;
            }
        }

        /**
         * @hide
         */
        public Type getValueType()
        {
            return mValueType;
        }

        override public string ToString()
        {
            return mPropertyName + ": " + mKeyframes.ToString();
        }

        /**
         * Utility method to derive a setter/getter method name from a property name, where the
         * prefix is typically "set" or "get" and the first letter of the property name is
         * capitalized.
         *
         * @param prefix The precursor to the method name, before the property name begins, typically
         * "set" or "get".
         * @param propertyName The name of the property that represents the bulk of the method name
         * after the prefix. The first letter of this word will be capitalized in the resulting
         * method name.
         * @return String the property name converted to a method name according to the conventions
         * specified above.
         */
        static string getMethodName(string prefix, string propertyName)
        {
            if (propertyName == null || propertyName.Length == 0)
            {
                // shouldn't get here
                return prefix;
            }
            return prefix + IProperty.CapitalizeName(propertyName);
        }

        class IntPropertyValuesHolder : PropertyValuesHolder
        {

            // C# does not have JNI approach - just use reflection
            private IIntProperty mIntProperty;

            Keyframes.IntKeyframes mIntKeyframes;
            int mIntAnimatedValue;

            public IntPropertyValuesHolder(string propertyName, Keyframes.IntKeyframes keyframes) : base(propertyName)
            {
                mValueType = typeof(int);
                mKeyframes = keyframes;
                mIntKeyframes = keyframes;
            }

            public IntPropertyValuesHolder(IProperty property, Keyframes.IntKeyframes keyframes) : base(property)
            {
                mValueType = typeof(int);
                mKeyframes = keyframes;
                mIntKeyframes = keyframes;
                if (property is IIntProperty)
                {
                    mIntProperty = (IIntProperty)mProperty;
                }
            }

            public IntPropertyValuesHolder(string propertyName, params int[] values) : base(propertyName)
            {
                setIntValues(values);
            }

            public IntPropertyValuesHolder(IProperty property, params int[] values) : base(property)
            {
                setIntValues(values);
                if (property is IIntProperty)
                {
                    mIntProperty = (IIntProperty)mProperty;
                }
            }

            override public void setProperty(IProperty property)
            {
                if (property is IIntProperty)
                {
                    mIntProperty = (IIntProperty)property;
                }
                else
                {
                    base.setProperty(property);
                }
            }

            override public void setIntValues(params int[] values)
            {
                base.setIntValues(values);
                mIntKeyframes = (Keyframes.IntKeyframes)mKeyframes;
            }

            override
            public void calculateValue(float fraction)
            {
                mIntAnimatedValue = mIntKeyframes.getIntValue(fraction);
            }

            override
            internal object getAnimatedValue()
            {
                return mIntAnimatedValue;
            }

            override
            public IntPropertyValuesHolder Clone()
            {
                IntPropertyValuesHolder newPVH = (IntPropertyValuesHolder)base.Clone();
                newPVH.mIntKeyframes = (Keyframes.IntKeyframes)newPVH.mKeyframes;
                return newPVH;
            }

            /**
             * Internal function to set the value on the target object, using the setter set up
             * earlier on this PropertyValuesHolder object. This function is called by ObjectAnimator
             * to handle turning the value calculated by ValueAnimator into a value set on the object
             * according to the name of the property.
             * @param target The target object on which the value is set
             */
            override
            internal void setAnimatedValue(object target)
            {
                if (mIntProperty != null)
                {
                    mIntProperty.setValue(target, mIntAnimatedValue);
                    return;
                }
                if (mProperty != null)
                {
                    mProperty.set(target, mIntAnimatedValue);
                    return;
                }
                if (mSetter != null)
                {
                    try
                    {
                        mTmpValueArray[0] = mIntAnimatedValue;
                        mSetter.Invoke(target, mTmpValueArray);
                    }
                    catch (TargetInvocationException e)
                    {
                        Log.e("PropertyValuesHolder", e.ToString());
                    }
                    catch (MethodAccessException e)
                    {
                        Log.e("PropertyValuesHolder", e.ToString());
                    }
                }
            }

            override
            protected void setupSetter(Type targetType)
            {
                if (mProperty != null)
                {
                    return;
                }
                base.setupSetter(targetType);
            }
        }

        class FloatPropertyValuesHolder : PropertyValuesHolder
        {

            // C# does not have JNI approach - just use reflection
            private IFloatProperty mFloatProperty;

            Keyframes.FloatKeyframes mFloatKeyframes;
            float mFloatAnimatedValue;

            public FloatPropertyValuesHolder(string propertyName, Keyframes.FloatKeyframes keyframes) : base(propertyName)
            {
                mValueType = typeof(float);
                mKeyframes = keyframes;
                mFloatKeyframes = keyframes;
            }

            public FloatPropertyValuesHolder(IProperty property, Keyframes.FloatKeyframes keyframes) : base(property)
            {
                mValueType = typeof(float);
                mKeyframes = keyframes;
                mFloatKeyframes = keyframes;
                if (property is IFloatProperty)
                {
                    mFloatProperty = (IFloatProperty)mProperty;
                }
            }

            public FloatPropertyValuesHolder(string propertyName, params float[] values) : base(propertyName)
            {
                setFloatValues(values);
            }

            public FloatPropertyValuesHolder(IProperty property, params float[] values) : base(property)
            {
                setFloatValues(values);
                if (property is IFloatProperty)
                {
                    mFloatProperty = (IFloatProperty)mProperty;
                }
            }

            override
            public void setProperty(IProperty property)
            {
                if (property is IFloatProperty)
                {
                    mFloatProperty = (IFloatProperty)property;
                }
                else
                {
                    base.setProperty(property);
                }
            }

            override
            public void setFloatValues(params float[] values)
            {
                base.setFloatValues(values);
                mFloatKeyframes = (Keyframes.FloatKeyframes)mKeyframes;
            }

            override
            public void calculateValue(float fraction)
            {
                mFloatAnimatedValue = mFloatKeyframes.getFloatValue(fraction);
            }

            override
            internal object getAnimatedValue()
            {
                return mFloatAnimatedValue;
            }

            override
            public FloatPropertyValuesHolder Clone()
            {
                FloatPropertyValuesHolder newPVH = (FloatPropertyValuesHolder)base.Clone();
                newPVH.mFloatKeyframes = (Keyframes.FloatKeyframes)newPVH.mKeyframes;
                return newPVH;
            }

            /**
             * Internal function to set the value on the target object, using the setter set up
             * earlier on this PropertyValuesHolder object. This function is called by ObjectAnimator
             * to handle turning the value calculated by ValueAnimator into a value set on the object
             * according to the name of the property.
             * @param target The target object on which the value is set
             */
            override
            internal void setAnimatedValue(object target)
            {
                if (mFloatProperty != null)
                {
                    mFloatProperty.setValue(target, mFloatAnimatedValue);
                    return;
                }
                if (mProperty != null)
                {
                    mProperty.set(target, mFloatAnimatedValue);
                    return;
                }
                if (mSetter != null)
                {
                    try
                    {
                        mTmpValueArray[0] = mFloatAnimatedValue;
                        mSetter.Invoke(target, mTmpValueArray);
                    }
                    catch (TargetInvocationException e)
                    {
                        Log.e("PropertyValuesHolder", e.ToString());
                    }
                    catch (MethodAccessException e)
                    {
                        Log.e("PropertyValuesHolder", e.ToString());
                    }
                }
            }

            override
            protected void setupSetter(Type targetType)
            {
                if (mProperty != null)
                {
                    return;
                }
                base.setupSetter(targetType);
            }

        }

        class MultiFloatValuesHolder : PropertyValuesHolder
        {
            // C# does not have JNI approach - just use reflection
            private long mJniSetter;
            private readonly Dictionary<Type, Dictionary<string, long>> sJNISetterPropertyMap =
                    new();

            public MultiFloatValuesHolder(string propertyName, ITypeConverter converter,
                    ITypeEvaluator evaluator, params object[] values) : base(propertyName)
            {
                setConverter(converter);
                setObjectValues(values);
                setEvaluator(evaluator);
            }

            public MultiFloatValuesHolder(string propertyName, ITypeConverter converter,
                    ITypeEvaluator evaluator, Keyframes keyframes) : base(propertyName)
            {
                setConverter(converter);
                mKeyframes = keyframes;
                setEvaluator(evaluator);
            }

            /**
             * Internal function to set the value on the target object, using the setter set up
             * earlier on this PropertyValuesHolder object. This function is called by ObjectAnimator
             * to handle turning the value calculated by ValueAnimator into a value set on the object
             * according to the name of the property.
             *
             * @param target The target object on which the value is set
             */
            override
            internal void setAnimatedValue(object target)
            {
                setAnimatedValues(target, (float[])getAnimatedValue());
            }

            /**
             * Internal function (called from ObjectAnimator) to set up the setter and getter
             * prior to running the animation. No getter can be used for multiple parameters.
             *
             * @param target The object on which the setter exists.
             */
            override
            internal void setupSetterAndGetter(object target)
            {
                setupSetter(target.GetType());
            }

            override
            protected void setupSetter(Type targetType)
            {
                base.setupSetter(targetType);
                setupSetter2(targetType);
                setupSetter4(targetType);
                setupSetterA(targetType);
            }
        }

        class MultiIntValuesHolder : PropertyValuesHolder
        {
            private long mJniSetter;
            private readonly Dictionary<Type, Dictionary<string, long>> sJNISetterPropertyMap =
                    new();

            public MultiIntValuesHolder(string propertyName, ITypeConverter converter,
                    ITypeEvaluator evaluator, params object[] values) : base(propertyName)
            {
                setConverter(converter);
                setObjectValues(values);
                setEvaluator(evaluator);
            }

            public MultiIntValuesHolder(string propertyName, ITypeConverter converter,
                    ITypeEvaluator evaluator, Keyframes keyframes) : base(propertyName)
            {
                setConverter(converter);
                mKeyframes = keyframes;
                setEvaluator(evaluator);
            }

            /**
             * Internal function to set the value on the target object, using the setter set up
             * earlier on this PropertyValuesHolder object. This function is called by ObjectAnimator
             * to handle turning the value calculated by ValueAnimator into a value set on the object
             * according to the name of the property.
             *
             * @param target The target object on which the value is set
             */
            override
            internal void setAnimatedValue(object target)
            {
                setAnimatedValues(target, (int[])getAnimatedValue());
            }

            /**
             * Internal function (called from ObjectAnimator) to set up the setter and getter
             * prior to running the animation. No getter can be used for multiple parameters.
             *
             * @param target The object on which the setter exists.
             */
            override
            internal void setupSetterAndGetter(object target)
            {
                setupSetter(target.GetType());
            }

            override
            protected void setupSetter(Type targetType)
            {
                base.setupSetter(targetType);
                setupSetter2(targetType);
                setupSetter4(targetType);
                setupSetterA(targetType);
            }
        }

        /**
         * Convert from SkiaSharp.SKPoint to float[] for multi-float setters along a Path.
         */
        private class SKPointToFloatArray : TypeConverter<SkiaSharp.SKPoint, float[]>
        {
            private float[] mCoordinates = new float[2];

            public SKPointToFloatArray() : base(typeof(SkiaSharp.SKPoint), typeof(float[]))
            {
            }

            override
            public float[] convert(object value)
            {
                return convert((SkiaSharp.SKPoint)value);
            }

            public float[] convert(SkiaSharp.SKPoint value)
            {
                mCoordinates[0] = value.X;
                mCoordinates[1] = value.Y;
                return mCoordinates;
            }
        };

        /**
         * Convert from SkiaSharp.SKPoint to int[] for multi-int setters along a Path.
         */
        private class SKPointToIntArray : TypeConverter<SkiaSharp.SKPoint, int[]>
        {
            private int[] mCoordinates = new int[2];

            public SKPointToIntArray() : base(typeof(SkiaSharp.SKPoint), typeof(int[]))
            {
            }

            override
            public int[] convert(object value)
            {
                return convert((SkiaSharp.SKPoint)value);
            }

            public int[] convert(SkiaSharp.SKPoint value)
            {
                mCoordinates[0] = (int)Math.Round(value.X);
                mCoordinates[1] = (int)Math.Round(value.Y);
                return mCoordinates;
            }
        };

        /**
         * @hide
         */
        internal class PropertyValues
        {
            public string propertyName;
            public Type type;
            public object startValue;
            public object endValue;
            public Func<float, object> dataSource = null;
            public string toString()
            {
                return "property name: " + propertyName + ", type: " + type + ", startValue: "
                        + startValue.ToString() + ", endValue: " + endValue.ToString();
            }
        }
    }
}