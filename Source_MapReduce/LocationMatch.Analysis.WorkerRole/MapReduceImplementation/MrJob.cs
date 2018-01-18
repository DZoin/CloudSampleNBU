using Microsoft.Hadoop.MapReduce;

namespace LocationMatch.Analysis.WorkerRole.MapReduceImplementation
{
    public class MrJob<TMapper, TReducerCombiner> : HadoopJob<TMapper, TReducerCombiner>
    {
        public MrJob()
        {
        }

        public MrJob(string inputPath, string outputPath)
        {
        }

        public override HadoopJobConfiguration Configure(ExecutorContext context)
        {
            var config = new HadoopJobConfiguration();

            config.InputPath = context.Arguments[0];
            config.OutputFolder = context.Arguments[1];

            return config;
        }
    }
}
