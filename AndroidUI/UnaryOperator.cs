/*
 * Copyright (C) 2016 The Android Open Source Project
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

namespace AndroidUI
{
    public class UnaryOperator
    {
        public UnaryOperator()
        {
        }
    }

    /**
     * Represents an operation on a single typed-valued operand that produces
     * a typed-valued result.  This is the primitive type specialization of
     * {@link UnaryOperator} for {@code double}.
     *
     * <p>This is a <a href="package-summary.html">functional interface</a>
     * whose functional method is {@link #applyAsDouble(double)}.
     *
     * @see UnaryOperator
     * @since 1.8
     */
    public class UnaryOperator<T> : UnaryOperator
    {
        readonly Func<T, T> a;
        readonly System.Reflection.ConstructorInfo constructorInfo;

        public static UnaryOperator identity()
        {
            return new UnaryOperator<T>(v => v);
        }

        public static R identity<R>() where R : UnaryOperator<T>
        {
            return (R) Activator.CreateInstance(typeof(R), new Func<T, T>(v => v));
        }

        public static implicit operator UnaryOperator<T>(Func<T, T> func)
        {
            return new UnaryOperator<T>(func);
        }

        public UnaryOperator(Func<T, T> a)
        {
            this.a = a ?? throw new ArgumentNullException(nameof(a));
            this.constructorInfo = GetType().GetConstructor(new Type[] { typeof(Func<T, T>) });
        }

        /**
         * Applies this operator to the given operand.
         *
         * @param operand the operand
         * @return the operator result
         */
        public virtual T apply(T operand)
        {
            return a.Invoke(operand);
        }

        /**
         * Returns a composed operator that first applies the {@code before}
         * operator to its input, and then applies this operator to the result.
         * If evaluation of either operator throws an exception, it is relayed to
         * the caller of the composed operator.
         *
         * @param before the operator to apply before this operator is applied
         * @return a composed operator that first applies the {@code before}
         * operator and then applies this operator
         * @throws NullPointerException if before is null
         *
         * @see #andThen(DoubleUnaryOperator)
         */
        public UnaryOperator<T> compose(UnaryOperator<T> before)
        {
            if (before == null) throw new ArgumentNullException(nameof(before));
            return (UnaryOperator<T>) constructorInfo.Invoke(new object[] { new Func<T, T>(v => apply(before.apply(v))) });
        }

        /**
         * Returns a composed operator that first applies this operator to
         * its input, and then applies the {@code after} operator to the result.
         * If evaluation of either operator throws an exception, it is relayed to
         * the caller of the composed operator.
         *
         * @param after the operator to apply after this operator is applied
         * @return a composed operator that first applies this operator and then
         * applies the {@code after} operator
         * @throws NullPointerException if after is null
         *
         * @see #compose(DoubleUnaryOperator)
         */
        public UnaryOperator<T> andThen(UnaryOperator<T> after)
        {
            if (after == null) throw new ArgumentNullException(nameof(after));
            return (UnaryOperator<T>)constructorInfo.Invoke(new object[] { new Func<T, T>(v => after.apply(apply(v))) });
        }
    }
}