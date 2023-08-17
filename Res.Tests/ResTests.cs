using System.Diagnostics;
using ExhaustiveMatching;

namespace Res.Tests;

public class TestValue
{
}

[Closed(typeof(InvalidStateException), typeof(NullValueException), typeof(IOException))]
public abstract class ResException : PanicException
{
    public class InvalidStateException : ResException {}
    public class NullValueException : ResException {}
    public class IOException : ResException {}
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
        IRes<TestValue, ResException> SomeOk()
        {
            return Res.Ok(new TestValue());
        }

        IRes<TestValue, ResException> SomeError()
        {
            return Res.Err<TestValue, ResException>(new ResException.IOException());
        }

        IRes<TestValue, ResException> result = SomeError();

        string res = result switch
        {
            Ok<TestValue, ResException> ok => ok.Unwrap().ToString() ?? "",
            Err<TestValue, ResException> err => err.Error switch
            {
                ResException.IOException e => e.Message,
                ResException.NullValueException e => e.Message,
                //ResException.InvalidStateException e => e.Message,
                // ResException e => throw ExhaustiveMatch.Failed(""),
                _ => throw ExhaustiveMatch.Failed(""),
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
} 