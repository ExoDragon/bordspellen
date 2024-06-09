using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Core.Domain.Attributes;

public class DateFutureAttribute : RangeAttribute
{
    public DateFutureAttribute() : base(
        typeof(DateTime),
        DateTime.Now.ToString(CultureInfo.CurrentCulture),
        DateTime.MaxValue.ToString(CultureInfo.CurrentCulture)
    )
    {}
}