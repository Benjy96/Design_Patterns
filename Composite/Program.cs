/**
Composite Pattern:
Leaves & Branches :implementing: ILivingThing

A leaf can store something like a name or colour.
A branch can also store something like a name/colour, but ALSO other branches or leaves (anything implementing ILivingThing)

* What is it good for?: THINGS WHICH MAY BE NESTED / HIERARCHICAL
- GUI (form within a form)
- file structures: ISystemObject could enable a Folder to contain files
- teams: IUnit could enable a company, department, sector, team, and sprint team
*/

public interface INode
{
    string Name { get; set; }
    void Display(int depth);
}

// Node
public class Node : INode
{
    public string Name { get; set; }

    public void Display(int depth)
    {
        // Write(new string('-', 5) + Name) will print -----name
        Console.WriteLine(new string('-', depth) + Name);
    }
}

///<summary>
/// "Composite" Node that can hold other nodes 
///</summary>
public class Composite : INode
{
    private List<INode> _children = new List<INode>();

    public string Name { get; set; }

    public void Add(INode node)
    {
        Console.WriteLine($"Adding {node.Name} to ({nameof(Composite)}) {Name}");
        _children.Add(node);
    }

    public void Remove(INode node)
    {
        Console.WriteLine($"Removing {node.Name} from ({nameof(Composite)}) {Name}");
        _children.Remove(node);
    }

    ///<summary>
    /// Prints this node's name and then calls Display on the child nodes: <br/>
    /// Nodes will print themselves only <br/>
    /// Composites will once again print themselves and then call Display on their child nodes
    ///</summary>
    public void Display(int numDashes)
    {
        Console.WriteLine(new string('-', numDashes) + Name);


        foreach (INode node in _children)
        {
            node.Display(numDashes + 2);
        }
    }
}

public class Program
{
    static void Main(string[] args)
    {
        Composite root = new Composite { Name = "C_Root" };
        root.Add(new Node { Name = "Leaf A" });
        root.Add(new Node { Name = "Leaf B" });

        Composite x = new Composite { Name = "C_X" };
        x.Add(new Node { Name = "Leaf XA" });
        x.Add(new Node { Name = "Leaf XB" });

        root.Add(x);
        root.Add(new Node { Name = "Leaf C" });

        // The "root" Composite Node now holds 4 objects: 3 Nodes and 1 other Composite

        Node leaf = new Node { Name = "Leaf D" };
        root.Add(leaf);
        root.Remove(leaf);

        Console.WriteLine($"\nCalling {nameof(root.Display)}() on {nameof(root)}, which is a {nameof(Composite)}...");
        root.Display(numDashes: 0);
    }
}
