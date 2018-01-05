using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.TemplateEngine.Cli
{
    public abstract class GenericProcess<T>
        where T : GenericProcess<T>, new()
    {
        private ProcessStartInfo _info;
        private DataReceivedEventHandler _errorDataReceived;
        private StringBuilder _stderr;
        private StringBuilder _stdout;
        private DataReceivedEventHandler _outputDataReceived;
        private bool _anyNonEmptyStderrWritten;

        protected static T Configure(string command, string args)
        {
            return new T
            {
                _info = new ProcessStartInfo(command, args)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
        }

        protected static T EscapeArgsAndConfigure(string command, IEnumerable<string> args)
        {
            string realArgs = ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(args);
            return Configure(command, realArgs);
        }

        public T ForwardStdErr()
        {
            _errorDataReceived = ForwardStreamStdErr;
            return (T)this;
        }

        public T ForwardStdOut()
        {
            _outputDataReceived = ForwardStreamStdOut;
            return (T)this;
        }

        private void ForwardStreamStdOut(object sender, DataReceivedEventArgs e)
        {
            Console.Out.WriteLine(e.Data);
        }

        private void ForwardStreamStdErr(object sender, DataReceivedEventArgs e)
        {
            if (!_anyNonEmptyStderrWritten)
            {
                if (string.IsNullOrWhiteSpace(e.Data))
                {
                    return;
                }

                _anyNonEmptyStderrWritten = true;
            }

            Console.Error.WriteLine(e.Data);
        }

        public T CaptureStdOut()
        {
            _stdout = new StringBuilder();
            _outputDataReceived += CaptureStreamStdOut;
            return (T)this;
        }

        private void CaptureStreamStdOut(object sender, DataReceivedEventArgs e)
        {
            _stdout.AppendLine(e.Data);
        }

        public T CaptureStdErr()
        {
            _stderr = new StringBuilder();
            _errorDataReceived += CaptureStreamStdErr;
            return (T)this;
        }

        private void CaptureStreamStdErr(object sender, DataReceivedEventArgs e)
        {
            _stderr.AppendLine(e.Data);
        }

        public Result Execute()
        {
            Process p = Process.Start(_info);
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.ErrorDataReceived += OnErrorDataReceived;
            p.OutputDataReceived += OnOutputDataReceived;
            p.WaitForExit();

            return new Result(_stdout?.ToString(), _stderr?.ToString(), p.ExitCode);
        }

        protected string EscapeAndConcatenateArgArrayForProcessStart(IEnumerable<string> args)
        {
            return ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(args);
        }

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            _errorDataReceived?.Invoke(sender, e);
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            _outputDataReceived?.Invoke(sender, e);
        }

        public class Result
        {
            public Result(string stdout, string stderr, int exitCode)
            {
                StdErr = stderr;
                StdOut = stdout;
                ExitCode = exitCode;
            }

            public string StdErr { get; }

            public string StdOut { get; }

            public int ExitCode { get; }
        }
    }
}
