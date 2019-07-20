using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdapterContext.DataModel
{
  [Table("Adapters", Schema = "mtc")]
  public class AdapterConfig
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual List<DataItem> Items { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastStarted { get; set; }
    public int PollRate { get; set; }

    public AdapterConfig()
    {
      this.Items = new List<DataItem>();
    }
  }
}
