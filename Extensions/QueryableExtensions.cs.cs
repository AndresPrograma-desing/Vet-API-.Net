using System;
using System.Linq;
using System.Linq.Expressions;

//Nueva ramma llamada "funcion agregada para la busqueda sin mayusculas ni minusculas". 
namespace vet_api_Net.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Filtra un IQueryable comparando una columna de texto de forma insensible a mayúsculas/minúsculas.
    /// </summary>
    public static IQueryable<T> WhereEqualsIgnoreCase<T>(
        this IQueryable<T> source,
        Expression<Func<T, string?>> propertySelector,
        string? value)
    {
        if (value == null)
        {
            var isNullExpression = Expression.Equal(propertySelector.Body, Expression.Constant(null, typeof(string)));
            var lambda = Expression.Lambda<Func<T, bool>>(isNullExpression, propertySelector.Parameters);
            return source.Where(lambda);
        }

        var memberExpression = propertySelector.Body;

        var notNullExpression = Expression.NotEqual(memberExpression, Expression.Constant(null, typeof(string)));

        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
        var toLowerExpression = Expression.Call(memberExpression, toLowerMethod);

        var valueLower = value.ToLower();
        var constantExpression = Expression.Constant(valueLower, typeof(string));

        var equalityExpression = Expression.Equal(toLowerExpression, constantExpression);

        var andExpression = Expression.AndAlso(notNullExpression, equalityExpression);

        var finalLambda = Expression.Lambda<Func<T, bool>>(andExpression, propertySelector.Parameters);

        return source.Where(finalLambda);
    }
}
