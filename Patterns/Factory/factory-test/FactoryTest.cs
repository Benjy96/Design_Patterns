using System;
using System.Collections;
using System.Collections.Generic;
using Factory;
using Xunit;

#region Data
public abstract class AbstractFactoryBaseTestData : IEnumerable<object[]>
{
    // Store a set of IVehicleFactories and a Type
    private readonly TheoryData<IVehicleFactory, Type> _theoryData = new TheoryData<IVehicleFactory, Type>();
    
    // Call this method with a Factory & Vehicle type param, and where the Factory is instantiated with new()
    // where T : new() means T must have public, parameterless constructor
    // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters
    protected void AddTestData<TConcreteFactory, TExpectedVehicle>() where TConcreteFactory : IVehicleFactory, new()   
    {
        // new TConcreteFactory() type param won't work without new() constraint
        _theoryData.Add(new TConcreteFactory(), typeof(TExpectedVehicle));
    }
    
    // IEnumerable Implementations
    public IEnumerator<object[]> GetEnumerator() => _theoryData.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class AbstractFactoryTestCars : AbstractFactoryBaseTestData
{
    public AbstractFactoryTestCars()
    {
        AddTestData<LowGradeVehicleFactory, CrapCar>();
        AddTestData<HighGradeVehicleFactory, LuxuryCar>();
    }
}
public class AbstractFactoryTestBikes : AbstractFactoryBaseTestData
{
    public AbstractFactoryTestBikes()
    {
        AddTestData<LowGradeVehicleFactory, CrapBike>();
        AddTestData<HighGradeVehicleFactory, LuxuryBike>();
    }
}
#endregion

public class AbstractFactoryTest
{
    [Theory]
    [ClassData(typeof(AbstractFactoryTestCars))]
    // AbstractFactoryTestCars adds test data to an enumerable, which gets passed to these params
    // Passes LowGradeVehicleFactory -> CrapCar
    // Passes HighGradeVehicleFactory -> LuxuryCar
    public void Should_create_a_Car_of_the_specified_type(IVehicleFactory vehicleFactory, Type expectedCarType)
    {
        // Act
        ICar result = vehicleFactory.CreateCar();
        // Assert
        Assert.IsType(expectedCarType, result);
    }

    [Theory]
    [ClassData(typeof(AbstractFactoryTestBikes))]
    public void Should_create_a_Bike_of_the_specified_type(IVehicleFactory vehicleFactory, Type expectedBikeType)
    {
        // Act
        IBike result = vehicleFactory.CreateBike();
        // Assert
        Assert.IsType(expectedBikeType, result);
    }
}