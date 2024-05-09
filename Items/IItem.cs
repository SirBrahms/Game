namespace Game.Items;

interface IItem 
{
    public string Name { get; }
    public string Description { get; }
    public bool RequirementCheck();
}
