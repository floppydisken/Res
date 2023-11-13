using BenchmarkDotNet.Attributes;
using ExhaustiveMatching;
using Res.Tests;

namespace Res.Benchmarks;

public static class ResultService
{
    public static IRes<TestValue, ResPanic> Result()
    {
         // Do something
         return Res.Ok<TestValue, ResPanic>(new TestValue());
    }

    public static IRes<TestValue, ResPanic> Error()
    {
        return Res.Err<TestValue, ResPanic>(new ResPanic.IoPanic());
    }

    public static string Throws()
    {
        throw new Exception("Doesnt matter what I am");
    }
}

[ShortRunJob]
[MemoryDiagnoser]
public class Benchmarks
{
    [Benchmark]
    public string RegularTryCatch()
    {
        try
        {
            var r = ResultService.Throws();
            return r;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    [Benchmark]
    public string ResultOnly()
    {
         var result = ResultService.Result();
     
         string res = result switch
         {
             Ok<TestValue, ResPanic> ok => ok.Unwrap().ToString() ?? "",
             Err<TestValue, ResPanic> err => err.Error switch
             {
                 ResPanic.IoPanic e => e.Message,
                 ResPanic.NullValuePanic e => e.Message,
                 ResPanic.InvalidStatePanic e => e.Message,
                 _ => throw ExhaustiveMatch.Failed(""),
             },
             _ => throw new ArgumentOutOfRangeException()
         };

         return res;
    }

    [Benchmark]
    public string ResultAndThrows()
    {
         var result = ResultService.Error();

         string res = result switch
         {
             Ok<TestValue, ResPanic> ok => ok.Unwrap().ToString() ?? "",
             Err<TestValue, ResPanic> err => err.Error switch
             {
                 ResPanic.IoPanic e => throw e,
                 ResPanic.NullValuePanic e => e.Message,
                 ResPanic.InvalidStatePanic e => e.Message,
                 _ => throw ExhaustiveMatch.Failed(""),
             },
             _ => throw new ArgumentOutOfRangeException()
         };

         return res;
    }

    [Benchmark]
    public TestValue ResultErrorButResolves()
    {
        var result = ResultService.Error();
        var r = result.UnwrapOr(new TestValue());

        return r;
    }
}