using System.Diagnostics;
using System.Reflection;
using System.Resources;
using ExhaustiveMatching;
using Microsoft.VisualStudio.TestPlatform.Common.Interfaces;

namespace Res.Tests;

public record TestValue();

public static class Types
{
    public static Type[] GetSubclassesOf(this Type type)
    {
        // Get all types in the assembly of the parent type
        var allTypes = Assembly.GetAssembly(type).GetTypes();

        // Filter out the types that are subclass of the parentType
        var subTypes = allTypes.Where(type.IsSubclassOf);

        return subTypes.ToArray();
    }
}

[Closed]
public abstract record TestError
{
    public sealed record ParseError(string Reason) : TestError, IError;
    public sealed record IoError(string Reason) : TestError, IError;
    public sealed record InvalidStateError(string Reason) : TestError, IError;
}

public enum IoErrorType
{
    IPv4,
    FileSystem,
}

public record ExternalError();
public record Person(string Name);

[Closed(
    typeof(InvalidStateError), 
    typeof(NullValueError), 
    typeof(IoError), 
    typeof(ExternalError)
)]
public abstract record TestResError
{
    public sealed record InvalidStateError(string State) : TestResError;
    public sealed record NullValueError(string NameOfValue) : TestResError;
    public record IoError(IoErrorType Origin) : TestResError;
}

public class ResTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ProperlyResolvesToError()
    {
        Res<TestValue, TestResError> SomeOk()
        {
            return new TestValue().ToOk();
        }

        Res<TestValue, TestResError> SomeOk2()
        {
            return new TestValue().ToOk();
        }

        Res<TestValue, TestResError> SomeError()
        {
            return Res.Err<TestValue, TestResError>(new TestResError.IoError(IoErrorType.FileSystem));
        }

        Res<TestValue, TestResError> SomeError2()
        {
            return Res.Err<TestValue, TestResError>(new TestResError.IoError(IoErrorType.IPv4));
        }

        Res<TestValue, TestResError> Error()
        {
            return new TestResError.IoError(IoErrorType.FileSystem)
                .ToErr<TestResError>();
        }

        Res<TestValue, TestResError> result = Error();
        Res<TestValue, TestResError> res2 = SomeError()
            .MapError<TestResError>((error) =>
                error switch
                {
                    TestResError.InvalidStateError invalidStateError => 
                        new TestResError.InvalidStateError(invalidStateError.State),
                    TestResError.IoError ioError => 
                        new TestResError.IoError(ioError.Origin),
                    TestResError.NullValueError nullValueError => 
                        new TestResError.NullValueError(nameof(error)),
                    _ => throw new UnknownErrorException(nameof(error))
                });

        Res<TestValue, TestResError> ok1 = new TestValue().ToOk();
    
        var v = res2.UnwrapOr(new TestValue());
        var v2 = res2.Map(ok => "The Man");

        string res = result switch
        {
            Ok<TestValue, TestResError> ok => ok.Unwrap().ToString() ?? "",
            Err<TestValue, TestResError> err => err.UnwrapError() switch
            {
                TestResError.IoError e => throw new InvalidOperationException("Failed to do the thing because of wrong thing."),
                TestResError.NullValueError e => e.NameOfValue,
                TestResError.InvalidStateError e => e.State,
                _ => throw ExhaustiveMatch.Failed(),
            },
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Console.WriteLine(res);
    }

    // [Test]
    // public void TestNonGenericVersion()
    // {
    //     
    //     IRes SomeOk()
    //     {
    //         return ResNonGeneric.Ok(new TestValue());
    //     }

    //     IRes SomeError()
    //     {
    //         return ResNonGeneric.Err(new TestResError.IoError());
    //     }

    //     IRes result = SomeError();
    //     var res2 = SomeError();
    //     var v = res2.UnwrapOr(new TestValue());

    //     string res = result switch
    //     {
    //         OkNonGeneric ok => ok.Unwrap().ToString() ?? "",
    //         ErrNonGeneric err => (err.Error as TestResError) switch
    //         {
    //             TestResError.IoError e => e.Message,
    //             TestResError.NullValueError e => e.Message,
    //             TestResError.InvalidStateError e => e.Message,
    //             // ResException e => throw ExhaustiveMatch.Failed(""),
    //             _ => throw ExhaustiveMatch.Failed(""),
    //         },
    //         _ => throw new ArgumentOutOfRangeException()
    //     };
    // }

    [Test]
    public void TestBasicRes()
    {
        var res = BasicRes.Ok<TestValue, TestError>(new TestValue());

        string unwrapped = res switch
        {
            BasicOk<TestValue, TestError> ok => ok.Unwrap().ToString() ?? "",
            BasicErr<TestValue, TestError> err => err.UnwrapError() switch
            {
                TestError.InvalidStateError invalidStateError => "",
                TestError.IoError ioError => "",
                TestError.ParseError parseError => "",
                {} e => throw new UnknownErrorException(e)
            },
            _ => ""
        };
    }

    public enum TestErrorCodes
    {
        CannotUnwrapError,
        IoError,
        UnauthorizedError,
        TimeoutError,
    }

    public record EnumResTestErrors
    {
        public record UnwrapError : EnumResTestErrors, IError<TestErrorCodes>
        {
            public TestErrorCodes Error => TestErrorCodes.CannotUnwrapError;
            public string Reason => "Failed to unwrap the value";
        }

        public record IoError : EnumResTestErrors, IError<TestErrorCodes>
        {
            public TestErrorCodes Error => TestErrorCodes.IoError;
            public string Reason => "Failed to fetch the thing over the thing";
        }
    }

    [Test]
    public void TestEnumRes()
    {
        //var res = EnumRes.Ok<TestValue, EnumResTestErrors>(new TestValue());
        var res = EnumRes.Err<TestValue, Error<TestErrorCodes>>(
            new Error<TestErrorCodes>(TestErrorCodes.CannotUnwrapError, "Failed to unwrap the thing")
        );

        if (res is EnumErr<TestValue, Error<TestErrorCodes>> err)
        {
            var unwrapped = err.UnwrapError();
            var actual = unwrapped.ErrorCode switch
            {
                TestErrorCodes.CannotUnwrapError => throw new Exception("Unrecoverable..."),
                TestErrorCodes.IoError => EnumRes.Ok<TestValue, Error<TestErrorCodes>>(new TestValue()),
                TestErrorCodes.UnauthorizedError => EnumRes.Err<TestValue, Error<TestErrorCodes>>(new (TestErrorCodes.IoError, new { Some = "Value" })),
                TestErrorCodes.TimeoutError => throw new Exception("Unrecoverable..."),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        var theEnum = TestErrorCodes.IoError;
        string r = theEnum switch
        {
            TestErrorCodes.CannotUnwrapError => "",
            //TestErrorCodes.IoError => "",
            //_ => throw new ArgumentOutOfRangeException()
            TestErrorCodes.IoError => "",
            TestErrorCodes.UnauthorizedError => "",
            TestErrorCodes.TimeoutError => "",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
} 