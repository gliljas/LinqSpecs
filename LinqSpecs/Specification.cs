﻿using System;
using System.Linq.Expressions;

namespace LinqSpecs
{
    /// <summary>
    /// Base class for query specifications that can be combined
    /// using logical AND, OR and NOT operators.
    /// </summary>
    [Serializable]
    public abstract class Specification<T>
    {
        /// <summary>
        /// Returns an expression that defines this query.
        /// </summary>
        /// <remarks>
        /// Typically calling this method is not needed as the query
        /// specification can be converted implicitly to an expression
        /// by just assigning it or passing it as such to another method.
        /// </remarks>
        public abstract Expression<Func<T, bool>> ToExpression();

        /// <summary>
        /// Performs an implicit conversion from <see cref="Specification{T}"/>
        /// to a LINQ expression.
        /// </summary>
        public static implicit operator Expression<Func<T, bool>>(Specification<T> spec)
        {
            return spec?.ToExpression() ?? throw new ArgumentNullException(nameof(spec));
        }

        /// <summary>
        /// Override operator false for supporting &amp;&amp; and || operations
        /// </summary>
        public static bool operator false(Specification<T> spec)
        {
            return false;
        }

        /// <summary>
        /// Override operator true for supporting &amp;&amp; and || operations
        /// </summary>
        public static bool operator true(Specification<T> spec)
        {
            return false;
        }

        /// <summary>
        /// Combines two query specifications using a logical AND operation.
        /// </summary>
        public static Specification<T> operator &(Specification<T> spec1, Specification<T> spec2)
        {
            return new AndSpecification<T>(spec1, spec2);
        }

        /// <summary>
        /// Combines a query specification with a lambda predicate using a logical AND operation.
        /// </summary>
        public static Specification<T> operator &(Specification<T> spec, Expression<Func<T,bool>> predicate)
        {
            return new AndSpecification<T>(spec, new AdHocSpecification<T>(predicate));
        }

        /// <summary>
        /// Combines a query specification with a lambda predicate using a logical AND operation.
        /// </summary>
        public static Specification<T> operator &(Expression<Func<T, bool>> predicate, Specification<T> spec)
        {
            return new AndSpecification<T>(new AdHocSpecification<T>(predicate), spec);
        }

        /// <summary>
        /// Combines two query specifications using a logical OR operation.
        /// </summary>
        public static Specification<T> operator |(Specification<T> spec1, Specification<T> spec2)
        {
            return new OrSpecification<T>(spec1, spec2);
        }

        /// <summary>
        /// Combines a query specification with a lambda predicate using a logical OR operation.
        /// </summary>
        public static Specification<T> operator |(Specification<T> spec, Expression<Func<T, bool>> predicate)
        {
            return new OrSpecification<T>(spec, new AdHocSpecification<T>(predicate));
        }

        /// <summary>
        /// Combines a query specification with a lambda predicate using a logical OR operation.
        /// </summary>
        public static Specification<T> operator |(Expression<Func<T, bool>> predicate, Specification<T> spec)
        {
            return new OrSpecification<T>(new AdHocSpecification<T>(predicate), spec);
        }

        /// <summary>
        /// Negates the given specification.
        /// </summary>
        public static Specification<T> operator !(Specification<T> spec)
        {
            return new NotSpecification<T>(spec);
        }
    }
}
