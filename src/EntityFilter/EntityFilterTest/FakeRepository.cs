using System;
using System.Collections.Generic;
using EntityFilterTest.Entities;

namespace EntityFilterTest
{
    public class FakeRepository
    {
        public List<CustomerEntity> Customers { get; set; } 
        public List<OrderEntity> Orders { get; set; }

        public FakeRepository()
        {
            Customers = new List<CustomerEntity>();
            Orders = new List<OrderEntity>();

            Customers.Add(new CustomerEntity
            {
                Id = 1,
                Name = "Louis Green",
                DateOfBirth = new DateTime(1975, 1, 1),
            });

            Customers.Add(new CustomerEntity
            {
                Id = 2,
                Name = "Harry Wilkinson",
                DateOfBirth = new DateTime(1980, 12, 1),
            });

            Customers.Add(new CustomerEntity
            {
                Id = 3,
                Name = "Harvey Butler",
                DateOfBirth = new DateTime(1976, 5, 14),
            });

            Customers.Add(new CustomerEntity
            {
                Id = 4,
                Name = "Payton Banks",
                DateOfBirth = new DateTime(1990, 12, 28),
            });

            Customers.Add(new CustomerEntity
            {
                Id = 5,
                Name = "Brayan Navarro",
                DateOfBirth = new DateTime(1955, 5, 6),
            });

            Orders.AddRange(new List<OrderEntity>
            {
                new OrderEntity
                {
                    Id = 1,
                    Date = new DateTime(2015, 12, 12),
                    Description = "Lorem Ipsum",
                    Price = 10,
                    Status = 1,
                    CustomerId = 1,
                    Customer = Customers[0]
                },
                new OrderEntity
                {
                    Id = 2,
                    Date = new DateTime(2015, 12, 20),
                    Description = "Praesent est ipsum",
                    Price = 20,
                    Status = 0,
                    CustomerId = 1,
                    Customer = Customers[0]
                },
                new OrderEntity
                {
                    Id = 3,
                    Date = new DateTime(2014, 1, 20),
                    Description = "Donec cursus sagittis",
                    Price = 2,
                    Status = 2,
                    CustomerId = 2,
                    Customer = Customers[1]
                },
                new OrderEntity
                {
                    Id = 4,
                    Date = new DateTime(2016, 1, 2),
                    Description = "Fusce sollicitudin dolor",
                    Price = 15,
                    Status = 2,
                    CustomerId = 3,
                    Customer = Customers[2]
                },
                new OrderEntity
                {
                    Id = 5,
                    Date = new DateTime(2015, 5, 14),
                    Description = "Nam accumsan efficitur",
                    Price = 13,
                    Status = 1,
                    CustomerId = 3,
                    Customer = Customers[2]
                },
                new OrderEntity
                {
                    Id = 6,
                    Date = new DateTime(2015, 10, 2),
                    Description = "Sed fermentum eros magna",
                    Price = 11,
                    Status = 1,
                    CustomerId = 3,
                    Customer = Customers[2]
                },
                new OrderEntity
                {
                    Id = 7,
                    Date = new DateTime(2015, 11, 27),
                    Description = "Morbi auctor pulvinar",
                    Price = 5,
                    Status = 0,
                    CustomerId = 4,
                    Customer = Customers[3]
                },
                new OrderEntity
                {
                    Id = 8,
                    Date = new DateTime(2015, 12, 20),
                    Description = "Praesent vel velit nec",
                    Price = 45,
                    Status = 0,
                    CustomerId = 5,
                    Customer = Customers[4]
                },
            });

        } 
    }
}
