namespace Moq.Extensions
{
    using System;
    using System.Linq;
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
                throw new ArgumentOutOfRangeException(nameof(expression), "Expression doesn't extract property of target type.");
            }

            BinaryExpression result;

            if (targetProperty.PropertyType == value.GetType())
            {
                result = Expression.Equal(
                    Expression.Property(this.target, targetProperty),
                    Expression.Constant(value));
            }
            else if (IsNullableWrapper(targetProperty, value) || IsHeir(targetProperty, value))
            {
                result = Expression.Equal(
                    Expression.Property(this.target, targetProperty),
                    Expression.Convert(Expression.Constant(value), targetProperty.PropertyType));
            }
            else
            {
                throw new NotImplementedException();
            }

            return this.UpdateSetup(result);
        }

        protected TBuilder RegisterFlag(Expression<Func<TObject, bool>> expression)
        {
            return this.RegisterValueForProperty(expression, true);
        }

        protected TBuilder UpdateSetup(BinaryExpression expression)
        {
            this.setup = this.IsDefaultSetup()
                ? expression
                : Expression.AndAlso(this.setup, expression);

            return (TBuilder)this;
        }

        protected bool IsDefaultSetup()
        {
            return this.setup is null;
        }

        protected bool IsNullableWrapper<TValue>(PropertyInfo targetProperty, TValue value)
        {
            var targetPropertyType = targetProperty.PropertyType;

            return targetPropertyType.IsGenericType
                && targetPropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                && Nullable.GetUnderlyingType(targetPropertyType) == value.GetType();
        }

        protected bool IsHeir<TValue>(PropertyInfo targetProperty, TValue value)
        {
            return targetProperty.PropertyType.IsAssignableFrom(value.GetType());
        }

        protected bool IsImplementer<TValue>(PropertyInfo targetProperty, TValue value)
        {
            var targetPropertyType = targetProperty.PropertyType;

            return targetPropertyType.IsInterface && value.GetType().GetInterfaces().Contains(targetPropertyType);
        }

        public TObject Build()
        {
            return this.IsDefaultSetup()
                ? Mock.Of<TObject>()
                : Mock.Of<TObject>(Expression.Lambda<Func<TObject, bool>>(this.setup, this.target));
        }
    }
}
