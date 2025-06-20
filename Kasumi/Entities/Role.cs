using MongoDB.Bson;

namespace Kasumi.Entities;

public class Role
{
    public ObjectId Id { get; set; }
    public ulong RoleId { get; set; }
    public string Name { get; set; }
    public ulong ServerId { get; set; }
}