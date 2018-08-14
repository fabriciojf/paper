using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Toolset.Structures
{
  public struct Date
  {
    public DateTime Value { get; }

    private Date(DateTime date)
    {
      this.Value = date.Date;
    }

    public Date(long ticks)
    {
      this.Value = new DateTime(ticks).Date;
    }

    public Date(long ticks, DateTimeKind kind)
    {
      this.Value = new DateTime(ticks, kind).Date;
    }

    public Date(int year, int month, int day)
    {
      this.Value = new DateTime(year, month, day).Date;
    }

    public Date(int year, int month, int day, Calendar calendar)
    {
      this.Value = new DateTime(year, month, day, calendar).Date;
    }

    public static implicit operator DateTime(Date date)
    {
      return date.Value;
    }

    public static implicit operator Date(DateTime date)
    {
      return new Date(date);
    }

    public override string ToString()
    {
      return Value.ToString("yyyy-MM-dd");
    }

    public string ToString(string format)
    {
      return Value.ToString(format);
    }

    public string ToString(IFormatProvider provider)
    {
      return Value.ToString(provider);
    }
  }
}