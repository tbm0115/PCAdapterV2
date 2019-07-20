using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdapterContext;

namespace AdapterContext.DataModel
{
  [Table("DataItems", Schema = "mtc")]
  public class DataItem:IDataItem
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public int TickFrequency { get; set; }

    public DataItem()
    {

    }
  }
}
