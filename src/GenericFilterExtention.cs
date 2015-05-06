using System;
using System.Linq;

namespace BackendPartenaire.Extensions
{
    public static class GenericFilterExtention
    {
        /// <summary>
        /// Generic extension method to filter a TSource entity based on reflexion
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="source"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IQueryable<TSource> ConstructQuerySearch<TSource, TModel>(this IQueryable<TSource> source,
            TModel model)
        {
            var type = model.GetType();
            var properties = type.GetProperties();

            foreach (var prop in properties)
            {
                object value = prop.GetValue(model, null);

                if (value != null && !String.IsNullOrWhiteSpace(value.ToString()))
                {
                    var attributes = prop.GetCustomAttributes(false);

                    FilterWhereAttribute attr =
                        (FilterWhereAttribute)
                            attributes.FirstOrDefault(x => x.GetType() == typeof (FilterWhereAttribute));
                    
                    if(attr != null)
                    {
                        string propertyName = (String.IsNullOrEmpty(attr.OnProperty)) ? prop.Name : attr.OnProperty;
                        source = source.WhereReflexion(propertyName, value, attr.Operator);
                    }
                }
            }

            return source;
        }
    }
}