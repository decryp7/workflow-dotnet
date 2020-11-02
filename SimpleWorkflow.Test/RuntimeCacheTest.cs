using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleWorkflow.Test
{
    public class DummyObject
    {
        public Guid Id = Guid.NewGuid();
    }

    [TestClass]
    public class RuntimeCacheTest
    {
        [TestMethod]
        public void AddObjectToRuntimeCache_OK()
        {
            RuntimeCache runtimeCache = new RuntimeCache();
            DummyObject dummyObject = new DummyObject();
            runtimeCache.Set("DummyObject", dummyObject);
            runtimeCache.Set("DummyObject", dummyObject);

            DummyObject cachedDummyObject = runtimeCache.Read<DummyObject>("DummyObject");
            Assert.AreSame(dummyObject, cachedDummyObject);
        }

        [TestMethod]
        public void GetNonExistingObject_GetNullObject()
        {
            RuntimeCache runtimeCache = new RuntimeCache();
            DummyObject cachedDummyObject = runtimeCache.Read<DummyObject>("DummyObject");
            Assert.IsNull(cachedDummyObject);
        }

        [TestMethod]
        public void CopyContentsOfRuntimeCache_OK()
        {
            RuntimeCache runtimeCache = new RuntimeCache();
            DummyObject dummyObject = new DummyObject();
            runtimeCache.Set("DummyObject", dummyObject);

            IReadOnlyRuntimeCache secondRuntimeCache = new RuntimeCache();
            runtimeCache.CopyTo(secondRuntimeCache);

            DummyObject cachedDummyObject = secondRuntimeCache.Read<DummyObject>("DummyObject");
            Assert.AreSame(dummyObject, cachedDummyObject);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void CopyContentsOfRuntimeCacheToSelf_InvalidOperationException()
        {
            IRuntimeCache runtimeCache = new RuntimeCache();
            DummyObject dummyObject = new DummyObject();
            runtimeCache.Set("DummyObject", dummyObject);
            runtimeCache.CopyTo(runtimeCache);
        }
    }
}