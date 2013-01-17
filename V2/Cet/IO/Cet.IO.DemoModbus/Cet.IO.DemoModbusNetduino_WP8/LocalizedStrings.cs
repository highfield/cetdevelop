using Cet.IO.DemoModbusNetduino.Resources;

namespace Cet.IO.DemoModbusNetduino
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}