using System;
using System.Collections.Generic;

namespace DbApi.Models;

public partial class Course
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Teacherid { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Teacher? Teacher { get; set; }
}
