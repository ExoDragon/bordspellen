using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Core.Domain.Attributes;

public class DatePastAttribute : RangeAttribute
{
    public DatePastAttribute() : base(
        typeof(DateTime),
        DateTime.MinValue.ToString(CultureInfo.CurrentCulture),
        DateTime.Now.ToString(CultureInfo.CurrentCulture)
    )
    {}
}