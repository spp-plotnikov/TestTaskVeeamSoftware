using Moq;
using NUnit.Framework;
using System;

namespace WinServicesManager.Tests
{
    [TestFixture]
    public class ServicesModelTest
    {
        [Test]
        public void ShouldNotBlockUIThreadWhenWaitingLongOperation()
        {
            bool waitForComplete = true;
            var mock = new Mock<IWinServicesProvider>();
            mock.Setup(m => m.ListAllWindowsServices()).Callback(() => { while (waitForComplete) ; });
            
            using (var model = new ServicesModel(mock.Object))
            {
                var result = model.WindowsServices;
                Assert.IsNotNull(result); // can get result at any time
                waitForComplete = false;
            }
        }
    }
}
