using Postgrest.Attributes;
using Postgrest.Models;

namespace IgorSay.Models;

//public record Term(string key, string value);
[Table("terms")]
public class Term : BaseModel
{
  [PrimaryKey("id", false)]
  public int Id { get; set; }

  [Column("key")]
  public string Key { get; set; }

  [Column("value")]
  public string Value { get; set; }

  [Column("created_at")]
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  [Column("approved")]
  public bool Approved { get; set; } = false;

  public Term() { }

  public Term(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
