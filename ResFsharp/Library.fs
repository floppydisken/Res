namespace ResFsharp

open FSharp.Core

type TestError =
    | IoError of string
    | PersistenceError of int
    | ParseError of string
    
type TestValue =
    { Name: string }

module Say =
    let v = Ok({ Name = "Hello buddy" })
    let er = Error(IoError("something"))
             |> Result.bind (fun (n) -> Ok({ Name = $"asdf {n}" }))
             |> Result.bind (fun (n) -> Ok({ Name = $"{n} asdf" }))
    
    let res =
      match er with
      | Error _er ->
          match _er with
          | IoError ioErr -> ioErr
          | PersistenceError netError -> ""
          | ParseError s -> failwith "todo"
      | Ok v -> v.Name
         
    let hello name =
        printfn "Hello %s" name