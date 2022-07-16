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

namespace AndroidUI.Widgets
{
    /**
     * A user interface element the user can tap or click to perform an action.
     *
     * <p>To display a button in an activity, add a button to the activity's layout XML file:</p>
     *
     * <pre>
     * &lt;Button
     *     android:id="@+id/button_id"
     *     android:layout_height="wrap_content"
     *     android:layout_width="wrap_content"
     *     android:text="@string/self_destruct" /&gt;</pre>
     *
     * <p>To specify an action when the button is pressed, set a click
     * listener on the button object in the corresponding activity code:</p>
     *
     * <pre>
     * public class MyActivity extends Activity {
     *     protected void onCreate(Bundle savedInstanceState) {
     *         super.onCreate(savedInstanceState);
     *
     *         setContentView(R.layout.content_layout_id);
     *
     *         final Button button = findViewById(R.id.button_id);
     *         button.setOnClickListener(new View.OnClickListener() {
     *             public void onClick(View v) {
     *                 // Code here executes on main thread after user presses button
     *             }
     *         });
     *     }
     * }</pre>
     *
     * <p>The above snippet creates an instance of {@link android.view.View.OnClickListener} and wires
     * the listener to the button using
     * {@link #setOnClickListener setOnClickListener(View.OnClickListener)}.
     * As a result, the system executes the code you write in {@code onClick(View)} after the
     * user presses the button.</p>
     *
     * <p class="note">The system executes the code in {@code onClick} on the
     * <a href="{@docRoot}guide/components/processes-and-threads.html#Threads">main thread</a>.
     * This means your onClick code must execute quickly to avoid delaying your app's response
     * to further user actions.  See
     * <a href="{@docRoot}training/articles/perf-anr.html">Keeping Your App Responsive</a>
     * for more details.</p>
     *
     * <p>Every button is styled using the system's default button background, which is often
     * different from one version of the platform to another. If you are not satisfied with the
     * default button style, you can customize it. For more details and code samples, see the
     * <a href="{@docRoot}guide/topics/ui/controls/button.html#Style">Styling Your Button</a>
     * guide.</p>
     *
     * <p>For all XML style attributes available on Button see
     * {@link android.R.styleable#Button Button Attributes},
     * {@link android.R.styleable#TextView TextView Attributes},
     * {@link android.R.styleable#View View Attributes}.  See the
     * <a href="{@docRoot}guide/topics/ui/themes.html#ApplyingStyles">Styles and Themes</a>
     * guide to learn how to implement and organize overrides to style-related attributes.</p>
     */
    public class Button : Topten_RichTextKit_TextView
    {

        /**
         * Simple constructor to use when creating a button from code.
         *
         * @param context The Context the Button is running in, through which it can
         *        access the current theme, resources, etc.
         *
         * @see #Button(Context, AttributeSet)
         */
        public Button()
        {
        }
    }
}
