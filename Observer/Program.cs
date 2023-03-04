/**
Flow:
1. Attach IObservers (updatable) to an ISubject (notifier, state-holding, attachable to & detachable from), which may store the IObservers in a list etc
2. Make a change to an ISubject
3. ISubject can loop through attached Observers and call their Update method, as they are Updatable, as required by IObserver interface
4. Observers' Update() methods are called, and they can do whatever.
*/


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
subject.State = 1;
Console.WriteLine("");

// Detach an observer and change the subject's state again
subject.Detach(observer2);
Console.WriteLine("");

subject.State = 2;

// Enforces ways to subscribe/unsubscribe observers, a way to notify them, and the thing (state) they will care about
interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
    int State{get;set;}
}

// Enforces a way to be updated by a Subject
interface IObserver
{
    void Update(ISubject subject);
}

// Has a list of Observers and a State. 
// When setting state, calls Notify, which can call the Observers' Update because of their IObserver interface, ensuring they are updatable.
class Subject : ISubject
{
    public List<IObserver> observers = new List<IObserver>();
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
            Console.WriteLine($"Changing {nameof(Subject)}'s state to {value}");
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
		Console.WriteLine("Removed an observer");
    }

    public void Notify()
    {
        foreach (IObserver observer in observers)
        {
            Console.WriteLine($"{nameof(Subject)} is notifying an observer...");
            observer.Update(this);
            Console.WriteLine($"{nameof(Subject)} just notified an observer\n");
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