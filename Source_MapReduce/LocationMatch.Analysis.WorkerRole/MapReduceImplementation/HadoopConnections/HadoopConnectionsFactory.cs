using System;

namespace LocationMatch.Analysis.WorkerRole.MapReduceImplementation.HadoopConnections
{
    public static class HadoopConnectionsFactory
    {
        private static IHadoopConnectionsFactory _instance;

        public static IHadoopConnectionsFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    Environment.SetEnvironmentVariable("HADOOP_HOME", @"c:\hadoop");
                    Environment.SetEnvironmentVariable("Java_HOME", @"c:\hadoop\jvm");

                    // Change this instantiation to change the HadoopConnectionsFactory type
                    _instance = new AzureHadoopConnectionsFactory();
                }

                return _instance;
            }
        }
    }
}