/*
Mapper class simply converts one type to another.

Can make flexible with generics (give Map TSource, and return a TDestination)
*/

public interface IMapper<TSource, TDestination>
{
    TDestination Map(TSource entity);
}

public class ProductMapper : IMapper<Product, ProductDetails>
{
    public ProductDetails Map(Product entity)
    {
        return new ProductDetails(Id: entity.Id ?? default, Name: entity.Name, QuantityInStock: entity.QuantityInStock);
    }
}

public record class ProductDetails(int Id, string Name, int QuantityInStock);
public class Product 
{ 
    public int? Id {get;set;}
    public string Name {get;set;} = "";
    public int QuantityInStock {get;set;}
}

public class Program
{
    public static void Main(string[] args)
    {
        Product product = new() { Id = 1, Name = "Test", QuantityInStock = 5};

        ProductMapper productMapper = new();
        ProductDetails pd = productMapper.Map(product);

        Console.WriteLine(pd);
    }
}