namespace Res;

public interface IError
{
    public string Reason { get; }
}

public class BasicResException<TErr> : Exception
{
    public TErr Error { get; set; }

    public BasicResException(TErr err)
    {
        Error = err;
    }
}


public enum UnwrapType
{
    Err,
    Ok
}

public class Errors
{
    public sealed class CannotUnwrapError : Errors, IError
    {
        public string Reason { get; }

        public CannotUnwrapError(Type resultType, UnwrapType unwrapType)
        {
            Reason = $"Unable to unwrap '{unwrapType}' on type {resultType.Name}";
        }
    }
}


public abstract record BasicRes<T, TErr>
{
    public abstract T Unwrap();
    public abstract T UnwrapOr(T other);
    public abstract TErr UnwrapError();
}

public sealed record BasicOk<T, TErr> : BasicRes<T, TErr> 
{
    private readonly T value;
    public BasicOk(T value)
    {
        this.value = value;
    }
    
    public override T Unwrap()
    {
        return value;
    }

    public override T UnwrapOr(T other)
    {
        return value;
    }

    public override TErr UnwrapError()
    {
        throw new BasicResException<Errors>(new Errors.CannotUnwrapError(typeof(TErr), UnwrapType.Err));
    }
}

public sealed record BasicErr<T, TErr> : BasicRes<T, TErr>
{
    private readonly TErr error;

    public BasicErr(TErr error)
    {
        this.error = error;
    }

    public override T Unwrap()
    {
        throw new BasicResException<TErr>(error);
    }

    public override TErr UnwrapError()
    {
        return error;
    }

    public override T UnwrapOr(T other)
    {
        return other;
    }
}

/// <summary>
/// Contains all of the factory methods for easy DX
/// </summary>
public static class BasicRes
{
    public static BasicRes<T, TErr> Ok<T, TErr>(T value)
    {
        return new BasicOk<T, TErr>(value);
    }
    
    public static BasicRes<T, TErr> Err<T, TErr>(TErr value)
    {
        return new BasicErr<T, TErr>(value);
    }
}
