using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolset;
using System.Text.RegularExpressions;

namespace Toolset.Text.Template
{
  class CastExpression : Expression
  {
    private static Regex dateRegex;
    private static Regex inverseDateRegex;
    private static Regex timeRegex;
    private static Regex zoneRegex;

    private Type type;

    public CastExpression(Type type)
    {
      this.type = type;
    }

    internal override Pipe Evaluate(Pipe input, object target, object context)
    {
      object result = null;
      try
      {
        result = ConvertPipeTo(input, type);
      }
      catch (Exception ex)
      {
        ex.Report(
          "Não foi possível aplicar uma conversão para " + type.Name + ": " + input
        );
      }
      return new Pipe(result);
    }

    public static object ConvertPipeTo(Pipe input, Type type)
    {
      if (input.IsNone)
        return MakeDefaultValue(type);

      return ConvertValueTo(input.Value, type);
    }

    public static object ConvertValueTo(object value, Type type)
    {
      if (value == null)
        return MakeDefaultValue(type);

      if (value.GetType() == type)
        return value;

      if (type == typeof(int))
      {
        if (value is float) return (int)((float)value);
        if (value is double) return (int)((double)value);
        if (value is bool) return ((bool)value) ? 1 : 0;

        var text = (value ?? "").ToString();
        int number;
        return int.TryParse(text, out number) ? number : 0;
      }

      if (type == typeof(float))
      {
        if (value is int) return (float)value;
        if (value is double) return (float)new decimal((double)value);
        if (value is bool) return ((bool)value) ? 1F : 0F;

        var text = (value ?? "").ToString();
        float number;
        return float.TryParse(text, out number) ? number : 0F;
      }

      if (type == typeof(bool))
      {
        if (value is int) return ((int)value) != 0;
        if (value is float) return ((float)value) != 0F;
        if (value is double) return ((double)value) != 0D;
        
        var text = (value ?? "").ToString();
        bool boolean;
        if (bool.TryParse(text, out boolean))
          return boolean;

        int number;
        return int.TryParse(text, out number) ? (number != 0) : false;
      }

      if (type == typeof(DateTime))
      {
        var text = (value ?? "").ToString();
        var dateTime = CreateDateTime(text);
        if (dateTime != null)
          return dateTime.Value;
      } 
        
      return null;
    }

    private static object MakeDefaultValue(Type type)
    {
      return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    private static DateTime? CreateDateTime(string text)
    {
      DateTime? datePart = ParseDate(text);
      DateTime? timePart = ParseTime(text);

      if (datePart != null && timePart != null)
      {
        var date = datePart.Value;
        var time = timePart.Value;
        var zone = ParseZone(text);
        var compound = new DateTimeOffset(
          date.Year, date.Month, date.Day,
          time.Hour, time.Minute, time.Second, time.Millisecond,
          zone
        );
        //return compound.LocalDateTime;
        return compound.DateTime;
      }
      else if (datePart != null)
      {
        return datePart.Value;
      }
      else if (timePart != null)
      {
        return timePart.Value;
      }

      return null;
    }

    private static DateTime? ParseDate(string text)
    {
      Match match;

      if (inverseDateRegex == null)
        inverseDateRegex = new Regex("([0-9]{4})[-/]([0-9]{1,2})[-/]([0-9]{1,2})");

      match = inverseDateRegex.Match(text);
      if (match.Success)
      {
        var year = int.Parse(match.Groups[1].Value);
        var month = int.Parse(match.Groups[2].Value);
        var day = int.Parse(match.Groups[3].Value);
        return new DateTime(year, month, day);
      }

      if (dateRegex == null)
        dateRegex = new Regex("([0-9]{1,2})[-/]([0-9]{1,2})[-/]([0-9]{2,4})");

      match = dateRegex.Match(text);
      if (match.Success)
      {
        var day = int.Parse(match.Groups[1].Value);
        var month = int.Parse(match.Groups[2].Value);
        var year = int.Parse(match.Groups[3].Value);
        if (year < 80)
          year += 2000;
        if (year < 100)
          year += 1900;
        return new DateTime(year, month, day);
      }

      return null;
    }

    private static DateTime? ParseTime(string text)
    {
      Match match;

      if (timeRegex == null)
        timeRegex = new Regex("([0-9]{1,2})[:]([0-9]{2})(?:[:]([0-9]{2}))?(?:[.]([0-9]{3}))?");

      match = timeRegex.Match(text);
      if (match.Success)
      {
        var hour = int.Parse(match.Groups[1].Value);
        var minute = int.Parse(match.Groups[2].Value);
        var second = int.Parse("0" + match.Groups[3].Value);
        var millisecond = int.Parse("0" + match.Groups[4].Value);

        var date = DateTime.MinValue;
        return new DateTime(
          date.Year, date.Month, date.Day,
          hour, minute, second, millisecond
        );
      }

      return null;
    }

    private static TimeSpan ParseZone(string text)
    {
      Match match;

      if (zoneRegex == null)
        zoneRegex = new Regex("([+-][0-9]{1}):([0-9]{2})");

      match = zoneRegex.Match(text);
      if (match.Success)
      {
        var hour = int.Parse(match.Groups[1].Value);
        var minute = int.Parse(match.Groups[2].Value);
        var zone = new TimeSpan(hour, minute, 0);
        return zone;
      }

      return DateTimeOffset.Now.Offset;
    }
  }
}
