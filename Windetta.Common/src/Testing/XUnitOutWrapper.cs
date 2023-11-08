using System.Text;
using Xunit.Abstractions;

namespace Windetta.Common.Testing;

public class XUnitOutWrapper : TextWriter
{
    private readonly ITestOutputHelper _outputHelper;

    public XUnitOutWrapper(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    public override Encoding Encoding => Encoding.ASCII;

    public override void WriteLine(string? value)
    {
        base.WriteLine(value);
        _outputHelper.WriteLine(value);
    }
}