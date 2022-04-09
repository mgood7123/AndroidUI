﻿namespace AndroidUI
{
    /**
     * A cache class that can provide new instances of a particular resource which may change
     * depending on the current {@link Resources.Theme} or {@link Configuration}.
     * <p>
     * A constant state should be able to return a bitmask of changing configurations, which
     * identifies the type of configuration changes that may invalidate this resource. These
     * configuration changes can be obtained from {@link android.util.TypedValue}. Entities such as
     * {@link android.animation.Animator} also provide a changing configuration method to include
     * their dependencies (e.g. An AnimatorSet's changing configuration is the union of the
     * changing configurations of each Animator in the set)
     * @hide
     */
    abstract public class ConstantState<T>
    {

        /**
         * Return a bit mask of configuration changes that will impact
         * this resource (and thus require completely reloading it).
         */
        abstract public int getChangingConfigurations();

        /**
         * Create a new instance without supplying resources the caller
         * is running in.
         */
        public abstract T newInstance();

        /**
         * Create a new instance from its constant state.  This must be
         * implemented for resources that can have a theme applied.
         */
        public virtual T newInstance(Theme theme)
        {
            return newInstance();
        }
    }
}