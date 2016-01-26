using System;

namespace EntityFilter
{
    /// <summary>
    /// Specifies the type of operation to apply on the property of an entity when using the <see cref="EntityFilterExtention"/> class
    /// <para>Example : [EntityFilter(Operator = OperatorType.Equal, OnProperty = "Foo")]</para>
    /// <para>Support deep linked properties. Example [EntityFilter(Operator = OperatorType.Equal, OnProperty = "Bar.Baz")]</para>
    /// <para>Support collection properties. Example [EntityFilter(Operator = OperatorType.Equal, OnProperty = "[Foos | First | Bar == ModelBar].Baz")]</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityFilterAttribute : Attribute
    {
        public OperatorType Operator { get; set; }

        /// <summary>
        /// The entity's property name to target if the model's property name is different
        /// </summary>
        public string OnProperty { get; set; }
    }

    /// <summary>
    /// Enumeration of possible operation to compare the entity's property value and the model's one
    /// </summary>
    public enum OperatorType
    {
        /// <summary>
        /// Represent an Equal expression
        /// </summary>
        EQUAL,

        /// <summary>
        /// Represent a NotEqual expression
        /// </summary>
        NOT_EQUAL,

        /// <summary>
        /// Represent a GreaterThan expression
        /// </summary>
        GREATER_THAN,

        /// <summary>
        /// Represent a LessThan expression
        /// </summary>
        LESS_THAN,

        /// <summary>
        /// Represent a GreaterThanOrEqual expression
        /// </summary>
        GREATER_THAN_OR_EQUAL,

        /// <summary>
        /// Represent a LessThanOrEqual expression
        /// </summary>
        LESS_THAN_OR_EQUAL,

        /// <summary>
        /// Represent a Contains expression
        /// </summary>
        CONTAINS
    }
}
