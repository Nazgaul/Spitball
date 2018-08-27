using System;
using System.Linq.Expressions;

namespace Cloudents.Core.Extension
{
    public static class ExpressionExtensions
    {
        public static string GetName<TSource, TField>(this Expression<Func<TSource, TField>> field)
        {
            if (Equals(field, null))
            {
                throw new NullReferenceException("Field is required");
            }

            MemberExpression expr;

            if (field.Body is MemberExpression expression)
            {
                expr = expression;
            }
            else if (field.Body is UnaryExpression unaryExpression)
            {
                expr = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                const string format = "Expression '{0}' not supported.";
                string message = string.Format(format, field);

                throw new ArgumentException(message, "field");
            }

            return expr.Member.Name;
        }
    }
}