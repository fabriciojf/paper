using System;
using System.Collections.Generic;
using System.Text;

namespace Toolset.Data
{
  public struct Time
  {
    public DateTime Value { get; }

    private Time(DateTime time)
    {
      this.Value = DateTime.MinValue + time.TimeOfDay;
    }

    private Time(TimeSpan time)
    {
      this.Value = DateTime.MinValue + time;
    }

    public Time(long ticks)
    {
      this.Value = DateTime.MinValue + new TimeSpan(ticks);
    }

    public Time(int hours, int minutes, int seconds)
    {
      this.Value = DateTime.MinValue + new TimeSpan(hours, minutes, seconds);
    }

    public Time(int days, int hours, int minutes, int seconds)
    {
      this.Value = DateTime.MinValue + new TimeSpan(days, hours, minutes, seconds);
    }

    public Time(int days, int hours, int minutes, int seconds, int milliseconds)
    {
      this.Value = DateTime.MinValue + new TimeSpan(days, hours, minutes, seconds, milliseconds);
    }

    public static implicit operator TimeSpan(Time time)
    {
      return time.Value.TimeOfDay;
    }

    public static implicit operator Time(TimeSpan time)
    {
      return new Time(time);
    }

    public static implicit operator DateTime(Time time)
    {
      return time.Value;
    }

    public static implicit operator Time(DateTime time)
    {
      return new Time(time);
    }

    public override string ToString()
    {
      return Value.ToString("hh:mm:sszzz");
    }

    public string ToString(string format)
    {
      return Value.ToString(format);
    }

    public string ToString(IFormatProvider formatProvider)
    {
      return Value.ToString(formatProvider);
    }
  }
}