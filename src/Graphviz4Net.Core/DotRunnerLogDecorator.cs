namespace Graphviz4Net
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    public class DotRunnerLogDecorator : IDotRunner
    {
        private readonly IDotRunner runner;

        private readonly string filename;

        public DotRunnerLogDecorator(IDotRunner runner, string filename = "tmp")
        {
            this.runner = runner;
            this.filename = filename;
        }

        public TextReader RunDot(Action<TextWriter> writeGraph, Graphs.LayoutEngine engine = Graphs.LayoutEngine.Dot)
        {
            return Run(writeGraph, "dot", engine);
        }

        public TextReader Run(Action<TextWriter> writeGraph, string format, Graphs.LayoutEngine engine = Graphs.LayoutEngine.Dot)
        {
            string graph;

            using (StringWriter writer = new StringWriter())
            {
                writeGraph(writer);
                graph = writer.GetStringBuilder().ToString();
                string graphFile = Path.Combine(Path.GetTempPath(), filename + ".dot");
                File.WriteAllText(graphFile, graph);
            }


            // now we read the file and write it to the real process input.
            using (TextReader reader = runner.Run(w => w.Write(graph), format, engine))
            {
                // we read all output, save it into another file, and return it as a memory stream
                string text = reader.ReadToEnd();
                string layoutFile = Path.Combine(Path.GetTempPath(), filename + ".layout.dot");
                File.WriteAllBytes(layoutFile, Encoding.UTF8.GetBytes(text));
                return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(text)));
            }                

                
            
        }
    }
}