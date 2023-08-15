using System.Diagnostics;

namespace Results.Tests;

public class TestValue
{
}

public class ResException : Exception
{
    public StackTrace RootStackTrace { get; private set; } = new(true);

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
            return Res.Ok<TestValue, ResException>(new TestValue());
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
                ResException.IOException => "",
                ResException.NullValueException => "",
                ResException.InvalidStateException invalidStateException => throw new NotImplementedException(),
                //ResException.InvalidStateException => "",
                //_ => "",
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
