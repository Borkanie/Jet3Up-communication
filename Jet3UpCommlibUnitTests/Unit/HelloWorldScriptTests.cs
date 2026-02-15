using System.Text;
using Jet3UpCommLib.Helpers;
using Jet3UpCommLib.Helpers.Factories;
using Jet3UpCommLib.Helpers.Jobs;
using Jet3UpCommLib.MockUp.Client;
using Jet3UpCommLib.Tests.Helpers;

namespace Jet3UpCommLib.Tests.Unit
{
    public class HelloWorldScriptTests
    {
        [Fact]
        public void Jet3UpMessageBuilder_Generates_HelloWorld_Object()
        {
            // Arrange: build a message with a single final object containing "Hello World"
            var builder = Jet3UpMessageBuilder.Start();
            builder.SetSize(FontSizeEnum.ISO1_7x5, rotation: 0, machineType: MachineTypeEnum.Neagra, delay: 100);
            // use Write overload with final text so it registers as single object at the end
            builder.Write("HTZ", "SIG", "ANR", "BTIDX", "CTRL", "Hello World");
            var message = builder.End();

            // The Hello World should appear as the final object block
            Assert.Contains("Hello World", message);
            // It should be wrapped in an OBJ block
            Assert.Contains("^0*OBJ [6", message);
            // Basic structure
            Assert.Contains("^0*BEGINLJSCRIPT", message);
            Assert.Contains("^0*ENDJOB []", message);
            Assert.Contains("^0*ENDLJSCRIPT []", message);
        }
    }
}
