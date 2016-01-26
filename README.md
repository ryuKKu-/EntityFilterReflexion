EntityFilterReflexion
=

Filter a set of entities from view-model properties based on reflexion.
Support deep-linked entities properties

## Example
### Entities
```csharp
    Entity Customer :
      public string Id { get; set; }
      public string Name { get; set; }
      public virtual ICollection<Order> Orders { get; set; }
    
    Entity Order :
      public int Id { get; set; }
      public string TypeOrder { get; set; }
      public DateTime Date { get; set; }
      public double Price { get; set; }
      public int Status { get; set; }
      [ForeignKey("Customer")]
      public string CustomerId { get; set; }
      public virtual Customer Customer { get; set; }
      public virtual ICollection<Good> Goods { get; set; }
      
    Entity Good : 
      public int Id { get; set; }
      public double Price { get; set; }
      public string Name { get; set; }
      
      public virtual ICollection<Order> Orders { get; set; }

    
```
    
### ViewModel

Apply the attribute `EntityFilter` on properties (corresponding to your entity properties) that need to be filtered.
This library is using reflexion to find the corresponding property in your entity, so if your view-model property's name differs, use `OnProperty` attribute. `Operator` corresponds to the type of expression used in the `Where` clause.

Example
```csharp
      [EntityFilter(Operator = OperatorType.EqualDate)]
      public DateTime? Date { get; set; }
      
      [EntityFilter(Operator = OperatorType.Equal)]
      public double? Price { get; set; }
  
      [EntityFilter(Operator = OperatorType.GreaterThanOrEqual, OnProperty = "Price")]
      public double? PriceBot { get; set; }
  
      [EntityFilter(Operator = OperatorType.LessThanOrEqual, OnProperty = "Price")]
      public double? PriceTop { get; set; }
  
      [EntityFilter(Operator = OperatorType.Equal, OnProperty = "TypeOrder")]
      public string Type { get; set; }
  
      [EntityFilter(Operator = OperatorType.Equal)]
      public int? Status { get; set; }
  
      [EntityFilter(Operator = OperatorType.Contains, OnProperty = "Customer.Name")]
      public string Name { get; set; }
  
      public string Order { get; set; }
      public string Way { get; set; }
```

You can also filter on a collection property by using this special syntax : 

`OnProperty = "[(1) | (2) | (3) (4) (5)].X"`

(1) must be the collection property name in your entity
(2) must be the subquery method name
(3) must be the collection attribute name to compare
(4) must be the compare method
(5) must be the model property to compare with in the subquery

Example
```csharp
    [EntityFilter(Operator = OperatorType.Equal, OnProperty = "[Goods(1) | First(2) | MerchantId(3) ==(4) MerchId(5)].Price")]
    public double GoodPrice { get; set; }
```

This will compile to the following LINQ query : 
orders.Where(x => x.Goods.First(g => g.MerchantId == model.MerchId).Price == model.GoodPrice);



### Controller
    
To apply the filter to your set of entities, retrieve them from your database as IQueryable object and use the `FilterFromModel` extension method
    
```csharp
      IQueryable<Order> query = dbContext.Orders.AsQueryable();
      var filtered = query.ConstructQuerySearch(viewModel);
```
You can also order your results by using `OrderByReflexion` extension method
    
```csharp
      var order = filtered.OrderByReflexion(viewModel.Order, viewModel.Way);
```
Note : `Way` must be equals to `asc` or `desc`


    
