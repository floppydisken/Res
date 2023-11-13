using System.Diagnostics;
using System.Runtime.Serialization;

namespace Res;

public class Panic : Exception
{
    public StackTrace RootStackTrace = new();

    public Panic()
    {
    }

    protected Panic(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public Panic(string? message) : base(message)
    {
    }

    public Panic(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class Error
{
}

public static class Res
{
    public static Ok<T, TError> Ok<T, TError>(T value) where TError : Panic
    {
        return new Ok<T, TError>(value);
    }
    
    public static Ok<T, Panic> Ok<T>(T value)
    {
        return new Ok<T, Panic>(value);
    }

    public static Ok<T, Panic> ToOk<T>(this T value)
    {
        return new Ok<T, Panic>(value);
    }
    
    public static Err<T, TError> Err<T, TError>(TError error) where TError : Panic
    {
        return new Err<T, TError>(error);
    }

    public static Err<T, Panic> ToErr<T>(this Panic error)
    {
        return new Err<T, Panic>(error);
    }
    
    // public static Err<T, TError> Err<T, TError>(TError error) where TError : Exception
    // {
    //     return new Err<T, TError>(error);
    // }
}

public interface IRes<T, in TErr> where TErr : Exception
{
    public T Unwrap();
    public T UnwrapOr(T t);
}

public class Err<T, TErr> : IRes<T, TErr> where TErr : Exception
{
    private readonly TErr error;
    public TErr Error => error;

    public Err(TErr error) => this.error = error;

    public T Unwrap() => throw this.error;

    public T UnwrapOr(T or)
        => or;
}

// public class Ok<T, TErr> : Res, IRes<T, TErr> where TErr : Exception
// {
//     private readonly T t;
// 
//     internal Ok(T t)
//     {
//         this.t = t;
//     }
// 
//     public T Unwrap()
//     {
//         throw new NotImplementedException();
//     }
// }

public class Ok<T, TError> : IRes<T, TError> where TError : Exception
{
    private readonly T t;

    internal Ok(T t)
    {
        this.t = t;
    }

    public T Unwrap()
    {
        return t;
    }

    public T UnwrapOr(T or)
        => t is null ? or : t;
}