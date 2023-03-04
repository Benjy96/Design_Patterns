public interface ISortStrategy
{
    void Sort(int[] data);
}

public class BubbleSortStrategy : ISortStrategy
{
    public void Sort(int[] data)
    {
        Console.WriteLine($"Bubble-sorting {nameof(data)}...");
    }
}

public class MergeSortStrategy : ISortStrategy
{
    public void Sort(int[] data)
    {
        Console.WriteLine($"Merge-sorting {nameof(data)}...");
    }
}

public class SortContext
{
    private ISortStrategy sortStrategy;

    public SortContext(ISortStrategy strategy)
    {
        Console.WriteLine($"Created a {nameof(SortContext)} using {strategy.GetType()}");
        sortStrategy = strategy;
    }

    public void SortData(int[] data)
    {
        sortStrategy.Sort(data);
    }
}

public class Program
{
    public enum SystemLoad { Low, Medium, Heavy }

    static void Main(string[] args)
    {
        int[] data = new int[] { 1,2,3,4,5 };

        SystemLoad currentSystemLoad = SystemLoad.Heavy;

        // Choose strategy depending on system load (how much the site is being used)
        ISortStrategy sortStrategy;
        if(currentSystemLoad is SystemLoad.Heavy)
        {
            Console.WriteLine($"{nameof(currentSystemLoad)} is {nameof(SystemLoad.Heavy)}. Therefore using {nameof(MergeSortStrategy)}");
            sortStrategy = new MergeSortStrategy();
        }
        else
        {
            Console.WriteLine($"{nameof(currentSystemLoad)} is NOT {nameof(SystemLoad.Heavy)}. Therefore using {nameof(BubbleSortStrategy)}");
            sortStrategy = new BubbleSortStrategy();
        }

        // Create a Sort "context" (i.e., representing something that needs to sort) and use the selected type of sorting based on some logic (system usage/performance in this case)
        SortContext context = new SortContext(sortStrategy);
        context.SortData(data);
        Console.WriteLine($"Sorted data:");
        Array.ForEach(data, Console.WriteLine);
    }
}
