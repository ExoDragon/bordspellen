namespace Core.Infrastructure.Helper;

public static class Helper
{
    public static int Age(DateTime dateTime)
    {
        var today = DateTime.Now;
        int age = today.Year - dateTime.Year;
        age = dateTime > today.AddYears(age) ? age - 1 : age;
        return age;
    }
}