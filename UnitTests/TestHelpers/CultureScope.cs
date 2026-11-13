using System.Globalization;

namespace UnitTests.TestHelpers;

// Sets the current culture for one test and restores the original afterwards,
// so a test with another culture cannot affect the tests that run after it
internal sealed class CultureScope : IDisposable
{
    private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;

    public CultureScope(string cultureName)
        => CultureInfo.CurrentCulture = new CultureInfo(cultureName);

    public void Dispose()
        => CultureInfo.CurrentCulture = _originalCulture;
}