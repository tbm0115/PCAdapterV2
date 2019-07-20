using System;
using System.Threading.Tasks;
using System.Xml;

namespace AdapterContext
{
  public class DataItemChangedEventArguments : EventArgs
  {
    public dynamic DataItem { get; }
    public DateTime Timestamp { get; }

    public DataItemChangedEventArguments(dynamic dataItem)
    {
      this.DataItem = dataItem;
      this.Timestamp = DateTime.UtcNow;
    }
  }
  public interface IDataItem
  {
    string Name { get; }
    bool IsActive { get; set; }
    int TickFrequency { get; set; }
  }
  public abstract class ADataItem : IDataItem {
    public string Name => "Not Implemented";
    public bool IsActive { get; set; }
    public int TickFrequency{get;set;}
    private bool _hasChanged { get; set; }
    public bool HasChanged => this._hasChanged;
    private object _value { get; set; }
    public object Value {
      get {
        return _value;
      }
      set {
        if (value == null)
        {
          this.Unavailable();
        }
        else
        {
          if (_value == null || !_value.Equals(value))
          {
            this.LastChanged = DateTime.UtcNow;
            this._hasChanged = true;
            this._value = value;
            this.DataItemChanged(this, new DataItemChangedEventArguments(this));
          }
          else
          {
            this._hasChanged = false;
          }
        }
      }
    }
    public DateTime LastChanged { get; set; }
    public DateTime LastTicked { get; set; }
    public event DataItemChangedHandler DataItemChanged;
    public delegate void DataItemChangedHandler(ADataItem sender, DataItemChangedEventArguments e);

    public ADataItem()
    {
      this._value = "UNAVAILABLE";
    }

    public virtual async void Tick()
    {
      throw new NotImplementedException();
    }
    public void Unavailable()
    {
      this.Value = "UNAVAILABLE";
    }

    public XmlNode ToXML(XmlDocument xDoc)
    {
      XmlNode xDataItem = xDoc.CreateElement("DataItem");
      xDataItem.AppendChild(xDoc.CreateElement("Name")).InnerText = this.Name;
      xDataItem.AppendChild(xDoc.CreateElement("IsActive")).InnerText = this.IsActive.ToString();
      xDataItem.AppendChild(xDoc.CreateElement("TickFrequency")).InnerText = this.TickFrequency.ToString();

      return xDataItem;
    }
  }
}