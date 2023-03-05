/*
Mediator pattern is to ensure classes that should receive messages can receive messages, and that a single sending class can send those messages.

Useful for when you want to control the flow of information to certain classes, perhaps only sending at certain times, and informing everyone.

Seems kinda similar to the Observer pattern (about informing people).
Difference is in Observer, stakeholders subscribe (have control). With Mediator, mediator controls who messages go to.
*/


// Thing that sends messages to other things
public interface Mediator
{
    string Name {get;}
    void SendMessage(Message message);
}

// Something that should be able to receive a message
public interface IColleague
{
    string Name {get;}
    void ReceiveMessage(Message message);
}

// Info about sender and content
public class Message
{
    public IColleague Sender {get;}
    public string Content {get;}
    public Message(IColleague from, string content)
    {
        Sender = from;
        Content = content;
    }
}

public class Lawyer : Mediator
{
    public string Name {get;}  // essentially readonly, but properties can be added to an interface, whereas a readonly field can't
    private readonly List<IColleague> _colleagues;   // readonly means can only assign at declaration/in constructor

    // if this were params int[] x, it would mean you can pass the array inline like so: 1,2,3,4,5
    public Lawyer(string name, params IColleague[] colleagues)
    {
        ArgumentNullException.ThrowIfNull(colleagues);
        Name = name;
        
        _colleagues = new List<IColleague>(colleagues);

        Console.WriteLine($"({nameof(Lawyer)}) {Name} is now responsible for:");
        _colleagues.ForEach(c => Console.WriteLine($"{c.Name}"));
    }

    public void SendMessage(Message m)
    {
        foreach(IColleague c in _colleagues)
        {
            c.ReceiveMessage(m);
        }
    }
}

public class JuryMember : IColleague
{
    public string Name {get;}
    public JuryMember(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Console.WriteLine($"{Name} just joined the Jury.");
    }
    public void ReceiveMessage(Message m)
    {
        Console.WriteLine($"{Name}: {m.Sender.Name} just told me this: {m.Content}");
    }
}

public class Program
{
    public static void Main()
    {
        JuryMember bob = new(nameof(bob));
        JuryMember alice = new(nameof(alice));
        Console.WriteLine();

        Lawyer prick = new(nameof(prick), bob, alice);
        Console.WriteLine();
        
        Message m = new Message(bob, "I heard the ice cream truck is coming at 1pm");
        
        prick.SendMessage(m);
    }
}