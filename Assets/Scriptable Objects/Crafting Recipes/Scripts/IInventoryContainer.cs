
public interface IInventoryContainer 
{
    bool ContainsItem(Item item);
    bool AddItem(Item item);
    bool RemoveItem(Item item);
    bool IsFull();

}
