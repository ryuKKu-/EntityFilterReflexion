using System.Linq;

namespace EntityFilter
{
    public static class EntityFilterExtention
    {
        /// <summary>
        /// Extension method to filter a IQueryable TSource entity with TModel values using reflection
        /// <para>In order to filter with TModel's values, you must apply the <see cref="EntityFilterAttribute"/> on each desired property </para>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="source"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IQueryable<TSource> FilterFromModel<TSource, TModel>(this IQueryable<TSource> source,
            TModel model)
        {
            var type = model.GetType();
            var properties = type.GetProperties();

            foreach (var prop in properties)
            {
                object value = prop.GetValue(model, null);

                if (!string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    var attributes = prop.GetCustomAttributes(false);

                    EntityFilterAttribute attr =
                        (EntityFilterAttribute)
                            attributes.FirstOrDefault(x => x.GetType() == typeof (EntityFilterAttribute));

                    if (attr != null)
                    {
                        string propertyName = (string.IsNullOrEmpty(attr.OnProperty)) ? prop.Name : attr.OnProperty;
                        source = source.WhereExpressionBuilder(propertyName, value, model, attr.Operator);
                    }
                }
            }

            return source;
        }
    }
}
