using System;

namespace BackendPartenaire.Extensions
{
    /// <summary>
    /// Specifies the type of operation to apply on the property of an entity when using the GenericFilterExtention class
    /// </summary>
    public class FilterWhereAttribute : Attribute
    {
        public OperatorType Operator { get; set; }
        
        /// <summary>
        /// The entity's property name to target if the model's property name is different
        /// </summary>
        public string OnProperty { get; set; }
    }

    /// <summary>
    /// Represent an enumeration of operation type associated to the FilterWhereAttribute class
    /// </summary>
    public enum OperatorType
    {
        /// <summary>
        /// Represent an Equal expression
        /// </summary>
        Equal,

        /// <summary>
        /// Represent an Equal expression on the property DateTime.Date
        /// </summary>
        EqualDate,

        /// <summary>
        /// Represent a GreaterThanOrEqual expression
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Represent a LessThanOrEqual expression
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Represent a Contains expression
        /// </summary>
        Contains
    }
}