using Dapper;
using Microsoft.Data.Sqlite;
using TodoMinApi.Domain;

namespace TodoMinApi.Data
{
    public class ItemRepository
    {
        private readonly Dictionary<int, Item> _items = new Dictionary<int, Item>();
        SqliteConnection db;
        public ItemRepository(string connectionString)
        {
            db = new SqliteConnection(connectionString);
            //    using var db = services.CreateScope().ServiceProvider.GetRequiredService<SqliteConnection>();
            var sql = $@"CREATE TABLE IF NOT EXISTS Items (
                        {nameof(Item.Id)} INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                        {nameof(Item.Title)} TEXT NOT NULL,
                        {nameof(Item.Completed)} INTEGER DEFAULT 0 NOT NULL CHECK({nameof(Item.Completed)} IN (0, 1))
                    );";
            db.ExecuteAsync(sql);
        }

        public async Task<IEnumerable<Item>> GetAll() => 
            await db.QueryAsync<Item>("SELECT * FROM Items");

        public async Task<Item> GetById(int id) => 
            await db.QuerySingleOrDefaultAsync<Item>("SELECT * FROM Items WHERE Id = @id", new { id });

        public async void Add(Item item) {
            await db.QuerySingleAsync<Item>(
            "INSERT INTO Items(Title, Completed) Values(@Title, @Completed) RETURNING * ", item);
            ///_items.Add(item.Id, item);
        }

        public async void Update(Item item) => 
                await db.ExecuteAsync("UPDATE Items SET Title = @Title, Completed = @Completed WHERE Id = @Id", item);

        public async void Delete(int id) => await db.ExecuteAsync("DELETE FROM Items WHERE Id = @id", new { id });        
    }
}