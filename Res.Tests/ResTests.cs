using System.Diagnostics;

namespace Results.Tests;

public class TestValue
{
}

public class TestValueException : Exception
{
    public StackTrace RootStackTrace { get; private set; } = new(true);

    public class SomeInvalidStateException : TestValueException {}
    public class NullValueException : TestValueException {}
    public class IOException : TestValueException {}
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
        
        IRes<TestValue, TestValueException> SomeOk()
        {
            return Res.Ok<TestValue, TestValueException>(new TestValue());
        }
        
        IRes<TestValue, TestValueException> SomeError()
        {
            return Res.Err<TestValue, TestValueException>(new TestValueException.IOException());
        }

        var result = SomeError();

        var res = result switch
        {
            Ok<TestValue, TestValueException> ok => ok.Unwrap().ToString(),
            Err<TestValue, TestValueException> err => err.Error switch
            {
                TestValueException.IOException => "",
                TestValueException.NullValueException => "",
                TestValueException.SomeInvalidStateException => "",
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
