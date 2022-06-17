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

namespace AndroidUI.Execution
{
    /**
     * Reference to a Handler, which others can use to send messages to it.
     * This allows for the implementation of message-based communication across
     * processes, by creating a Messenger pointing to a Handler in one process,
     * and handing that Messenger to another process.
     *
     * <p>Note: the implementation underneath is just a simple wrapper around
     * a {@link Binder} that is used to perform the communication.  This means
     * semantically you should treat it as such: this class does not impact process
     * lifecycle management (you must be using some higher-level component to tell
     * the system that your process needs to continue running), the connection will
     * break if your process goes away for any reason, etc.</p>
     */
    public sealed class Messenger
    {
        private readonly IMessenger mTarget;

        /**
         * Create a new Messenger pointing to the given Handler.  Any Message
         * objects sent through this Messenger will appear in the Handler as if
         * {@link Handler#sendMessage(Message) Handler.sendMessage(Message)} had
         * been called directly.
         * 
         * @param target The Handler that will receive sent messages.
         */
        public Messenger(Handler target)
        {
            mTarget = target.getIMessenger();
        }

        /**
         * Send a Message to this Messenger's Handler.
         * 
         * @param message The Message to send.  Usually retrieved through
         * {@link Message#obtain() Message.obtain()}.
         * 
         * @throws RemoteException Throws DeadObjectException if the target
         * Handler no longer exists.
         */
        public void send(Message message)
        {
            mTarget.send(message);
        }

        /**
         * Retrieve the IBinder that this Messenger is using to communicate with
         * its associated Handler.
         * 
         * @return Returns the IBinder backing this Messenger.
         */
        //public IBinder getBinder()
        //{
        //    return mTarget.asBinder();
        //}

        /**
         * Comparison operator on two Messenger objects, such that true
         * is returned then they both point to the same Handler.
         */
        //public boolean equals(@Nullable Object otherObj)
        //{
        //    if (otherObj == null)
        //    {
        //        return false;
        //    }
        //    try
        //    {
        //        return mTarget.asBinder().equals(((Messenger)otherObj)
        //                .mTarget.asBinder());
        //    }
        //    catch (ClassCastException e)
        //    {
        //    }
        //    return false;
        //}

        //public int hashCode()
        //{
        //    return mTarget.asBinder().hashCode();
        //}
    }
}