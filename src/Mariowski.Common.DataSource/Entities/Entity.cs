﻿using System;
using System.Collections.Generic;

namespace Mariowski.Common.DataSource.Entities
{
    [Serializable]
    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }

        /// <inheritdoc />
        public virtual bool IsTransient()
        {
            if (EqualityComparer<TPrimaryKey>.Default.Equals(Id, default))
                return true;

            // Workaround for EF Core since it sets int/long to min value when attaching to db context.
            return Id switch
            {
                int id => id <= 0,
                long id => id <= 0,
                _ => false
            };
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns>
        /// True if <paramref name="obj">obj</paramref> is a same instance as this instance;
        /// True if <paramref name="obj">obj</paramref> is a <see cref="T:Entity{TPrimaryKey}"></see> that has the same Id as this instance;
        /// Otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Entity<TPrimaryKey>))
                return false;

            // Same instances must be considered as equal.
            if (ReferenceEquals(this, obj))
                return true;

            // Transient objects are not considered as equal.
            var other = (Entity<TPrimaryKey>)obj;
            if (IsTransient() && other.IsTransient())
                return false;

            // Must have a IS-A relation of types or must be same type.
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.IsAssignableFrom(typeOfOther) && !typeOfOther.IsAssignableFrom(typeOfThis))
                return false;

            return Id.Equals(other.Id);
        }

        /// <summary>
        /// Returns the HashCode of underlying Id.
        /// </summary>
        /// <returns>HashCode of Id.</returns>
        public override int GetHashCode()
            => Id.GetHashCode();

        /// <summary>
        /// Indicates whether the values of two specified <see cref="T:Entity{TPrimaryKey}"></see> objects are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>True if <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are equal; otherwise, false.</returns>
        public static bool operator ==(Entity<TPrimaryKey> left, Entity<TPrimaryKey> right)
            => left?.Equals(right) ?? Equals(right, null);

        /// <summary>
        /// Indicates whether the values of two specified <see cref="T:Entity{TPrimaryKey}"></see> objects are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if <paramref name="left">left</paramref> and <paramref name="right">right</paramref> are not equal; otherwise, false.</returns>
        public static bool operator !=(Entity<TPrimaryKey> left, Entity<TPrimaryKey> right)
            => !(left == right);

        /// <summary>
        /// Returns the string in following format: "[{TypeName} {Id}]".
        /// </summary>
        /// <returns>The string in following format: "[{TypeName} {Id}]".</returns>
        public override string ToString()
            => $"[{GetType().Name} {Id}]";
    }
}