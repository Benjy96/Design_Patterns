namespace Factory;

public class Program
{
    public static void Main() { }
}

public interface IVehicleFactory
{
    ICar CreateCar();
    IBike CreateBike();
}

public class HighGradeVehicleFactory : IVehicleFactory
{
    public IBike CreateBike() => new LuxuryBike();
    public ICar CreateCar() => new LuxuryCar();
}

public class LowGradeVehicleFactory : IVehicleFactory
{
    public IBike CreateBike() => new CrapBike();
    public ICar CreateCar() => new CrapCar();
}

public interface ICar { }
public interface IBike { }

public class CrapCar : ICar { }
public class LuxuryCar : ICar { }
public class CrapBike : IBike { }
public class LuxuryBike : IBike { }