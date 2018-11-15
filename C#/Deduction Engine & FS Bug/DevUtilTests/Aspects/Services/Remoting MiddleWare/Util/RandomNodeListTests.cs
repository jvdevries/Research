using System.Linq;
using System.Threading.Tasks;
using DevUtil.Aspects.Services.Remoting_MiddleWare.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevUtilTests.Aspects.Services.Remoting_MiddleWare.Util
{
    [TestClass]
    public class RandomNodeListTests
    {
        public async Task<RandomNodeList> CreateTestList(int n)
        {
            var list = new RandomNodeList();
            for (var i = 1; i <= n; i++)
                await list.Add(i.ToString());
            return list;
        }

        [TestMethod]
        public async Task Add()
        {
            Assert.AreEqual("1,2,3,", (await CreateTestList(3)).ToString());
        }

        [TestMethod]
        public async Task DelHead()
        {
            for (var i = 0; i < 3; i++)
            {
                var list = await CreateTestList(i);
                for (var j = 0; j <= i; j++)
                    await list.Del(j.ToString());
                Assert.AreEqual("", list.ToString());
            }
        }

        [TestMethod]
        public async Task DelTail()
        {
            for (var i = 0; i < 3; i++)
            {
                var list = await CreateTestList(i);
                for (var j = i; j > 0; j--)
                    await list.Del(j.ToString());
                Assert.AreEqual("", list.ToString());
            }
        }

        [TestMethod]
        public async Task DelMiddle()
        {
            var list = await CreateTestList(5);
            for (var j = 2; j < 5; j++)
                await list.Del(j.ToString());
            Assert.AreEqual("1,5,", list.ToString());
        }

        [TestMethod]
        public async Task DelThreaded()
        {
            var n = 10000;
            var k = 8;

            var list = await CreateTestList(n);

            var i = 0;
            var groups = (from item in Enumerable.Range(2, n - 2)
                group item by i++ % 8
                into part
                select part.AsEnumerable()).ToList();

            var tasks = new Task[8];
            foreach (var index in Enumerable.Range(0, k))
                tasks[index] = Task.Run(async delegate
                {
                    foreach (var del in groups[index])
                        await list.Del(del.ToString());
                });

            Task.WaitAll(tasks);

            Assert.AreEqual($"1,{n},", list.ToString());
        }

        [TestMethod]
        public async Task AddThreaded() // Relies on Del working.
        {
            var n = 10000;
            var k = 8;

            var list = await CreateTestList(1);

            var i = 0;
            var groups = (from item in Enumerable.Range(2, n - 1)
                group item by i++ % 8
                into part
                select part.AsEnumerable()).ToList();

            var tasks = new Task[8];
            foreach (var index in Enumerable.Range(0, k))
                tasks[index] = Task.Run(async delegate
                {
                    foreach (var add in groups[index])
                        await list.Add(add.ToString());
                });

            Task.WaitAll(tasks);

            i = 0;
            groups = (from item in Enumerable.Range(2, n - 2)
                group item by i++ % 8
                into part
                select part.AsEnumerable()).ToList();

            foreach (var index in Enumerable.Range(0, k))
                tasks[index] = Task.Run(async delegate
                {
                    foreach (var del in groups[index])
                        await list.Del(del.ToString());
                });

            Task.WaitAll(tasks);

            Assert.AreEqual($"1,{n},", list.ToString());
        }
    }
}