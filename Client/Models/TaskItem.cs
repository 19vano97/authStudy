using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Client.Models;

[Index("ProjectId", Name = "IX_TaskItems_ProjectId")]
[Index("StatusId", Name = "IX_TaskItems_StatusId")]
public partial class TaskItem
{
    [Key]
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int StatusId { get; set; }

    public Guid? ReporterId { get; set; }

    public Guid? AssigneeId { get; set; }

    public Guid? ProjectId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime ModifyDate { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("TaskItems")]
    public virtual ProjectItem? Project { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("TaskItems")]
    public virtual TaskItemStatus Status { get; set; } = null!;
}
