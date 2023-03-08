/**
Adapter pattern creates a link between old code and new code, or acts as a "decoupling"/service layer.

* For example: *
If an old Rectangle class implemented no interface, but we create a new interface (IShape), then we can make a new class which implements the interface and then just calls the original methods.

* When is this useful? *
- When you want a new version of something while keeping something old & not changing that old thing
- When you're refactoring old code or code that can't be changed and/or adding new interfaces.
- When you want to decouple code (essentially a service layer).

*/

interface IBank
{
    void Calculate();
}

class CalculatorAdapter : IBank
{
    private LegacyCalculator _adaptee = new LegacyCalculator();

    public void Calculate()
    {
        Console.WriteLine($"{nameof(CalculatorAdapter)}.{nameof(Calculate)}()");
        _adaptee.Calculate();
    }

    public void DoFancyNewStuff()
    {
        Console.WriteLine($"{nameof(CalculatorAdapter)} is doing some new stuff");
    }
}

class LegacyCalculator
{
    public void Calculate()
    {
        DoX();
        Console.WriteLine($"{nameof(LegacyCalculator)}.{nameof(Calculate)}()");
    }

    private void DoX() { }
}

class Program
{
    static void DoImportantStuff()
    {
        Console.WriteLine($"{nameof(DoImportantStuff)}()...");
        LegacyCalculator lR = new LegacyCalculator();
        lR.Calculate();
        lR.Calculate();
        lR.Calculate();
        Console.WriteLine();
    }

    static void Main()
    {
        DoImportantStuff(); // Don't change anything about/in this method! Hasn't been touched for 40 years and doing some complex stuff

        IBank bank = new CalculatorAdapter();
        Console.WriteLine($"Calling ({bank.GetType()}) {nameof(bank)}'s {nameof(bank.Calculate)}() method...");
        bank.Calculate();
    }
}
