using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data;

public class Group(string name)
{
    [Key]
    public string Name { get; set; } = name;
    public ICollection<Connection> Connections { get; set; } = [];
}
