// using BenchmarkDotNet.Attributes;
// using ExhaustiveMatching;
// using Res.Tests;
// 
// namespace Res.Benchmarks;
// 
// public static class ResultService
// {
//     public static Res<TestValue, TestResError> Result()
//     {
//          // Do something
//          return Res.Ok<TestValue, TestResError>(new TestValue());
//     }
// 
//     public static Res<TestValue, TestResError> Error()
//     {
//         return Res.Err<TestValue, TestResError>(new TestResError.IoError(IoErrorType.FileSystem));
//     }
// 
//     public static string Throws()
//     {
//         throw new Exception("Doesnt matter what I am");
//     }
// }
// 
// [ShortRunJob]
// [MemoryDiagnoser]
// public class ResWithExceptionBenchmarks
// {
//     [Benchmark]
//     public string RegularTryCatch()
//     {
//         try
//         {
//             var r = ResultService.Throws();
//             return r;
//         }
//         catch (Exception e)
//         {
//             return e.Message;
//         }
//     }
// 
//     [Benchmark]
//     public string ResultOnly()
//     {
//          var result = ResultService.Result();
//      
//          string res = result switch
//          {
//              Ok<TestValue, TestResError> ok => ok.Unwrap().ToString() ?? "",
//              Err<TestValue, TestResError> err => err.UnwrapError() switch
//              {
//                  TestResError.IoError e => e.ToString(),
//                  TestResError.NullValueError e => e.ToString(),
//                  TestResError.InvalidStateError e => e.ToString(),
//                  _ => throw ExhaustiveMatch.Failed(""),
//              },
//              _ => throw new ArgumentOutOfRangeException()
//          };
// 
//          return res;
//     }
// 
//     [Benchmark]
//     public string ResultAndThrows()
//     {
//          var result = ResultService.Error();
// 
//          string res = result switch
//          {
//              Ok<TestValue, TestResError> ok => ok.Unwrap().ToString() ?? "",
//              Err<TestValue, TestResError> err => err.UnwrapError() switch
//              {
//                  TestResError.IoError e => throw new Exception(),
//                  TestResError.NullValueError e => e.ToString(),
//                  TestResError.InvalidStateError e => e.ToString(),
//                  _ => throw ExhaustiveMatch.Failed(""),
//              },
//              _ => throw new ArgumentOutOfRangeException()
//          };
// 
//          return res;
//     }
// 
//     [Benchmark]
//     public TestValue ResultErrorButResolves()
//     {
//         var result = ResultService.Error();
//         var r = result.UnwrapOr(new TestValue());
// 
//         return r;
//     }
// }