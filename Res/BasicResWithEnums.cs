namespace Res;

public interface IError<out TError> where TError : Enum
{
    public TError Error { get; }
    public string Reason { get; } // could be any object really
}

public record Error<TErrorCode>(TErrorCode ErrorCode, object ErrorValue);

public class EnumResException<TErr> : Exception
{
    public TErr Error { get; set; }

    public EnumResException(TErr err)
    {
        Error = err;
    }
}

public enum EnumResErrors
{
    CannotUnwrap
}

public class EnumErrors
{
    public sealed class CannotUnwrapError : Errors, IError<EnumResErrors>
    {
        public string Reason { get; }
        public EnumResErrors Error { get; } = EnumResErrors.CannotUnwrap;

        public CannotUnwrapError(Type resultType, UnwrapType unwrapType)
        {
            Reason = $"Unable to unwrap '{unwrapType}' on type {resultType.Name}";
        }
    }
}


public abstract record EnumRes<T, TErr>
{
    public abstract T Unwrap();
    public abstract T UnwrapOr(T other);
    public abstract TErr UnwrapError();
}

public sealed record EnumOk<T, TErr> : EnumRes<T, TErr> 
{
    private readonly T value;
    public EnumOk(T value)
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

public sealed record EnumErr<T, TErr> : BasicRes<T, TErr>
{
    private readonly TErr error;

    public EnumErr(TErr error)
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
public static class EnumRes
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
