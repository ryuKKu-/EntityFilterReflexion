using System;
using System.ComponentModel.DataAnnotations;
using EntityFilter;

namespace EntityFilterTest
{
    /// <summary>
    /// Example model to filter a collection of OrderEntity's objects
    /// </summary>
    public class OrderFilterExampleModel
    {
        [EntityFilter(Operator = OperatorType.GREATER_THAN, OnProperty = "Date")]
        // Will compile to orders.Where(x => x.Date > model.MinDate)
        public DateTime? MinDate { get; set; }

        [EntityFilter(Operator = OperatorType.LESS_THAN, OnProperty = "Date")]
        // Will compile to orders.Where(x => x.Date < model.MaxDate)
        public DateTime? MaxDate { get; set; }

        [EntityFilter(Operator = OperatorType.CONTAINS)]
        // Will compile to orders.Where(x => x.Description != null && x.Description.Contains(model.Description))
        public string Description { get; set; }

        [EntityFilter(Operator = OperatorType.EQUAL)]
        // Will compile to orders.Where(x => x.Status == model.Status)
        public int? Status { get; set; }

        [EntityFilter(Operator = OperatorType.GREATER_THAN_OR_EQUAL, OnProperty = "Price")]
        // Will compile to orders.Where(x => x.Price >= model.MinPrice)
        public double? MinPrice { get; set; }

        [EntityFilter(Operator = OperatorType.LESS_THAN_OR_EQUAL, OnProperty = "Price")]
        // Will compile to orders.Where(x => x.Price <= model.MaxPrice)
        public double? MaxPrice { get; set; }

        [EntityFilter(Operator = OperatorType.EQUAL, OnProperty = "Customer.Id")]
        // Will compile to orders.Where(x => x.Customer.Id == model.CustomerId)
        public int? CustomerId { get; set; }

        public string OrderBy { get; set; }

        public string Way { get; set; }

        public OrderFilterExampleModel()
        {
            OrderBy = "Id";
            Way = "asc";
        }

    }
}
