namespace Res;

public abstract class BasicRes<T, TErr>
{
    public abstract T Unwrap();
    public abstract T UnwrapOr(T or);
}

public class BasicOk<T, TErr>
{
}

public class BasicErr<T, TErr>
{
}

/// <summary>
/// Contains all of the factory methods for easy DX
/// </summary>
public static class BasicRes
{
}