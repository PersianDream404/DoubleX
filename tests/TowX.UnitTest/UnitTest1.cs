using Xunit.Abstractions;

namespace TowX.UnitTest
{

    public class test
    {
        public void test1()
        {
            Console.WriteLine("test");
        }
    }
    public class UnitTest1(ITestOutputHelper testOutputHelper) :IAsyncLifetime
    {
        public async Task DisposeAsync()
        {
           // throw new NotImplementedException();
        }

        public async Task InitializeAsync()
        {
           // throw new NotImplementedException();
        }

        [Fact]
        public void Test1()
        {
            testOutputHelper.WriteLine("salam");
        }
    }
}
