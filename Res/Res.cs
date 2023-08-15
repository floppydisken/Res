namespace Results;

public class NullValueException : Exception
{
    public NullValueException(Type t) : base($"Result with {t.Name} is null") { }
}

public class Res
{
    public static Ok<T, TError> Ok<T, TError>(T value) where TError : Exception
    {
        return new Ok<T, TError>(value);
    }
    
    public static Err<T, TError> Err<T, TError>(TError error) where TError : Exception
    {
        return new Err<T, TError>(error);
    }
}

public interface IRes<T, TErr> where TErr : Exception
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