using System.Numerics;
using AttackStructures;

namespace NinjaWithOCP;

/// <summary>
/// Problem: If we want to change weapons, we must MODIFY the class.
/// Open-Closed Principle: Open for extension, CLOSED for MODIFICATION.
/// </summary>
public class Ninja : IAttackable, IAttacker
{
    private readonly Weapon _meleeWeapon;
    private readonly Weapon _rangedWeapon;

    public string Name { get; }
    public Vector2 Position { get; set; }

    // Dependency injection instead of hardcoded instantiation in NinjaNoOCP version
    public Ninja(string name, Weapon meleeWeapon, Weapon rangedWeapon, Vector2? position = null)
    {
        Name = name;
        _meleeWeapon = meleeWeapon;
        _rangedWeapon = rangedWeapon;
        Position = position ?? Vector2.Zero;
    }

    public AttackResult Attack(IAttackable target)
    {
        var distance = this.DistanceFrom(target);   // extension method for IAttackable interface, of which a Ninja is (IAttackable)

        if(_meleeWeapon.CanHit(distance))
        {
            return new AttackResult(_meleeWeapon, this, target);
        }
        else
        {
            return new AttackResult(_rangedWeapon, this, target);
        }
    }
}