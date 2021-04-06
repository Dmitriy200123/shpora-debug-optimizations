using System;
using BenchmarkDotNet.Attributes;

namespace JpegBenchmarks.Benchmarks
{

    public class ExampleWithRedefinedFinalize
    {
        public string Name { get; set; }
        
        ~ExampleWithRedefinedFinalize()
        {
            Name = null;
        }
    }
    
    public class ExampleWithOutRedefinedFinalize
    {
        public string Name { get; set; }
    }
    
    
    [DisassemblyDiagnoser]
    public class GarbageCollectionBenchmark
    {
        
        [Benchmark]
        public void ClearListWithExampleWithRedefinedFinalize()
        {
            for (var i = 0; i < 10; i++)
            {
                CreateExample();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            } 
        }
        
        [Benchmark]
        public void ClearListWithExampleWithOutRedefinedFinalize()
        {
            for (var i = 0; i < 10; i++)
            {
                CreateExample1();
            } 
        }

        private void CreateExample()
        {
            var t = new ExampleWithRedefinedFinalize {Name = "empty"};
        }
        
        private void CreateExample1()
        {
            var t = new ExampleWithOutRedefinedFinalize {Name = "empty"};
        }
    }
}