using System.Linq;

namespace BackendPartenaire.Extensions
{
    public static class Pagination
    {
        /// <summary>
        /// Méthode d'extension générique pour appliquer une pagination sur un ensemble de données 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="pageSize">Nombre d'éléments par page</param>
        /// <param name="page">Numéro de page</param>
        /// <returns></returns>
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> list, int pageSize, int page)
        {
            return list.Skip(pageSize * (page - 1)).Take(pageSize);
        }
    }
}