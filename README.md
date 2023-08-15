# Why?

Rust match cases and error handling is amazing and tedious, which is great when you want guarentees for handling error states.

Dotnet does not have required exception handling which tends to lead to a catch-all exception handler for something like APIs or CLI applications.

It doesn't have to be this way however. What if we could somewhat emulate the Result from Rust and monadify C# a little, then use the IDE or LSP to provide us hints?

Alternatives: https://github.com/altmann/FluentResults

This project aims to provide a more developer ergonomic experience using the new C# switch cases. It aims to use the already existing error capabilities of C# to provide an in-depth view of *what went wrong* while still providing easy to handle errors.

The idea is something like this.

Imagine we could define the error states
```csharp
public class ResException : Exception
{
    // This hopefully ensures that we can **ALWAYS** track the root of the problem when debugging.
    public StackTrace RootStackTrace { get; private set; } = new(true);

    public class InvalidStateException : ResException {}
    public class NullValueException : ResException {}
    public class IOException : ResException {}
}
```

Then we could do something like this with a Result

```csharp
var result = someInstance.DoSomethingAndGibResult();

var unwrapped = result switch
{
    Ok<TestValue, ResException> ok => ok.Unwrap().ToString(),
    Err<TestValue, ResException> err => err.Error switch
    {
        ResException.IOException => "I'm so handling this IOException.",
        ResException.NullValueException => "Oh noo, the billion dollar mistake.",
        ResException.SomeInvalidStateException => "Yeesh, you really messed up there chief.",
    },
    _ => throw new PanicAtTheDiscoException()
};
```

NOTE: A little annoying, you have to define the types for the generics. Any suggestions as to how to avoid defining generics would be awesome. And honestly, perhaps we're looking at using Source Generators to solve this. How? I have no clue.

or 
```csharp
var result = someInstance.DoSomethingAndGibResult().Unwrap();
```
Which returns the result if any or throws if Err.

And IDEs like Rider would be all mad about when cases are not handled. Perhaps even the omnisharp lsp. Anyways. The whole idea is to make it painfully obvious that there are missing handled cases.

Think this could be cool. It is yet to be determined whether or not it provides actually readable code, but I'd argue that it definitely defeats something like this.

```csharp
SomeType shouldDefaultOnError;
try
{
    shouldDefaultOnError = SomeMethod();
}
catch (Exception e)
{
    shouldDefaultOnError = SomeType.Default();
}
```

The advantage of this approach is to make it obvious what is missing when writing code.

Also it's now obvious what possible return values there are. Even though we don't get something as great as discriminated unions, polymorphism almost gives us the same guarentees. It is now APART OF THE API which is wonderful.

# Strict mode

Taking inspiration from https://github.com/SteveDunn/Vogen (or https://github.com/SteveDunn/Vogen/blob/main/src/Vogen/CompilationExtensions.cs) we could probably ensure compile time exceptions for unhandled cases. We're returning to the Java-esque forced throw handlers, which is a bit ironic, but man does it make sense when you're building a big project and want guarentees and transparency.
