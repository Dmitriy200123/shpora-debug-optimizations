using BenchmarkDotNet.Running;

namespace DotTraceExamples
{
	class Program
	{
		static void Main(string[] args)
		{
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
			//ProgrammRunner.Run(new ComplexOperationTestProgram());
			//ProgrammRunner.Run(new EdgePreservingSmoothingProgram());
		}
	}
}
