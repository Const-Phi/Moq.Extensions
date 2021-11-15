namespace Moq.Extensions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Moq;

    public abstract class BaseTestDataBuilder<TObject, TBuilder>
        where TObject : class
        where TBuilder : BaseTestDataBuilder<TObject, TBuilder>
    {
        private readonly ParameterExpression target = Expression.Parameter(typeof(TObject));

        private BinaryExpression setup;

        protected TBuilder RegisterValueForProperty<TValue>(Expression<Func<TObject, TValue>> expression, TValue value)
        {
            if (!((expression.Body as MemberExpression)?.Member is PropertyInfo targetProperty))
            {
                throw new ArgumentOutOfRangeException(nameof(expression), "Expression don't extract property of target type.");
            }

            var result = Expression.Equal(
                Expression.Property(this.target, targetProperty),
                Expression.Constant(value));

            return this.UpdateSetup(result);
        }

        protected TBuilder UpdateSetup(BinaryExpression expression)
        {
            this.setup = this.setup == null
                ? expression
                : Expression.AndAlso(this.setup, expression);

            return (TBuilder)this;
        }

        public TObject Build()
        {
            return Mock.Of(Expression.Lambda<Func<TObject, bool>>(this.setup, this.target));
        }
    }
}
