using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using FluentResults;
using CSharpFunctionalExtensions;
using Res;

namespace ResBenchmarks;


record BenchError
{
    public sealed record FailedToBenchError() : BenchError;
    public sealed record DoYouEventLiftError() : BenchError;
}

public class Config : ManualConfig
{
    public Config()
    {
        WithOptions(ConfigOptions.DisableOptimizationsValidator);
    }
}

[MemoryDiagnoser]
[Config(typeof(Config))]
public class ResVsOthers
{
    private readonly int iterations = 1000;
    
    [Benchmark]
    public void FluentResultsOkWrapUnwrapBench()
    {
        for (var i = 0; i < iterations; i++)
        {
            var res = FluentResults.Result.Ok(10);
            var v = res.Value;
        }
    }

    [Benchmark]
    public void CSharpFunctionalExtensionsOkWrapUnwrapBench()
    {
        for (var i = 0; i < iterations; i++)
        {
            var res = CSharpFunctionalExtensions.Result.Ok(10);
            var v = res.Value;
        }
    }

    [Benchmark]
    public void ResOkWrapUnwrapBench()
    {
        for (var i = 0; i < iterations; i++)
        {
            var res = Res.Res.Ok<int, BenchError>(10);
            var v = res.Unwrap();
        }
    }
}

[MemoryDiagnoser]
[Config(typeof(Config))]
public class ResVsError
{
    private readonly int iterations = 1000;

    private static readonly BenchError error = new BenchError.FailedToBenchError();
    
    [Benchmark]
    public void FluentResultsErrorWrapUnwrapBench()
    {
        for (var i = 0; i < iterations; i++)
        {
            var res = FluentResults.Result.Fail("Some failure");
            var v = res.Reasons;
        }
    }

    [Benchmark]
    public void CSharpFunctionalExtensionsErrorWrapUnwrapBench()
    {
        for (var i = 0; i < iterations; i++)
        {
            var res = CSharpFunctionalExtensions.Result.Failure("I am a failure");
            var v = res.Error;
        }
    }

    [Benchmark]
    public void ResErrorWrapUnwrapBench()
    {
        for (var i = 0; i < iterations; i++)
        {
            var res = Res.Res.Err<int, BenchError>(error);
            var v = res.UnwrapError();
        }
    }
}
