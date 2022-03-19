using TodoMinApi.Domain;

namespace TodoMinApi.Data
{
    public class ItemRepository
    {
        private readonly Dictionary<int, Item> _items = new Dictionary<int, Item>();

        public ItemRepository()
        {
            var item1 = new Item(1, "Go to the gym", false);
            var item2 = new Item(2, "Buy bread", false);
            var item3 = new Item(3, "Watch TV ", false);
            _items.Add(item1.Id, item1);
            _items.Add(item2.Id, item2);
            _items.Add(item3.Id, item3);
        }

        public IEnumerable<Item> GetAll() => _items.Values;

        public Item GetById(int id) => _items[id];

        public void Add(Item item) {
            _items.Add(item.Id, item);
        }

        public void Update(Item item) => _items[item.Id] = item;
        public void Delete(int id) => _items.Remove(id);        
    }
}