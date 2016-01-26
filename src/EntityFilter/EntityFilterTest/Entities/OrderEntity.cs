using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFilterTest.Entities
{
    public class OrderEntity
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public double Price { get; set; }

        public int Status { get; set; }

        [ForeignKey("CustomerEntity")]
        public int CustomerId { get; set; }

        public virtual CustomerEntity Customer { get; set; }

        //public virtual ICollection<>  {get;set;} 
    }
}
