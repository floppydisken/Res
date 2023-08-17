using System.Diagnostics;
using System.Runtime.Serialization;

namespace Res;

public class NullValueException : Exception
{
    public NullValueException(Type t) : base($"Result with {t.Name} is null") { }
}

public class PanicException : Exception
{
    public StackTrace RootStackTrace = new();

    public PanicException()
    {
    }

    protected PanicException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public PanicException(string? message) : base(message)
    {
    }

    public PanicException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

public class Res
{
    public static Ok<T, TError> Ok<T, TError>(T value) where TError : PanicException
    {
        return new Ok<T, TError>(value);
    }
    
    public static Ok<T, Exception> Ok<T>(T value)
    {
        return new Ok<T, Exception>(value);
    }
    
    public static Err<T, TError> Err<T, TError>(TError error) where TError : PanicException
    {
        return new Err<T, TError>(error);
    }
    
    // public static Err<T, TError> Err<T, TError>(TError error) where TError : Exception
    // {
    //     return new Err<T, TError>(error);
    // }
}

public interface IRes<out T, in TErr> where TErr : Exception
{
    public T Unwrap();
}

public class Err<T, TErr> : Res, IRes<T, TErr> where TErr : Exception
{
    private readonly TErr error;
    public TErr Error => error;

    public Err(TErr error) => this.error = error;

    public T Unwrap() => throw this.error;
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

public class Ok<T, TError> : Res, IRes<T, TError> where TError : Exception
{
    private readonly T t;

    internal Ok(T t)
    {
        this.t = t;
    }

    public T Unwrap()
    {
        throw new NotImplementedException();
    }
}