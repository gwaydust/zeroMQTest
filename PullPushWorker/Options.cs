namespace PullPushWorker
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using CommandLine;
    using CommandLine.Text;

    class Options : CommandLineOptionsBase
    {
        [Option("l", "pullEndPoint", Required = true, HelpText = "Pull end point")]
        public string pullEndPoint { get; set; }

        [Option("s", "pushEndPoint", Required = false, HelpText = "Push end point")]
        public string pushEndPoint { get; set; }

        [Option("t", "workerId", Required = true, HelpText = "Unique id to identify the worker")]
        public string workerId { get; set; }

        [Option("d", "delay", Required = false, HelpText = "Delay between messages (ms). Default = 0")]
        public int delay { get; set; }        

        [HelpOption(HelpText = "Dispaly this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = "PullPushWorker",               
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            this.HandleParsingErrorsInHelp(help);            
            help.AddPreOptionsLine("Usage: PullPushWorker.exe -l <Pull endpoint> -s -b <Pull endpoint> -t <worker ID> [-d <time delay>]");          
            help.AddOptions(this);

            return help;
        }

        private void HandleParsingErrorsInHelp(HelpText help)
        {
            if (this.LastPostParsingState.Errors.Count > 0)
            {
                var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
                    help.AddPreOptionsLine(errors);
                }
            }
        }
     
        public Options()
        {
            delay = 0;           
        }
    }
}
