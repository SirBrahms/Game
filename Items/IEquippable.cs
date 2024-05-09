using System.Reflection;

namespace Game.Items;

interface IEquippable
{
    public bool HasDurability { get; }
    public float Durability { get; }
    public int RequiredItemSlots { get; }
    public void Equip();
    public void Unequip();
    public void ActionActive();
    public void ActionPassive();
}