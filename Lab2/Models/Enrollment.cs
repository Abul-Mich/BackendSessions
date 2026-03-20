using System;
using System.Collections.Generic;

namespace DbApi.Models;

public partial class Enrollment
{
    public int Id { get; set; }

    public int? Studentid { get; set; }

    public int? Courseid { get; set; }

    public DateTime Enrollmentdate { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Student? Student { get; set; }
}
