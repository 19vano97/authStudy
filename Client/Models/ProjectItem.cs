using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Client.Models;

public partial class ProjectItem
{
    [Key]
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid? OwnerId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime ModifyDate { get; set; }

    [InverseProperty("Project")]
    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
}
