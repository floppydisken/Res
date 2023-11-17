using System.Diagnostics;
using System.Runtime.Serialization;

namespace Res;


public class UnwrapException : Exception
{
}

public class UnknownErrorException : Exception
{
    private readonly object _error;

    public UnknownErrorException(object error)
        : base($"Failed to handle error with value '${error}'")
    {
        _error = error;
    }
}

public static class Res
{
    public static Ok<T, TError> Ok<T, TError>(T value)
        => new(value);
 
    public static Ok<T, object> ToOk<T>(this T value)
        => Ok<T, object>(value);

    public static Err<T, TError> Err<T, TError>(TError error)
        => new (error);

    /// <summary>
    /// Be advised. Remember to pass in the generic if the type in the
    /// final Res does not match directly or be pained by the compiler errors.
    /// </summary>
    /// <param name="error"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Err<object, TError> ToErr<TError>(this TError error)
         => Err<object, TError>(error);
}

public abstract class Res<T, TErr>
{
    public abstract bool IsError { get; }
    public abstract bool IsOk { get; }
    public abstract T Unwrap();
    public abstract T UnwrapOr(T t);
    public abstract TErr UnwrapError();
    public abstract Res<T, RErr> MapError<RErr>(Func<TErr, RErr> mapper);
    public abstract Res<R, TErr> Map<R>(Func<T, R> mapper);
    public abstract Res<R, TErr> Bind<R>(Func<Res<T, TErr>, Res<R, TErr>> binder);

    public static implicit operator Res<T, TErr>(Ok<T, object> r) 
        => Res.Ok<T, TErr>(r.Unwrap());

    public static implicit operator Res<T, TErr>(Err<object, TErr> r)
        => Res.Err<T, TErr>(r.UnwrapError());
}

public sealed class Err<T, TErr> : Res<T, TErr>
{
    private readonly TErr error;
    public override bool IsError => true;
    public override bool IsOk => false;

    public Err(TErr error) => this.error = error;
    
    public override T Unwrap() => throw new UnwrapException();

    public override T UnwrapOr(T or)
        => or;

    public override TErr UnwrapError()
        => error;

    public override Res<T, RErr> MapError<RErr>(Func<TErr, RErr> mapper)
        => Res.Err<T, RErr>(mapper(error));

    public override Res<R, TErr> Map<R>(Func<T, R> mapper)
        => Res.Err<R, TErr>(error);

    public override Res<R, TErr> Bind<R>(Func<Res<T, TErr>, Res<R, TErr>> binder)
        => Res.Err<R, TErr>(error);
}

public sealed class Ok<T, TError> : Res<T, TError>
{
    public override bool IsError => false;
    public override bool IsOk => true;
 
    private readonly T t;

    internal Ok(T t) => this.t = t;
    
    public override T Unwrap() => t;

    public override T UnwrapOr(T or) => t is null ? or : t;

    public override TError UnwrapError() => throw new UnwrapException();

    public override Res<T, RErr> MapError<RErr>(Func<TError, RErr> mapper)
        => t.ToOk();

    public override Res<R, TError> Map<R>(Func<T, R> mapper)
        => mapper(t).ToOk();

    public override Res<R, TError> Bind<R>(Func<Res<T, TError>, Res<R, TError>> binder)
        => binder(this);
}
