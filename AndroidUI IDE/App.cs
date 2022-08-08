using AndroidUI.Widgets;
using System.IO;
using View = AndroidUI.Widgets.View;

namespace AndroidUI.IDE
{
    internal class App : Applications.Application
    {
        Utils.LogTag Log = new("IDE");

        Utils.Lists.ArrayList<string> getProjects(string dir)
        {
            Log.d("searching directory '" + dir + "' for '.csproj' files");

            var csprojs = Directory.EnumerateFiles(dir, "*.csproj", SearchOption.AllDirectories);

            Utils.Lists.ArrayList<string> projectList = new Utils.Lists.ArrayList<string>();
            foreach (var s in csprojs)
            {
                Log.d("    found: '" + s + "'");
                projectList.Add(s);
            }
            Log.d("found " + projectList.Count + " projects");
            return projectList;
        }

        void buildProject(string project)
        {
            Log.d("building project: " + project);

            using var terminal = OS.Terminal.Create();

            using var r = terminal.Run("dotnet --build");
            r.WaitForExit();
            Log.d("EXITED WITH CODE: " + r.ProcessExitCode);


            terminal.RedirectOutput = true;
            
            using var p = terminal.Run("dotnet --build");
            
            p.WaitForExit();
            Log.d("DONE");

            var str = p.ReadOutput();
            if (str != null)
            {
                Log.d("read " + str.Length + " bytes");
                Console.Write(str);
            }
            Log.d("EXITED WITH CODE: " + p.ProcessExitCode);

            terminal.RedirectInput = true;
            terminal.RedirectOutput = false;
            using var p2 = terminal.Run("powershell");
            p2.SendInput("\n");
            p2.SendInput("help\n");
            p2.SendInput("q", 1000);
            p2.SendInput("\n", 600);
            p2.SendInput("\n");
            p2.SendInput("exit\n");
            p2.WaitForExit();
            Log.d("EXITED WITH CODE: " + p2.ProcessExitCode);

            terminal.RedirectOutput = false;
            terminal.RedirectInput = false;
            terminal.NeedsInput = false;
            Log.d("started dotnet process");
            terminal.Run("dotnet build " + project).Dispose();
            Log.d("dotnet process has ended");
        }

        class TextViewWriterStream : OS.WriteStream
        {
            View parent;
            Topten_RichTextKit_TextView textView;
            System.Text.StringBuilder builder;

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public TextViewWriterStream(View parent, Topten_RichTextKit_TextView textView) : base(new MemoryStream(), true)
            {
                this.parent = parent;
                this.textView = textView;
                builder = new();
            }

            protected override long OnSeek(Stream stream, long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override bool CanSeek => false;

            protected override int OnReadFromCopyTo(Stream stream, byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            protected override void OnWriteByte(Stream stream, byte value)
            {
                builder.Append((char)value);
                textView.setText(builder.ToString());
            }

            protected override void OnWrite(Stream stream, byte[] buffer, int offset, int count)
            {
                for (int i = 0; i < count; i++)
                {
                    builder.Append((char)buffer[i]);
                }
                textView.setText(builder.ToString());
            }

            protected override void OnWrite(Stream stream, ReadOnlySpan<byte> buffer)
            {
                throw new NotSupportedException();
            }
        }

        public override void OnCreate()
        {
            var projects = getProjects("C:\\Users\\small\\source\\repos");

            LinearLayout linearLayout = new();
            ScrollView ProjectContent = new();
            ScrollView ConsoleOutput = new();

            Topten_RichTextKit_TextView ConsoleOutputTextView = new();
            LinearLayout projectsList = new();
            foreach (string proj in projects)
            {
                FrameLayout row = new();
                var button = new Topten_RichTextKit_TextView("Build project", 18, SkiaSharp.SKColors.Black);
                button.setBackgroundColor((int)(uint)Utils.Const.Constants.color_code_LineageOS);
                setOnClickBuildProject(ConsoleOutput, ConsoleOutputTextView, proj, button);
                Topten_RichTextKit_TextView text = new();
                text.setText(proj);
                text.setTextSize(18);
                LinearLayout linear = new();
                linear.setOrientation(LinearLayout.OrientationMode.HORIZONTAL);
                linear.addView(button, new LinearLayout.LayoutParams(100, View.LayoutParams.WRAP_CONTENT));
                linear.addView(new Space(), new LinearLayout.LayoutParams(20, View.LayoutParams.WRAP_CONTENT));
                linear.addView(text, new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.WRAP_CONTENT));
                projectsList.addView(linear, new View.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.WRAP_CONTENT));
            }

            ConsoleOutputTextView.setText("Console Output");
            ConsoleOutputTextView.setTextColor(Graphics.Color.GRAY);
            ConsoleOutput.addView(ConsoleOutputTextView, View.MATCH_PARENT_W__MATCH_PARENT_H);
            ConsoleOutputTextView.setTag("Console Output TextView");
            ConsoleOutput.setTag("Console Output");
            ConsoleOutput.AutoScroll = true;

            var dotnet_help = new Topten_RichTextKit_TextView("dotnet help", 18, SkiaSharp.SKColors.Black);
            dotnet_help.setTag("dotnet help");
            dotnet_help.setBackgroundColor((int)(uint)Utils.Const.Constants.color_code_LineageOS);

            dotnet_help.setOnClickListener(v =>
            {
                Log.d("CLICKED");
                Task.Run(() =>
                {
                    lock (ConsoleOutputTextView)
                    {
                        Log.d("STARTED");
                        dotnet_help.setText("Executing dotnet help ...");

                        var terminal = OS.Terminal.Create();
                        terminal.NeedsInput = false;
                        terminal.RedirectOutput = true;

                        terminal.Run(
                            new TextViewWriterStream(ConsoleOutput, ConsoleOutputTextView),
                            "dotnet help "
                        );
                        terminal.Dispose();

                        dotnet_help.setText("dotnet help");
                    }
                });
            });

            var erase_console_output = new Topten_RichTextKit_TextView("Erase console output", 18, SkiaSharp.SKColors.Black);
            erase_console_output.setTag("Erase console output");
            erase_console_output.setBackgroundColor((int)(uint)Utils.Const.Constants.color_code_LineageOS);
            erase_console_output.setOnClickListener(v =>
            {
                Log.d("CLICKED");
                Task.Run(() =>
                {
                    lock (ConsoleOutputTextView)
                    {
                        erase_console_output.setText("Clearing...");
                        ConsoleOutputTextView.setTextColor(Graphics.Color.GRAY);
                        ConsoleOutputTextView.setText("Console Output");
                        erase_console_output.setText("Erase console output");
                    }
                });
            });

            linearLayout.addView(erase_console_output, new LinearLayout.LayoutParams(View.LayoutParams.WRAP_CONTENT, View.LayoutParams.WRAP_CONTENT));
            linearLayout.addView(dotnet_help, new LinearLayout.LayoutParams(View.LayoutParams.WRAP_CONTENT, View.LayoutParams.WRAP_CONTENT));

            ProjectContent.addView(projectsList, View.MATCH_PARENT_W__WRAP_CONTENT_H);
            ProjectContent.setTag("Project Content (holds Project List)");

            linearLayout.addView(ProjectContent, new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 2));
            linearLayout.addView(ConsoleOutput, new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1));
            linearLayout.setTag("Project List/Console Output");
            SetContentView(linearLayout);
        }

        private void setOnClickBuildProject(ScrollView ConsoleOutput, Topten_RichTextKit_TextView ConsoleOutputTextView, string proj, Topten_RichTextKit_TextView button)
        {
            button.setOnClickListener(v =>
            {
                Log.d("CLICKED");
                Task.Run(() =>
                {
                    lock (ConsoleOutputTextView)
                    {
                        Log.d("STARTED");
                        button.setText("Building...");

                        var terminal = OS.Terminal.Create();
                        terminal.NeedsInput = false;
                        terminal.RedirectOutput = true;

                        terminal.Run(
                            new TextViewWriterStream(ConsoleOutput, ConsoleOutputTextView),
                            "dotnet build " + proj
                        );
                        terminal.Dispose();

                        button.setText("Build project");
                    }
                });
            });
        }
    }
}