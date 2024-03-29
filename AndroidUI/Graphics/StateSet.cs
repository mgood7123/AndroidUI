﻿using AndroidUI.Extensions;
using AndroidUI.Utils.Arrays;

namespace AndroidUI.Graphics
{
    /**
     * State sets are arrays of positive ints where each element
     * represents the state of a {@link android.view.View} (e.g. focused,
     * selected, visible, etc.).  A {@link android.view.View} may be in
     * one or more of those states.
     *
     * A state spec is an array of signed ints where each element
     * represents a required (if positive) or an undesired (if negative)
     * {@link android.view.View} state.
     *
     * Utils dealing with state sets.
     *
     * In theory we could encapsulate the state set and state spec arrays
     * and not have static methods here but there is some concern about
     * performance since these methods are called during view drawing.
     */
    public class StateSet
    {
        /**
         * The order here is very important to
         * {@link android.view.View#getDrawableState()}
         */
        private static int[][] VIEW_STATE_SETS;

        /** @hide */
        public const int VIEW_STATE_WINDOW_FOCUSED = 1;
        /** @hide */
        public const int VIEW_STATE_SELECTED = 1 << 1;
        /** @hide */
        public const int VIEW_STATE_FOCUSED = 1 << 2;
        /** @hide */
        public const int VIEW_STATE_ENABLED = 1 << 3;
        /** @hide */
        public const int VIEW_STATE_PRESSED = 1 << 4;
        /** @hide */
        public const int VIEW_STATE_ACTIVATED = 1 << 5;
        /** @hide */
        public const int VIEW_STATE_ACCELERATED = 1 << 6;
        /** @hide */
        public const int VIEW_STATE_HOVERED = 1 << 7;
        /** @hide */
        public const int VIEW_STATE_DRAG_CAN_ACCEPT = 1 << 8;
        /** @hide */
        public const int VIEW_STATE_DRAG_HOVERED = 1 << 9;

        internal static class R
        {
            internal static class attr
            {
                internal static readonly int[] ViewDrawableStates ={
                  0x0101009c, 0x0101009d, 0x0101009e, 0x010100a1,
                  0x010100a7, 0x010102fe, 0x0101031b, 0x01010367,
                  0x01010368, 0x01010369
                };

                /**
                 * State value for {@link android.graphics.drawable.StateListDrawable StateListDrawable},
                 * set when a view's window has input focus.
                 * <p>May be a boolean value, such as "<code>true</code>" or
                 * "<code>false</code>".
                 */
                internal const int state_window_focused = 0x0101009d;
                internal const int state_selected = 0x010100a1;
                internal const int state_focused = 0x0101009c;
                internal const int state_enabled = 0x0101009e;
                internal const int state_pressed = 0x010100a7;
                internal const int state_activated = 0x010102fe;
                internal const int state_accelerated = 0x0101031b;
                internal const int state_hovered = 0x01010367;
                internal const int state_drag_can_accept = 0x01010368;
                internal const int state_drag_hovered = 0x01010369;
                internal const int state_checked = 0x010100a0;
            }
        }

        static readonly int[] VIEW_STATE_IDS = new int[] {
                R.attr.state_window_focused,    VIEW_STATE_WINDOW_FOCUSED,
                R.attr.state_selected,          VIEW_STATE_SELECTED,
                R.attr.state_focused,           VIEW_STATE_FOCUSED,
                R.attr.state_enabled,           VIEW_STATE_ENABLED,
                R.attr.state_pressed,           VIEW_STATE_PRESSED,
                R.attr.state_activated,         VIEW_STATE_ACTIVATED,
                R.attr.state_accelerated,       VIEW_STATE_ACCELERATED,
                R.attr.state_hovered,           VIEW_STATE_HOVERED,
                R.attr.state_drag_can_accept,   VIEW_STATE_DRAG_CAN_ACCEPT,
                R.attr.state_drag_hovered,      VIEW_STATE_DRAG_HOVERED
        };

        static StateSet()
        {
            if (VIEW_STATE_IDS.Length / 2 != R.attr.ViewDrawableStates.Length)
            {
                throw new Exceptions.IllegalStateException(
                        "VIEW_STATE_IDs array length does not match ViewDrawableStates style array");
            }

            int[] orderedIds = new int[VIEW_STATE_IDS.Length];
            for (int i = 0; i < R.attr.ViewDrawableStates.Length; i++)
            {
                int viewState = R.attr.ViewDrawableStates[i];
                for (int j = 0; j < VIEW_STATE_IDS.Length; j += 2)
                {
                    if (VIEW_STATE_IDS[j] == viewState)
                    {
                        orderedIds[i * 2] = viewState;
                        orderedIds[i * 2 + 1] = VIEW_STATE_IDS[j + 1];
                    }
                }
            }

            int NUM_BITS = VIEW_STATE_IDS.Length / 2;
            VIEW_STATE_SETS = new int[1 << NUM_BITS][];
            for (int i = 0; i < VIEW_STATE_SETS.Length; i++)
            {
                int numBits = i.bitCount();
                int[] set = new int[numBits];
                int pos = 0;
                for (int j = 0; j < orderedIds.Length; j += 2)
                {
                    if ((i & orderedIds[j + 1]) != 0)
                    {
                        set[pos++] = orderedIds[j];
                    }
                }
                VIEW_STATE_SETS[i] = set;
            }
        }

        internal static int[] get(int mask)
        {
            if (mask >= VIEW_STATE_SETS.Length)
            {
                throw new Exception("Invalid state set mask");
            }
            return VIEW_STATE_SETS[mask];
        }

        /** @hide */
        internal StateSet() { }

        /**
         * A state specification that will be matched by all StateSets.
         */
        public static readonly int[] WILD_CARD = new int[0];

        /**
         * A state set that does not contain any valid states.
         */
        public static readonly int[] NOTHING = new int[] { 0 };

        /**
         * Return whether the stateSetOrSpec is matched by all StateSets.
         *
         * @param stateSetOrSpec a state set or state spec.
         */
        public static bool isWildCard(int[] stateSetOrSpec)
        {
            return stateSetOrSpec.Length == 0 || stateSetOrSpec[0] == 0;
        }

        /**
         * Return whether the stateSet matches the desired stateSpec.
         *
         * @param stateSpec an array of required (if positive) or
         *        prohibited (if negative) {@link android.view.View} states.
         * @param stateSet an array of {@link android.view.View} states
         */
        public static bool stateSetMatches(int[] stateSpec, int[] stateSet)
        {
            if (stateSet == null)
            {
                return stateSpec == null || isWildCard(stateSpec);
            }
            int stateSpecSize = stateSpec.Length;
            int stateSetSize = stateSet.Length;
            for (int i = 0; i < stateSpecSize; i++)
            {
                int stateSpecState = stateSpec[i];
                if (stateSpecState == 0)
                {
                    // We've reached the end of the cases to match against.
                    return true;
                }
                bool mustMatch;
                if (stateSpecState > 0)
                {
                    mustMatch = true;
                }
                else
                {
                    // We use negative values to indicate must-NOT-match states.
                    mustMatch = false;
                    stateSpecState = -stateSpecState;
                }
                bool found = false;
                for (int j = 0; j < stateSetSize; j++)
                {
                    int state = stateSet[j];
                    if (state == 0)
                    {
                        // We've reached the end of states to match.
                        if (mustMatch)
                        {
                            // We didn't find this must-match state.
                            return false;
                        }
                        else
                        {
                            // Continue checking other must-not-match states.
                            break;
                        }
                    }
                    if (state == stateSpecState)
                    {
                        if (mustMatch)
                        {
                            found = true;
                            // Continue checking other other must-match states.
                            break;
                        }
                        else
                        {
                            // Any match of a must-not-match state returns false.
                            return false;
                        }
                    }
                }
                if (mustMatch && !found)
                {
                    // We've reached the end of states to match and we didn't
                    // find a must-match state.
                    return false;
                }
            }
            return true;
        }

        /**
         * Return whether the state matches the desired stateSpec.
         *
         * @param stateSpec an array of required (if positive) or
         *        prohibited (if negative) {@link android.view.View} states.
         * @param state a {@link android.view.View} state
         */
        public static bool stateSetMatches(int[] stateSpec, int state)
        {
            int stateSpecSize = stateSpec.Length;
            for (int i = 0; i < stateSpecSize; i++)
            {
                int stateSpecState = stateSpec[i];
                if (stateSpecState == 0)
                {
                    // We've reached the end of the cases to match against.
                    return true;
                }
                if (stateSpecState > 0)
                {
                    if (state != stateSpecState)
                    {
                        return false;
                    }
                }
                else
                {
                    // We use negative values to indicate must-NOT-match states.
                    if (state == -stateSpecState)
                    {
                        // We matched a must-not-match case.
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * Check whether a list of state specs has an attribute specified.
         * @param stateSpecs a list of state specs we're checking.
         * @param attr an attribute we're looking for.
         * @return {@code true} if the attribute is contained in the state specs.
         * @hide
         */
        public static bool containsAttribute(int[][] stateSpecs, int attr)
        {
            if (stateSpecs != null)
            {
                foreach (int[] spec in stateSpecs)
                {
                    if (spec == null)
                    {
                        break;
                    }
                    foreach (int specAttr in spec)
                    {
                        if (specAttr == attr || -specAttr == attr)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int[] trimStateSet(int[] states, int newSize)
        {
            if (states.Length == newSize)
            {
                return states;
            }

            int[] trimmedStates = new int[newSize];
            Arrays.arraycopy(states, 0, trimmedStates, 0, newSize);
            return trimmedStates;
        }

        public static string dump(int[] states)
        {
            System.Text.StringBuilder sb = new();

            int count = states.Length;
            for (int i = 0; i < count; i++)
            {

                switch (states[i])
                {
                    case R.attr.state_window_focused:
                        sb.Append("W ");
                        break;
                    case R.attr.state_pressed:
                        sb.Append("P ");
                        break;
                    case R.attr.state_selected:
                        sb.Append("S ");
                        break;
                    case R.attr.state_focused:
                        sb.Append("F ");
                        break;
                    case R.attr.state_enabled:
                        sb.Append("E ");
                        break;
                    case R.attr.state_checked:
                        sb.Append("C ");
                        break;
                    case R.attr.state_activated:
                        sb.Append("A ");
                        break;
                }
            }

            return sb.ToString();
        }
    }
}