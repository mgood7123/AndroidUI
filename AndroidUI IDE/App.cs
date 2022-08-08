using AndroidUI.Utils;
using AndroidUI.Utils.Widgets;
using AndroidUI.Widgets;
using System.IO;
using static AndroidUI.Utils.Runnable;
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
            return projectList;
            // dotnet build "C:/Users/small/source/repos/AndroidUI\AndroidUI-Application-Windows\AndroidUI-Application-Windows.csproj"
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
            terminal.Run("dotnet build " + project).Dispose();

            //Log.d("started dotnet process");
            //terminal.Run("IDE", "dotnet build \"" + project + "\"");
            //Log.d("dotnet process has ended");
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
            FlywheelScrollView ProjectContent = new();
            LinearLayout projectsList = new();
            FlywheelScrollView ConsoleOutput = new();

            Topten_RichTextKit_TextView ConsoleTextView = new();
            foreach (string proj in projects)
            {
                FrameLayout row = new();

                var button = new Widgets.Topten_RichTextKit_TextView("Build project", 18, SkiaSharp.SKColors.Black);
                button.setTag("Build Project");
                button.setBackgroundColor((int)(uint)Utils.Const.Constants.color_code_LineageOS);

                button.setOnClickListener(v =>
                {
                    Log.d("CLICKED");
                    Task.Run(() =>
                    {
                        lock (ConsoleTextView)
                        {
                            Log.d("STARTED");
                            button.setText("Building...");

                            var terminal = OS.Terminal.Create();
                            terminal.NeedsInput = false;
                            terminal.RedirectOutput = true;

                            terminal.Run(
                                new TextViewWriterStream(ConsoleOutput, ConsoleTextView),
                                "dotnet build " + proj
                            );
                            terminal.Dispose();

                            button.setText("Build project");
                        }
                    });
                });

                Topten_RichTextKit_TextView text = new();
                text.setText(proj);
                text.setTextSize(18);


                LinearLayout linear = new();

                linear.setOrientation(LinearLayout.HORIZONTAL);

                linear.addView(button, new LinearLayout.LayoutParams(View.LayoutParams.WRAP_CONTENT, View.LayoutParams.WRAP_CONTENT));
                linear.addView(new Space(), new LinearLayout.LayoutParams(20, View.LayoutParams.WRAP_CONTENT));
                linear.addView(text, new LinearLayout.LayoutParams(View.LayoutParams.WRAP_CONTENT, View.LayoutParams.WRAP_CONTENT, 1));
                linear.setTag("build project: " + proj);

                projectsList.addView(linear, new Layout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.WRAP_CONTENT));
            }
            projectsList.setTag("Project List");

            ConsoleTextView.setTextColor(Graphics.Color.GRAY);
            ConsoleOutput.addView(ConsoleTextView, View.MATCH_PARENT_W__MATCH_PARENT_H);
            ConsoleTextView.setTag("Console TextView");
            ConsoleOutput.setTag("Console Output");
            ConsoleOutput.AutoScroll = true;

            var dotnet_help = new Widgets.Topten_RichTextKit_TextView("dotnet help", 18, SkiaSharp.SKColors.Black);
            dotnet_help.setTag("dotnet help");
            dotnet_help.setBackgroundColor((int)(uint)Utils.Const.Constants.color_code_LineageOS);

            dotnet_help.setOnClickListener(v =>
            {
                Log.d("CLICKED");
                Task.Run(() =>
                {
                    lock (ConsoleTextView)
                    {
                        Log.d("STARTED");
                        dotnet_help.setText("Executing dotnet help ...");

                        var terminal = OS.Terminal.Create();
                        terminal.NeedsInput = false;
                        terminal.RedirectOutput = true;

                        terminal.Run(
                            new TextViewWriterStream(ConsoleOutput, ConsoleTextView),
                            "dotnet help "
                        );
                        terminal.Dispose();

                        dotnet_help.setText("dotnet help");
                    }
                });
            });

            var erase_console_output = new Widgets.Topten_RichTextKit_TextView("Erase console output", 18, SkiaSharp.SKColors.Black);
            erase_console_output.setTag("Erase console output");
            erase_console_output.setBackgroundColor((int)(uint)Utils.Const.Constants.color_code_LineageOS);
            erase_console_output.setOnClickListener(v =>
            {
                Log.d("CLICKED");
                Task.Run(() =>
                {
                    lock (ConsoleTextView)
                    {
                        erase_console_output.setText("Clearing...");
                        ConsoleTextView.setText("");
                        erase_console_output.setText("Erase console output");
                    }
                });
            });

            linearLayout.addView(erase_console_output, new LinearLayout.LayoutParams(View.LayoutParams.WRAP_CONTENT, View.LayoutParams.WRAP_CONTENT));
            linearLayout.addView(dotnet_help, new LinearLayout.LayoutParams(View.LayoutParams.WRAP_CONTENT, View.LayoutParams.WRAP_CONTENT));

            ProjectContent.addView(projectsList, View.MATCH_PARENT_W__MATCH_PARENT_H);
            ProjectContent.setTag("Project Content (holds Project List)");

            linearLayout.addView(ProjectContent, new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 2));
            linearLayout.addView(ConsoleOutput, new LinearLayout.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT, 1));
            linearLayout.setTag("Project List/Console Output");
            SetContentView(linearLayout);
        }
    }
}