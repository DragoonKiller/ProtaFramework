using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Prota
{
    public static class HandleErrorTaskExt
    {
        public static T HandleError<T>(this T a) where T: Delegate
        {
            if(a.Method.ReturnType == typeof(void))
                throw new Exception("HandleError cannot access exception through void return value.");
            
            var invokeMethod = a.GetType().GetMethod("Invoke");
            var parameters = a.Method.GetParameters().Select(p => Expression.Parameter(p.ParameterType)).ToArray();
            var writeLineMethod = typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string) });
            var ex = Expression.Parameter(typeof(Exception));
            var tryCatchBlock = Expression.TryCatch(
                Expression.Block(
                    Expression.Call(Expression.Constant(a), invokeMethod, parameters)
                ),
                Expression.Catch(ex,
                    Expression.Block(
                        Expression.Call(writeLineMethod, Expression.Call(ex, nameof(ToString), Type.EmptyTypes))
                    )
                )
            );
            var lambda = Expression.Lambda<T>(tryCatchBlock, parameters);
            return lambda.Compile();
        }
    }
}
