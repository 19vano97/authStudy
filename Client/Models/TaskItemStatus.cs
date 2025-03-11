using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Client.Models;

public partial class TaskItemStatus
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime ModifyDate { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}
