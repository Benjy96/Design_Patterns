// Create the subject
Subject subject = new Subject();
Console.WriteLine("");

// Create Observers
Observer observer1 = new Observer("Observer 1");
Observer observer2 = new Observer("Observer 2");
Console.WriteLine("");

// Attach observers to the subject
subject.Attach(observer1);
subject.Attach(observer2);
Console.WriteLine("");

// Change the subject's state and notify observers
Console.WriteLine("Changing the subject's state to 1...");
subject.State = 1;
Console.WriteLine("");

// Detach an observer and change the subject's state again
Console.WriteLine($"Detaching {nameof(observer2)}..."); 
subject.Detach(observer2);
Console.WriteLine("");

Console.WriteLine("Changing the subject's state to 2...");
subject.State = 2;

interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
	int State {get;set;}
}

interface IObserver
{
    void Update(ISubject subject);
}

class Subject : ISubject
{
    private List<IObserver> observers = new List<IObserver>();
    private int state;
	
	public Subject()
	{
		Console.WriteLine("Created a Subject");
	}

    public int State
    {
        get { return state; }
        set
        {
            state = value;
            Notify();
        }
    }

    public void Attach(IObserver observer)
    {
        observers.Add(observer);
		Console.WriteLine("Attached an observer");
    }

    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (IObserver observer in observers)
        {
            observer.Update(this);
        }
    }
}

class Observer : IObserver
{
    private string name;

    public Observer(string name)
    {
        this.name = name;
		Console.WriteLine("Created an Observer: " + name);
    }

    public void Update(ISubject subject)
    {
        Console.WriteLine($"{name} received notification: Subject's state is {subject.State}");
    }
}