EntityFilterReflexion
=

Filter a set of entities from view-model properties based on reflexion.
Support deep-linked entities properties

## Example
### Entities
```csharp
    Entity User :
      public string Id { get; set; }
      public string Name { get; set; }
      public virtual ICollection<Order> Orders { get; set; }
    
    Entity Order :
      public int Id { get; set; }
      public string TypeOrder { get; set; }
      public DateTime Date { get; set; }
      public double Price { get; set; }
      public int Status { get; set; }
      [ForeignKey("User")]
      public string UserId { get; set; }
      public virtual ApplicationUser User { get; set; }
```
    
### ViewModel

Apply the attribute `FilterWhere` on properties (corresponding to your entity properties) that need to be filtered.
This library is using reflexion to find the corresponding property in your entity, so if your view-model property's name differs, use `OnProperty` attribute. `Operator` corresponds to the type of expression used in the `Where` clause.
    
OperatorType.Equal corresponds to 
```csharp
      .Where(x => x.Prop == "foo")
```
    
```csharp
      [FilterWhere(Operator = OperatorType.EqualDate)]
      public DateTime? Date { get; set; }
      
      [FilterWhere(Operator = OperatorType.Equal)]
      public double? Price { get; set; }
  
      [FilterWhere(Operator = OperatorType.GreaterThanOrEqual, OnProperty = "Price")]
      public double? PriceBot { get; set; }
  
      [FilterWhere(Operator = OperatorType.LessThanOrEqual, OnProperty = "Price")]
      public double? PriceTop { get; set; }
  
      [FilterWhere(Operator = OperatorType.Equal, OnProperty = "TypeOrder")]
      public string Type { get; set; }
  
      [FilterWhere(Operator = OperatorType.Equal)]
      public int? Status { get; set; }
  
      [FilterWhere(Operator = OperatorType.Contains, OnProperty = "User.Name")]
      public string Name { get; set; }
  
      public string Order { get; set; }
      public string Way { get; set; }
      public int PageSize { get; set; }
      public int Page { get; set; }
```
    
### Controller
    
To apply the filter to your set of entities, retrieve them from your database as IQueryable object and use `ConstructQuerySearch` extension method
    
```csharp
      IQueryable<Order> query = dbContext.Orders.AsQueryable();
      var filtered = query.ConstructQuerySearch(viewModel);
```
    You can also order your results by using `OrderByReflexion` extension method
```csharp
      var order = filtered.OrderByReflexion(viewModel.Order, viewModel.Way);
```
    Note : `Way` must be equals to `asc` or `desc`
    
    And appy pagination ...
```csharp
      var results = order.ApplyPagination(viewModel.PageSize, viewModel.Page);
```

    
