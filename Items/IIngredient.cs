namespace Game.Items;

interface IIngredient
{
    public bool Consumable { get; }
    public void Consume();
    public void Use();
}