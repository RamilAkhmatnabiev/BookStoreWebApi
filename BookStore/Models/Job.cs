using System;
using System.Collections.Generic;

namespace BookStoreWebApi.Models;

public partial class Job
{
    public short JobId { get; set; }

    public string JobDesc { get; set; } = null!;
}
