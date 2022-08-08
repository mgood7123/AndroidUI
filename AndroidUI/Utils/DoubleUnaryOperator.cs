﻿/*
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

namespace AndroidUI.Utils
{
    /**
     * Represents an operation on a single {@code double}-valued operand that produces
     * a {@code double}-valued result.  This is the primitive type specialization of
     * {@link UnaryOperator} for {@code double}.
     *
     * <p>This is a <a href="package-summary.html">functional interface</a>
     * whose functional method is {@link #applyAsDouble(double)}.
     *
     * @see UnaryOperator
     * @since 1.8
     */
    public class DoubleUnaryOperator : UnaryOperator<double>
    {
        public DoubleUnaryOperator(RunnableWithReturn<double, double> a) : base(a)
        {
        }

        /**
        * Returns a unary operator that always returns its input argument.
        *
        * @return a unary operator that always returns its input argument
        */
        public static new DoubleUnaryOperator identity()
        {
            return identity<DoubleUnaryOperator>();
        }
    }
}