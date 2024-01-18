using BNS360.Core.Entities;
using BNS360.Core.Services.Shared;
using System.Linq.Expressions;
using System.Reflection;
namespace BNS360.Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>>? Criteria { get; set; }

        public List<Expression<Func<T, object>>>? NavigationProperties
            => IncludeNavigationProps ? ExploreNavigationPros() : default;
        protected bool IncludeNavigationProps { private get; set; }

        private List<Expression<Func<T, object>>>? ExploreNavigationPros()
        {
            var baseEntity = typeof(BaseEntity);

            var proprties = typeof(T).GetProperties().Where(p => p.PropertyType.BaseType == baseEntity)
                .Select(p => GetExpression(p)).ToList();

            return proprties;
        }

        private Expression<Func<T, object>> GetExpression(PropertyInfo propertyInfo)
        {
            var paramter = Expression.Parameter(typeof(T), typeof(T).Name);
            var propertyAccess = Expression.Property(paramter, propertyInfo);
            var lamda = Expression.Lambda<Func<T, object>>(propertyAccess, paramter);
            return lamda;
        }

    }
}
