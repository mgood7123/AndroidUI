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

using AndroidUI.Utils.Lists;

namespace AndroidUI.Utils.Input
{
    /*
     * Velocity tracker algorithm based on least-squares linear regression.
     */
    class LeastSquaresVelocityTrackerStrategy : VelocityTrackerStrategy
    {
        const string LOG_TAG = "LeastSquaresVelocityTrackerStrategy";
        public enum Weighting
        {
            // No weights applied.  All data points are equally reliable.
            WEIGHTING_NONE,

            // Weight by time delta.  Data points clustered together are weighted less.
            WEIGHTING_DELTA,

            // Weight such that points within a certain horizon are weighed more than those
            // outside of that horizon.
            WEIGHTING_CENTRAL,

            // Weight such that points older than a certain amount are weighed less.
            WEIGHTING_RECENT,
        };

        // Degree must be no greater than Estimator::MAX_DEGREE.
        public LeastSquaresVelocityTrackerStrategy(int degree, Weighting weighting = Weighting.WEIGHTING_NONE)
        {
            for (int i = 0; i < mMovements.Length; i++)
            {
                mMovements[i] = new();
            }

            mDegree = degree;
            mWeighting = weighting;

            clear();
        }

        public override void clear()
        {
            mIndex = 0;
            mMovements[0].idBits.Clear();
        }

        public override void clearPointers(BitwiseList<object> idBits)
        {
            BitwiseList<object> remainingIdBits = new(mMovements[mIndex].idBits & ~idBits);
            mMovements[mIndex].idBits = remainingIdBits;
        }

        public override void addMovement(
            long eventTime, BitwiseList<object> idBits, Dictionary<object, NativeVelocityTracker.Position> positions
        )
        {
            if (mMovements[mIndex].eventTime != eventTime)
            {
                // When ACTION_POINTER_DOWN happens, we will first receive ACTION_MOVE with the coordinates
                // of the existing pointers, and then ACTION_POINTER_DOWN with the coordinates that include
                // the new pointer. If the eventtimes for both events are identical, just update the data
                // for this time.
                // We only compare against the last value, as it is likely that addMovement is called
                // in chronological order as events occur.
                mIndex++;
            }
            if (mIndex == HISTORY_SIZE)
            {
                mIndex = 0;
            }

            Movement movement = mMovements[mIndex];
            movement.eventTime = eventTime;
            movement.idBits = idBits;
            int count = idBits.Count;
            for (int i = 0; i < count; i++)
            {
                object id = idBits[i];
                movement.positions[id] = positions[id];
            }
        }

        /**
         * Solves a linear least squares problem to obtain a N degree polynomial that fits
         * the specified input data as nearly as possible.
         *
         * Returns true if a solution is found, false otherwise.
         *
         * The input consists of two vectors of data points X and Y with indices 0..m-1
         * along with a weight vector W of the same size.
         *
         * The output is a vector B with indices 0..n that describes a polynomial
         * that fits the data, such the sum of W[i] * W[i] * abs(Y[i] - (B[0] + B[1] X[i]
         * + B[2] X[i]^2 ... B[n] X[i]^n)) for all i between 0 and m-1 is minimized.
         *
         * Accordingly, the weight vector W should be initialized by the caller with the
         * reciprocal square root of the variance of the error in each input data point.
         * In other words, an ideal choice for W would be W[i] = 1 / var(Y[i]) = 1 / stddev(Y[i]).
         * The weights express the relative importance of each data point.  If the weights are
         * all 1, then the data points are considered to be of equal importance when fitting
         * the polynomial.  It is a good idea to choose weights that diminish the importance
         * of data points that may have higher than usual error margins.
         *
         * Errors among data points are assumed to be independent.  W is represented here
         * as a vector although in the literature it is typically taken to be a diagonal matrix.
         *
         * That is to say, the function that generated the input data can be approximated
         * by y(x) ~= B[0] + B[1] x + B[2] x^2 + ... + B[n] x^n.
         *
         * The coefficient of determination (R^2) is also returned to describe the goodness
         * of fit of the model for the given data.  It is a value between 0 and 1, where 1
         * indicates perfect correspondence.
         *
         * This function first expands the X vector to a m by n matrix A such that
         * A[i][0] = 1, A[i][1] = X[i], A[i][2] = X[i]^2, ..., A[i][n] = X[i]^n, then
         * multiplies it by w[i]./
         *
         * Then it calculates the QR decomposition of A yielding an m by m orthonormal matrix Q
         * and an m by n upper triangular matrix R.  Because R is upper triangular (lower
         * part is all zeroes), we can simplify the decomposition into an m by n matrix
         * Q1 and a n by n matrix R1 such that A = Q1 R1.
         *
         * Finally we solve the system of linear equations given by R1 B = (Qtranspose W Y)
         * to find B.
         *
         * For efficiency, we lay out A and Q column-wise in memory because we frequently
         * operate on the column vectors.  Conversely, we lay out R row-wise.
         *
         * http://en.wikipedia.org/wiki/Numerical_methods_for_linear_least_squares
         * http://en.wikipedia.org/wiki/Gram-Schmidt
         */
        static bool solveLeastSquares(List<float> x, List<float> y, List<float> w, int n, float[] outB, out float outDet)
        {
            int m = x.Count;
            if (DEBUG_STRATEGY)
            {
                Log.d(LOG_TAG, "solveLeastSquares: m=" + m
                    + ", n=" + n
                    + ", x=" + vectorToString(x, m)
                    + ", y=" + vectorToString(y, m)
                    + ", w=" + vectorToString(w, m)
                );
            }
            if (m != y.Count || m != w.Count) throw new Exception("Mismatched vector sizes");

            // Expand the X vector to a matrix A, pre-multiplied by the weights.
            float[,] a = new float[n, m]; // column-major order
            for (int h = 0; h < m; h++)
            {
                a[0, h] = w[h];
                for (int i = 1; i < n; i++)
                {
                    a[i, h] = a[i - 1, h] * x[h];
                }
            }
            if (DEBUG_STRATEGY)
            {
                unsafe
                {
                    fixed (float* x_ = &a[0, 0])
                    {
                        Log.d(LOG_TAG, "  - a=" + matrixToString(x_, m, n, false /*rowMajor*/));
                    }
                }
            }

            // Apply the Gram-Schmidt process to A to obtain its QR decomposition.
            float[,] q = new float[n, m]; // orthonormal basis, column-major order
            float[,] r = new float[n, n]; // upper triangular matrix, row-major order
            for (int j = 0; j < n; j++)
            {
                for (int h = 0; h < m; h++)
                {
                    q[j, h] = a[j, h];
                }
                for (int i = 0; i < j; i++)
                {
                    unsafe
                    {
                        fixed (float* a_ = &q[j, 0])
                        fixed (float* b_ = &q[i, 0])
                        {
                            float dot = vectorDot(a_, b_, m);
                            for (int h = 0; h < m; h++)
                            {
                                q[j, h] -= dot * q[i, h];
                            }
                        }
                    }
                }
                float norm;
                unsafe
                {
                    fixed (float* a_ = &q[j, 0])
                    {
                        norm = vectorNorm(a_, m);
                    }
                }
                if (norm < 0.000001f)
                {
                    // vectors are linearly dependent or zero so no solution
                    if (DEBUG_STRATEGY)
                    {
                        Log.d(LOG_TAG, "  - no solution, norm=" + norm);
                    }
                    outDet = 0;
                    return false;
                }

                float invNorm = 1.0f / norm;
                for (int h = 0; h < m; h++)
                {
                    q[j, h] *= invNorm;
                }
                for (int i = 0; i < n; i++)
                {
                    unsafe
                    {
                        fixed (float* a_ = &q[j, 0])
                        fixed (float* b_ = &a[i, 0])
                        {
                            r[j, i] = i < j ? 0 : vectorDot(a_, b_, m);
                        }
                    }
                }
            }
            if (DEBUG_STRATEGY)
            {
                unsafe
                {
                    fixed (float* a_ = &q[0, 0])
                    fixed (float* b_ = &r[0, 0])
                    {
                        Log.d(LOG_TAG, "  - q=" + matrixToString(a_, m, n, false /*rowMajor*/));
                        Log.d(LOG_TAG, "  - r=" + matrixToString(b_, n, n, true /*rowMajor*/));
                    }
                }

                // calculate QR, if we factored A correctly then QR should equal A
                float[,] qr = new float[n, m];
                for (int h = 0; h < m; h++)
                {
                    for (int i = 0; i < n; i++)
                    {
                        qr[i, h] = 0;
                        for (int j = 0; j < n; j++)
                        {
                            qr[i, h] += q[j, h] * r[j, i];
                        }
                    }
                }
                unsafe
                {
                    fixed (float* a_ = &qr[0, 0])
                    {
                        Log.d(LOG_TAG, "  - qr=" + matrixToString(a_, m, n, false /*rowMajor*/));
                    }
                }
            }

            // Solve R B = Qt W Y to find B.  This is easy because R is upper triangular.
            // We just work from bottom-right to top-left calculating B's coefficients.
            float[] wy = new float[m];
            for (int h = 0; h < m; h++)
            {
                wy[h] = y[h] * w[h];
            }
            for (int i = n; i != 0;)
            {
                i--;
                unsafe
                {
                    fixed (float* a_ = &q[i, 0])
                    fixed (float* wyp = wy)
                    {
                        outB[i] = vectorDot(a_, wyp, m);
                    }
                }
                for (int j = n - 1; j > i; j--)
                {
                    outB[i] -= r[i, j] * outB[j];
                }
                outB[i] /= r[i, i];
            }
            if (DEBUG_STRATEGY)
            {
                Log.d(LOG_TAG, "  - b=" + vectorToString(outB, n));
            }

            // Calculate the coefficient of determination as 1 - (SSerr / SStot) where
            // SSerr is the residual sum of squares (variance of the error),
            // and SStot is the total sum of squares (variance of the data) where each
            // has been weighted.
            float ymean = 0;
            for (int h = 0; h < m; h++)
            {
                ymean += y[h];
            }
            ymean /= m;

            float sserr = 0;
            float sstot = 0;
            for (int h = 0; h < m; h++)
            {
                float err = y[h] - outB[0];
                float term = 1;
                for (int i = 1; i < n; i++)
                {
                    term *= x[h];
                    err -= term * outB[i];
                }
                sserr += w[h] * w[h] * err * err;
                float var = y[h] - ymean;
                sstot += w[h] * w[h] * var * var;
            }
            outDet = sstot > 0.000001f ? 1.0f - sserr / sstot : 1;
            if (DEBUG_STRATEGY)
            {
                Log.d(LOG_TAG, "  - sserr=" + sserr);
                Log.d(LOG_TAG, "  - sstot=" + sstot);
                Log.d(LOG_TAG, "  - det=" + outDet);
            }
            return true;
        }

        /*
         * Optimized unweighted second-order least squares fit. About 2x speed improvement compared to
         * the default implementation
         */
        static float[] solveUnweightedLeastSquaresDeg2(List<float> x, List<float> y)
        {
            int count = x.Count;
            if (count != y.Count) throw new Exception("Mismatched array sizes");
            // Solving y = a*x^2 + b*x + c
            float sxi = 0, sxiyi = 0, syi = 0, sxi2 = 0, sxi3 = 0, sxi2yi = 0, sxi4 = 0;

            for (int i = 0; i < count; i++)
            {
                float xi = x[i];
                float yi = y[i];
                float xi2 = xi * xi;
                float xi3 = xi2 * xi;
                float xi4 = xi3 * xi;
                float xiyi = xi * yi;
                float xi2yi = xi2 * yi;

                sxi += xi;
                sxi2 += xi2;
                sxiyi += xiyi;
                sxi2yi += xi2yi;
                syi += yi;
                sxi3 += xi3;
                sxi4 += xi4;
            }

            float Sxx = sxi2 - sxi * sxi / count;
            float Sxy = sxiyi - sxi * syi / count;
            float Sxx2 = sxi3 - sxi * sxi2 / count;
            float Sx2y = sxi2yi - sxi2 * syi / count;
            float Sx2x2 = sxi4 - sxi2 * sxi2 / count;

            float denominator = Sxx * Sx2x2 - Sxx2 * Sxx2;
            if (denominator == 0)
            {
                Log.w(LOG_TAG, "division by 0 when computing velocity, Sxx=" + Sxx + ", Sx2x2=" + Sx2x2 + ", Sxx2=" + Sxx2);
                return null;
            }
            // Compute a
            float numerator = Sx2y * Sxx - Sxy * Sxx2;
            float a = numerator / denominator;

            // Compute b
            numerator = Sxy * Sx2x2 - Sx2y * Sxx2;
            float b = numerator / denominator;

            // Compute c
            float c = syi / count - b * sxi / count - a * sxi2 / count;

            return new float[3] { c, b, a };
        }

        public override bool getEstimator(object id, NativeVelocityTracker.Estimator outEstimator)
        {
            outEstimator.clear();

            // Iterate over movement samples in reverse time order and collect samples.
            List<float> x = new();
            List<float> y = new();
            List<float> w = new();
            List<float> time = new();

            int index = mIndex;
            Movement newestMovement = mMovements[mIndex];
            do
            {
                Movement movement = mMovements[index];
                if (!movement.idBits.Contains(id))
                {
                    break;
                }

                long age = newestMovement.eventTime - movement.eventTime;
                if (age > HORIZON)
                {
                    break;
                }

                NativeVelocityTracker.Position position = movement.getPosition(id);
                x.Add(position.x);
                y.Add(position.y);
                w.Add(chooseWeight(index));
                time.Add(-age * 0.000000001f);
                index = (index == 0 ? HISTORY_SIZE : index) - 1;
            } while (x.Count < HISTORY_SIZE);

            int m = x.Count;
            if (m == 0)
            {
                return false; // no data
            }

            // Calculate a least squares polynomial fit.
            int degree = mDegree;
            if (degree > m - 1)
            {
                degree = m - 1;
            }

            if (degree == 2 && mWeighting == Weighting.WEIGHTING_NONE)
            {
                // Optimize unweighted, quadratic polynomial fit
                float[] xCoeff = solveUnweightedLeastSquaresDeg2(time, x);
                float[] yCoeff = solveUnweightedLeastSquaresDeg2(time, y);
                if (xCoeff != null && yCoeff != null)
                {
                    outEstimator.time = newestMovement.eventTime;
                    outEstimator.degree = 2;
                    outEstimator.confidence = 1;
                    for (int i = 0; i <= outEstimator.degree; i++)
                    {
                        outEstimator.xCoeff[i] = xCoeff[i];
                        outEstimator.yCoeff[i] = yCoeff[i];
                    }
                    return true;
                }
            }
            else if (degree >= 1)
            {
                // General case for an Nth degree polynomial fit
                float xdet, ydet;
                int n = degree + 1;
                if (solveLeastSquares(time, x, w, n, outEstimator.xCoeff, out xdet) &&
                    solveLeastSquares(time, y, w, n, outEstimator.yCoeff, out ydet))
                {
                    outEstimator.time = newestMovement.eventTime;
                    outEstimator.degree = degree;
                    outEstimator.confidence = xdet * ydet;
                    if (DEBUG_STRATEGY)
                    {
                        Log.d(LOG_TAG, "estimate: degree=" + outEstimator.degree
                            + ", xCoeff=" + vectorToString(outEstimator.xCoeff, n)
                            + ", yCoeff=" + vectorToString(outEstimator.yCoeff, n)
                            + ", confidence=" + outEstimator.confidence
                            );
                    }
                    return true;
                }
            }

            // No velocity data available for this pointer, but we do have its current position.
            outEstimator.xCoeff[0] = x[0];
            outEstimator.yCoeff[0] = y[0];
            outEstimator.time = newestMovement.eventTime;
            outEstimator.degree = 0;
            outEstimator.confidence = 1;
            return true;
        }

        float chooseWeight(int index)
        {
            switch (mWeighting)
            {
                case Weighting.WEIGHTING_DELTA:
                    {
                        // Weight points based on how much time elapsed between them and the next
                        // point so that points that "cover" a shorter time span are weighed less.
                        //   delta  0ms: 0.5
                        //   delta 10ms: 1.0
                        if (index == mIndex)
                        {
                            return 1.0f;
                        }
                        int nextIndex = (index + 1) % HISTORY_SIZE;
                        float deltaMillis = (mMovements[nextIndex].eventTime - mMovements[index].eventTime)
                                * 0.000001f;
                        if (deltaMillis < 0)
                        {
                            return 0.5f;
                        }
                        if (deltaMillis < 10)
                        {
                            return (float)(0.5f + deltaMillis * 0.05);
                        }
                        return 1.0f;
                    }

                case Weighting.WEIGHTING_CENTRAL:
                    {
                        // Weight points based on their age, weighing very recent and very old points less.
                        //   age  0ms: 0.5
                        //   age 10ms: 1.0
                        //   age 50ms: 1.0
                        //   age 60ms: 0.5
                        float ageMillis = (mMovements[mIndex].eventTime - mMovements[index].eventTime)
                                * 0.000001f;
                        if (ageMillis < 0)
                        {
                            return 0.5f;
                        }
                        if (ageMillis < 10)
                        {
                            return (float)(0.5f + ageMillis * 0.05);
                        }
                        if (ageMillis < 50)
                        {
                            return 1.0f;
                        }
                        if (ageMillis < 60)
                        {
                            return (float)(0.5f + (60 - ageMillis) * 0.05);
                        }
                        return 0.5f;
                    }

                case Weighting.WEIGHTING_RECENT:
                    {
                        // Weight points based on their age, weighing older points less.
                        //   age   0ms: 1.0
                        //   age  50ms: 1.0
                        //   age 100ms: 0.5
                        float ageMillis = (mMovements[mIndex].eventTime - mMovements[index].eventTime)
                                * 0.000001f;
                        if (ageMillis < 50)
                        {
                            return 1.0f;
                        }
                        if (ageMillis < 100)
                        {
                            return 0.5f + (100 - ageMillis) * 0.01f;
                        }
                        return 0.5f;
                    }

                case Weighting.WEIGHTING_NONE:
                default:
                    return 1.0f;
            }
        }

        // Sample horizon.
        // We don't use too much history by default since we want to react to quick
        // changes in direction.
        const long HORIZON = 100 * 1000000; // 100 ms

        // Number of samples to keep.
        const int HISTORY_SIZE = 20;

        int mDegree;
        Weighting mWeighting;
        int mIndex;
        Movement[] mMovements = new Movement[HISTORY_SIZE];
    }
}