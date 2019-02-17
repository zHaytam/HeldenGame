using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Helden.Common.Utils.Extensions
{
    public static class ReflectionExtensions
    {

        public static T CreateDelegate<T>(this ConstructorInfo ctor)
        {
            var parameters = ctor.GetParameters().Select(param => Expression.Parameter(param.ParameterType)).ToList();
            var lamba = Expression.Lambda<T>(Expression.New(ctor, parameters), parameters);
            return lamba.Compile();
        }

    }
}
