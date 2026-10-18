using System.ComponentModel.DataAnnotations;

namespace BolumKurulu.Web.Validation;

public class TcKimlikNoAttribute : ValidationAttribute
{
    public TcKimlikNoAttribute() : base("Geçerli bir T.C. kimlik numarası giriniz.") { }

    public override bool IsValid(object? value)
    {
        if (value is not string tc || tc.Length != 11 || !tc.All(char.IsDigit) || tc[0] == '0')
            return false;

        var d = tc.Select(c => c - '0').ToArray();
        int d10 = ((d[0] + d[2] + d[4] + d[6] + d[8]) * 7 - (d[1] + d[3] + d[5] + d[7]) + 10 * 10) % 10;
        int d11 = d.Take(10).Sum() % 10;

        return d[9] == d10 && d[10] == d11;
    }
}