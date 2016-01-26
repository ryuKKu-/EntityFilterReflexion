using System;
using System.Collections.Generic;

namespace EntityFilterTest.Entities
{
    public class CustomerEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<OrderEntity> Orders { get; set; }

        public CustomerEntity()
        {
            Orders = new List<OrderEntity>();
        }
    }
}
