// using System.Diagnostics;
// using System.Runtime.Serialization;
// using System.Text;
// 
// namespace Res;
// 
// public static class ResNonGeneric
// {
//     public static OkNonGeneric Ok(object value)
//     {
//         return new OkNonGeneric(value);
//     }
//     
//     public static ErrNonGeneric Err(ResError error)
//     {
//         return new ErrNonGeneric(error);
//     }
// }
// 
// public interface IRes
// {
//     public object Unwrap();
//     public object UnwrapOr(object t);
// }
// 
// public class ErrNonGeneric : IRes
// {
//     private readonly ResError error;
//     public ResError Error => error;
// 
//     public ErrNonGeneric(ResError error) => this.error = error;
// 
//     public object Unwrap() => throw this.error;
// 
//     public object UnwrapOr(object or)
//         => or;
// }
// 
// // public class Ok<T, TErr> : Res, IRes<T, TErr> where TErr : Exception
// // {
// //     private readonly T t;
// // 
// //     internal Ok(T t)
// //     {
// //         this.t = t;
// //     }
// // 
// //     public T Unwrap()
// //     {
// //         throw new NotImplementedException();
// //     }
// // }
// 
// public class OkNonGeneric : IRes
// {
//     private readonly object t;
// 
//     internal OkNonGeneric(object t)
//     {
//         this.t = t;
//     }
// 
//     public object Unwrap()
//         => t;
// 
//     public object UnwrapOr(object or)
//         => Unwrap();
// }
