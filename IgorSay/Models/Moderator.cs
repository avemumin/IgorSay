using Postgrest.Attributes;
using Postgrest.Models;

namespace IgorSay.Models;

[Table("moderator")]
public class Moderator : BaseModel
{
  [PrimaryKey("id")]
  public int Id { get; set; }

  [Column("username")]
  public string Username { get; set; }

  [Column("user_id")]
  public string UserId { get; set; }

}
