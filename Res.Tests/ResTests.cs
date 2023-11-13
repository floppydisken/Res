using System.Diagnostics;
using System.Resources;
using ExhaustiveMatching;

namespace Res.Tests;

public class TestValue
{
}

[Closed(typeof(InvalidStatePanic), typeof(NullValuePanic), typeof(IoPanic))]
public abstract class ResPanic : Panic
{
    public class InvalidStatePanic : ResPanic {}
    public class NullValuePanic : ResPanic {}
    public class IoPanic : ResPanic {}
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
        IRes<TestValue, ResPanic> SomeOk()
        {
            return global::Res.Res.Ok(new TestValue());
        }

        IRes<TestValue, ResPanic> SomeOk2()
        {
            return new TestValue().ToOk();
        }

        IRes<TestValue, ResPanic> SomeError()
        {
            return Res.Err<TestValue, ResPanic>(new ResPanic.IoPanic());
        }

        IRes<TestValue, ResPanic> SomeError2()
        {
            return new ResPanic.IoPanic().ToErr<TestValue>();
            //return Res.Err<TestValue, ResException>(new ResException.IOException());
        }

        IRes<TestValue, ResPanic> result = SomeError();
        var res2 = SomeError();
        var v = res2.UnwrapOr(new TestValue());

        string res = result switch
        {
            Ok<TestValue, ResPanic> ok => ok.Unwrap().ToString() ?? "",
            Err<TestValue, ResPanic> err => err.Error switch
            {
                ResPanic.IoPanic e => throw e,
                ResPanic.NullValuePanic e => e.Message,
                ResPanic.InvalidStatePanic e => e.Message,
                // ResException e => throw ExhaustiveMatch.Failed(""),
                _ => throw ExhaustiveMatch.Failed(""),
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [Test]
    public void TestNonGenericVersion()
    {
        
        IRes SomeOk()
        {
            return ResNonGeneric.Ok(new TestValue());
        }

        IRes SomeError()
        {
            return ResNonGeneric.Err(new ResPanic.IoPanic());
        }

        IRes result = SomeError();
        var res2 = SomeError();
        var v = res2.UnwrapOr(new TestValue());

        string res = result switch
        {
            OkNonGeneric ok => ok.Unwrap().ToString() ?? "",
            ErrNonGeneric err => (err.Error as ResPanic) switch
            {
                ResPanic.IoPanic e => e.Message,
                ResPanic.NullValuePanic e => e.Message,
                ResPanic.InvalidStatePanic e => e.Message,
                // ResException e => throw ExhaustiveMatch.Failed(""),
                _ => throw ExhaustiveMatch.Failed(""),
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
} 