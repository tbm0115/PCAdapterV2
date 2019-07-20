using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterContext.DataModel
{
  [Table("Durations", Schema = "mtc")]
  public class Duration
  {
    [Key]
    public int Id { get; set; }
    public DateTime Started { get; set; }
    public DateTime? Ended { get; set; }
    public virtual DataItem Item { get; set; }
    public long? Timespan { get; set; }
    public string Value { get; set; }

    public Duration()
    {

    }
  }
}
