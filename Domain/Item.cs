using System.ComponentModel.DataAnnotations;

namespace TodoMinApi.Domain
{
    //public record Item(int Id, string Title, bool Completed);
    public class Item {

    // public Item(int id, string title, bool completed)
    // {
    //     Id = id;
    //     Title = title;
    //     Completed = completed;
    // }

    public int Id { get; set; }
    [Required]
    public string? Title { get; set; }
    public bool Completed { get; set; }
}
}