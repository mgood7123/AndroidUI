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

using System.Diagnostics.Contracts;
using System.Text;

namespace AndroidUI.Utils
{
    public class LogTag
    {
        object DEFAULT_TAG = "DEFAULT_TAG";

        public LogTag()
        {
            DEFAULT_TAG = GetType().Name;
        }

        public LogTag(object obj_or_string)
        {
            if (obj_or_string == null)
            {
                DEFAULT_TAG = GetType().Name;
            }
            else if (obj_or_string is string)
            {
                DEFAULT_TAG = (string)obj_or_string;
            }
            else
            {
                DEFAULT_TAG = obj_or_string;
            }
        }

        public void WriteLine(string msg)
        {
            v(msg);
        }

        public void WriteLine(string msg, bool append_new_line)
        {
            v(msg, append_new_line);
        }

        public void v(string message) => Log.log_internal(DEFAULT_TAG.ToString(), message);
        public void i(string message) => Log.log_internal(DEFAULT_TAG.ToString(), message);
        public void d(string message) => Log.log_internal(DEFAULT_TAG.ToString(), message);
        public void w(string message) => Log.log_internal(DEFAULT_TAG.ToString(), message);
        public void e(string message) => Log.log_internal(DEFAULT_TAG.ToString(), message);

        public void v(string message, bool append_new_line) => Log.log_internal(DEFAULT_TAG.ToString(), message, append_new_line);
        public void i(string message, bool append_new_line) => Log.log_internal(DEFAULT_TAG.ToString(), message, append_new_line);
        public void d(string message, bool append_new_line) => Log.log_internal(DEFAULT_TAG.ToString(), message, append_new_line);
        public void w(string message, bool append_new_line) => Log.log_internal(DEFAULT_TAG.ToString(), message, append_new_line);
        public void e(string message, bool append_new_line) => Log.log_internal(DEFAULT_TAG.ToString(), message, append_new_line);


        public void v(string tag, string message) => Log.log_internal(tag, message);
        public void i(string tag, string message) => Log.log_internal(tag, message);
        public void d(string tag, string message) => Log.log_internal(tag, message);
        public void w(string tag, string message) => Log.log_internal(tag, message);
        public void e(string tag, string message) => Log.log_internal(tag, message);

        public void v(string tag, string message, bool append_new_line) => Log.log_internal(tag, message, append_new_line);
        public void i(string tag, string message, bool append_new_line) => Log.log_internal(tag, message, append_new_line);
        public void d(string tag, string message, bool append_new_line) => Log.log_internal(tag, message, append_new_line);
        public void w(string tag, string message, bool append_new_line) => Log.log_internal(tag, message, append_new_line);
        public void e(string tag, string message, bool append_new_line) => Log.log_internal(tag, message, append_new_line);
    }

    public static class Log
    {
        public static void WriteLine(string msg)
        {
            WriteLine(msg, true);
        }

        public static void WriteLine(string msg, bool append_new_line)
        {
            if (append_new_line)
            {
                Console.WriteLine(msg);
            }
            else
            {
                Console.Write(msg);
            }
        }

        internal static void log_internal(string tag, string message, bool append_new_line = true)
        {
            WriteLine(tag + ": " + message, append_new_line);
        }

        public static void v(string tag, string message) => log_internal(tag, message);
        public static void i(string tag, string message) => log_internal(tag, message);
        public static void d(string tag, string message) => log_internal(tag, message);
        public static void w(string tag, string message) => log_internal(tag, message);
        public static void e(string tag, string message) => log_internal(tag, message);

        public static void v(string tag, string message, bool append_new_line) => log_internal(tag, message, append_new_line);
        public static void i(string tag, string message, bool append_new_line) => log_internal(tag, message, append_new_line);
        public static void d(string tag, string message, bool append_new_line) => log_internal(tag, message, append_new_line);
        public static void w(string tag, string message, bool append_new_line) => log_internal(tag, message, append_new_line);
        public static void e(string tag, string message, bool append_new_line) => log_internal(tag, message, append_new_line);
    }

    public class NewlineConvertingWriter : StringWriter
    {
        readonly object LOCK = new();
        StringBuilder output = new();

        public NewlineConvertingWriter()
        {
            FlushOnWrite = true;
        }

        public NewlineConvertingWriter(string target_new_line) : base(target_new_line)
        {
        }

        protected override void OnFlush(string text)
        {
            lock (LOCK)
            {
                output.Append(text);
            }
        }

        public string GetConversionResult()
        {
            lock (LOCK)
            {
                return output.ToString();
            }
        }

        public void ClearConversionResult()
        {
            lock (LOCK)
            {
                output = new();
            }
        }
    }

    public class LogWriter : StringWriter
    {
        private string TAG;
        public LogWriter(string tag)
        {
            TAG = tag;
        }

        protected override void OnFlush(string text)
        {
            Log.WriteLine(text, false);
        }

        protected override void OnNewLineAfterFlush()
        {
            Log.WriteLine(TAG + ": ", false);
        }
    }

    class ConsoleStreamWrapper : MemoryStream
    {
        public override void Write(byte[] buffer, int offset, int count)
        {
            char[] str = new char[count];
            for (int i = offset; i < count; i++)
            {
                str[i] = (char)buffer[i];
            }
            Console.Out.Write(str, 0, count);
            Console.Out.Flush();
        }

        public override void WriteByte(byte value)
        {
            Console.Out.Write((char)value);
            Console.Out.Flush();
        }

        public override void Flush()
        {
            Console.Out.Flush();
        }
    }

    class LogStream : MemoryStream
    {
        LogWriter w;

        private bool flushOnWrite;

        public LogStream(string tag)
        {
            w = new(tag);
        }

        public LogStream(string tag, bool flushOnWrite)
        {
            w = new(tag);
            this.flushOnWrite = flushOnWrite;
        }

        public bool FlushOnWrite
        {
            get
            {
                lock (w)
                {
                    return flushOnWrite;
                }
            }

            set
            {
                lock (w)
                {
                    flushOnWrite = value;
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            char[] str = new char[count];
            for (int i = offset; i < count; i++)
            {
                str[i] = (char)buffer[i];
            }
            w.Write(str, 0, count);
            lock (w)
            {
                if (FlushOnWrite) w.Flush();
            }
        }

        public override void WriteByte(byte value)
        {
            w.Write((char)value);
            lock (w)
            {
                if (flushOnWrite) w.Flush();
            }
        }
    }

    public class StringWriter : TextWriter
    {
        private static volatile UnicodeEncoding m_encoding = null;
        private Stack<int> indentLevel;
        private Stack<int> indentSize;
        private bool _isOpen;
        private StringBuilder _sb;

        NewLineConverter newLineConverter;

        static bool IO_LOCKED = true;
        private static readonly object LOCK = new();
        private static readonly object IO_LOCK = new();

        public static void UnlockIO()
        {
            lock (IO_LOCK)
            {
                IO_LOCKED = false;
            }
        }

        public static void LockIO()
        {
            lock (IO_LOCK)
            {
                IO_LOCKED = true;
            }
        }

        bool flushed;

        public StringWriter() : this(Environment.NewLine)
        {
        }

        public StringWriter(string target_newline)
        {
            indentLevel = new();
            indentSize = new();
            indentLevel.Push(0);
            indentSize.Push(4);
            _isOpen = true;
            _sb = new StringBuilder();
            newLineConverter = new(this);
            newLineConverter.AddConversion("\r\n", target_newline);
            newLineConverter.AddConversion("\n", target_newline);
            flushed = true;
            //OnNewLineAfterFlush();
        }

        public string GetNewLineConversionTarget()
        {
            return newLineConverter.GetNewLineConversionTargetAt(0);
        }

        public void SetNewLineConversionTarget(string target_newline)
        {
            newLineConverter.SetNewLineConversionTarget(target_newline);
        }

        public override Encoding Encoding
        {
            get
            {
                if (m_encoding == null)
                {
                    m_encoding = new UnicodeEncoding(false, false);
                }
                return m_encoding;
            }
        }

        protected virtual void OnFlush(string text)
        {

        }

        protected virtual void OnNewLineBeforeFlush()
        {

        }

        protected virtual void OnNewLineAfterFlush()
        {

        }

        public override void Flush()
        {
            base.Flush();
            OnFlush(_sb.ToString());
            _sb.Clear();
        }

        private bool flushOnWrite;

        public bool FlushOnWrite
        {
            get
            {
                lock (LOCK)
                {
                    return flushOnWrite;
                }
            }

            set
            {
                lock (LOCK)
                {
                    flushOnWrite = value;
                }
            }
        }

        public void Clear()
        {
            lock (LOCK)
            {
                _sb.Clear();
            }
        }

        public override void Close()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            // Do not destroy _sb, so that we can extract this after we are
            // done writing (similar to MemoryStream's GetBuffer & ToArray methods)
            _isOpen = false;
            base.Dispose(disposing);
        }

        private void indent()
        {
            flushed = false;
            int indentLevel = this.indentLevel.Peek();
            int indentSize = this.indentSize.Peek();
            for (int i = 0; i < indentLevel; i++)
            {
                for (int _ = 0; _ < indentSize; _++)
                {
                    _sb.Append(' ');
                }
            }
        }

        bool doConversion = true;

        public void DisableNewLineConversion()
        {
            lock (LOCK)
            {
                doConversion = false;
            }
        }

        public void EnableNewLineConversion()
        {
            lock (LOCK)
            {
                doConversion = true;
            }
        }

        // Writes a character to the underlying string buffer.
        //
        public override void Write(char value)
        {
            if (!_isOpen)
                WriterClosed();

            if (flushed) indent();
            lock (LOCK)
            {
                if (doConversion)
                {
                    (string str, bool isNewLine) = newLineConverter.ProcessNext(value);

                    // flush if newLineConverter flushes before us
                    if (flushed) indent();

                    if (str != null)
                    {
                        _sb.Append(str);
                        if (isNewLine)
                        {
                            OnNewLineBeforeFlush();
                            Flush();
                            OnNewLineAfterFlush();
                            flushed = true;
                        }
                        else
                        {
                            if (flushOnWrite) Flush();
                        }
                    }
                }
                else
                {
                    base.Write(value);
                    if (flushOnWrite) Flush();
                }
            }
        }

        internal static void WriterClosed()
        {
            throw new ObjectDisposedException(null, "Writer Closed");
        }

        // Writes a range of a character array to the underlying string buffer.
        // This method will write count characters of data into this
        // StringWriter from the buffer character array starting at position
        // index.
        //
        public override void Write(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "Null Buffer");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Need Non-Negative Num");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Need Non-Negative Numm");
            if (buffer.Length - index < count)
                throw new ArgumentException("Invalid Offset Length");
            Contract.EndContractBlock();

            if (!_isOpen)
                WriterClosed();

            bool locked = false;
            lock (IO_LOCK)
            {
                locked = IO_LOCKED;
            }

            if (locked)
            {
                lock (LOCK)
                {
                    for (int i = index; i < count; i++) Write(buffer[i]);
                }
            }
            else
            {
                for (int i = index; i < count; i++) Write(buffer[i]);
            }
        }

        // Writes a string to the underlying string buffer. If the given string is
        // null, nothing is written.
        //
        public override void Write(string value)
        {
            if (!_isOpen)
                WriterClosed();

            bool locked = false;
            lock (IO_LOCK)
            {
                locked = IO_LOCKED;
            }

            if (locked)
            {
                lock (LOCK)
                {
                    foreach (char c in value) Write(c);
                }
            }
            else
            {
                foreach (char c in value) Write(c);
            }
        }

        // Returns a string containing the characters written to this TextWriter
        // so far.
        //
        public override string ToString()
        {
            lock (LOCK)
            {
                return _sb.ToString();
            }
        }

        public void pushIndent()
        {
            lock (LOCK)
            {
                indentLevel.Push(indentLevel.Peek() + 1);
            }
        }

        public void popIndent()
        {
            lock (LOCK)
            {
                if (indentLevel.Count > 1)
                {
                    indentLevel.Pop();
                }
            }
        }

        public int getIndentLevel()
        {
            return indentLevel.Peek();
        }

        public void pushIndentSize(int size)
        {
            lock (LOCK)
            {
                indentSize.Push(size);
            }
        }

        public void popIndentSize()
        {
            lock (LOCK)
            {
                if (indentSize.Count > 1)
                {
                    indentSize.Pop();
                }
            }
        }

        public int getIndentSize()
        {
            return indentSize.Peek();
        }

        public static StringWriter operator +(StringWriter left, StringWriter right)
        {
            left.Write(right);
            return left;
        }

        public static StringWriter operator +(StringWriter left, string right)
        {
            left.Write(right);
            return left;
        }

        public static StringWriter operator +(StringWriter left, char right)
        {
            left.Write(right);
            return left;
        }

        public static StringWriter operator +(StringWriter left, object right)
        {
            left.Write(right.ToString());
            return left;
        }
    }

    class NewLineConverter
    {
        TextWriter textWriter;

        LinkedList<char> buffer;

        List<(NewLineDetector from, string to)> conversions;
        List<(NewLineDetector from, string to)> eliminated;
        private static object LOCK = new();

        public NewLineConverter(TextWriter textWriter)
        {
            this.textWriter = textWriter;
            conversions = new List<(NewLineDetector from, string to)>();
            eliminated = new List<(NewLineDetector from, string to)>();
            buffer = new LinkedList<char>();
        }

        public string GetNewLineConversionTargetAt(int index)
        {
            if (index < 0) throw new IndexOutOfRangeException("index cannot be negative");
            lock (LOCK)
            {
                return conversions[index].to;
            }
        }

        public void SetNewLineConversionTarget(string target_newline)
        {
            if (target_newline == null) throw new ArgumentNullException(nameof(target_newline));
            lock (LOCK)
            {
                for (int i = 0; i < conversions.Count; i++)
                {
                    (NewLineDetector from, string to) c = conversions[i];
                    c.to = target_newline;
                    conversions[i] = c;
                }
            }
        }

        public void AddConversion(string from, string to)
        {
            lock (LOCK)
            {
                NewLineDetector conversion = new(textWriter, from);
                conversions.Add((conversion, to));
                conversions.Sort((a, b) =>
                {
                    string sa = a.from.NewLine?.Invoke();
                    int NL_A = sa != null ? sa.Length : 0;

                    string sb = b.from.NewLine?.Invoke();
                    int NL_B = sb != null ? sb.Length : 0;

                    return Comparer<int>.Default.Compare(NL_A, NL_B);
                });
            }
        }

        bool flushing_input = false;

        public (string str, bool isNewLine) ProcessNext(char c)
        {
            lock (LOCK)
            {
                if (flushing_input)
                {
                    // prevent recursion
                    flushing_input = false;
                    return ("" + c, c == '\r' || c == '\n');
                }
                if (conversions.Count == 0)
                {
                    // we have no available conversions
                    return ("" + c, false);
                }

                bool bufferScan = false;

                char firstCharacter = c;

                while (true)
                {
                    // loop throuch each detector
                    // keep detecting until all detectors are exhausted

                    bool matched = false;
                    bool isPartialMatch = false;
                    string replacement = null;

                    foreach ((NewLineDetector newLineDetector, string to) x in conversions)
                    {
                        if (eliminated.Contains(x))
                        {
                            // this detector has been eliminated
                            continue;
                        }
                        if (x.newLineDetector.processNext(firstCharacter))
                        {
                            // we got a complete match
                            matched = true;
                            replacement = "" + x.to;
                            break;
                        }
                        if (x.newLineDetector.getIndex() != 0)
                        {
                            // we got a partial match
                            isPartialMatch = true;
                            continue;
                        }
                        else
                        {
                            // we got no match, eliminate
                            eliminated.Add(x);
                            continue;
                        }
                    }

                    if (matched)
                    {
                        if (!bufferScan)
                        {
                            // we got a match, clear the buffer
                            buffer.Clear();
                            eliminated.Clear();
                        }
                        else
                        {
                            // we got a match, flush it later
                        }
                    }
                    else if (isPartialMatch)
                    {
                        if (!bufferScan)
                        {
                            // we got a partial match
                            buffer.AddLast(firstCharacter);
                            // nothing should be done while we have a partial match
                            return (null, false);
                        }
                        else
                        {
                            // we got a partial match
                            // do nothing and advance to next character
                        }
                    }
                    else
                    {
                        // we got no matches
                        eliminated.Clear();
                        if (!bufferScan && buffer.Count != 0)
                        {
                            buffer.AddLast(firstCharacter);
                        }
                    }

                    if (!bufferScan || !isPartialMatch)
                    {
                        // if !bufferScan
                        // we got a match, or we didnt get any matches at all

                        // if bufferScan
                        // we got a match or we didnt get any matches at all
                        // we cannot reset state during partial match in a bufferScan

                        // reset detector states
                        foreach ((NewLineDetector newLineDetector_, _) in conversions)
                        {
                            newLineDetector_.reset();
                        }
                    }

                    if (matched)
                    {
                        if (!bufferScan)
                        {
                            // we got a match, return its replacement
                            return (replacement, true);
                        }
                        else
                        {
                            // we got a match
                            // flush our replacement to text writer
                            textWriter.Write(replacement);
                        }
                    }

                    if (bufferScan)
                    {
                        // save first character
                        char saved = firstCharacter;

                        // remove first character to proceed to the next
                        buffer.RemoveFirst();

                        // return early if we can
                        if (buffer.Count == 0)
                        {
                            // buffer has become empty, we can no longer match anything
                            return ("" + saved, false);
                        }
                        else
                        {
                            // we still have input in the buffer
                            // flush our input to text writer
                            flushing_input = true;
                            textWriter.Write(saved);

                            // set our input to the first character
                            firstCharacter = buffer.First.Value;
                            continue;
                        }
                    }
                    else
                    {
                        // at this point, we didnt get a match
                        if (buffer.Count == 0)
                        {
                            // buffer is empty, no matches whatsoever
                            return ("" + firstCharacter, false);
                        }

                        // rescan the buffer
                        bufferScan = true;

                        // set our input to the first character
                        firstCharacter = buffer.First.Value;
                        continue;
                    }
                }
            }
        }
    }

    public class NewLineDetector
    {
        TextWriter textWriter;
        int index;
        int previousIndex;
        public RunnableWithReturn<string> NewLine;
        private static object LOCK = new();

        public NewLineDetector(TextWriter textWriter, string from)
        {
            this.textWriter = textWriter;
            NewLine = () => from ?? this.textWriter.NewLine;
            reset();
        }

        public NewLineDetector(TextWriter textWriter) : this(textWriter, null)
        {
        }

        public bool processNext(char c)
        {
            lock (LOCK)
            {
                string n = NewLine?.Invoke();
                if (n == null || n.Length == 0)
                    return false;

                previousIndex = index;

                if (n[index] == c)
                {
                    // n[0] = '\r'
                    // n[1] = '\n'
                    // index = 0
                    // n.Length = 2
                    // 0 == 1
                    if (index == n.Length - 1)
                    {
                        index = 0;
                        return true;
                    }
                    index++;
                }
                else
                {
                    index = 0;
                }
                return false;
            }
        }

        public void reset()
        {
            lock (LOCK)
            {
                index = 0;
                previousIndex = 0;
            }
        }

        public int getIndex() => index;
        public int getPreviousIndex() => previousIndex;
    }
}