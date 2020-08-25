using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
namespace Tests
{
    public class UtilityTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void UtilityTestsSimplePasses()
        {
            long playerBounty = 100;
            double dropPercentage = 0.5;
            long expectedNew = 50;
            long expectedDrop = 50;
            var res = Utility.GetBountyDropInfo(playerBounty, dropPercentage);
            Assert.AreEqual(expectedNew, res.newBounty);
            Assert.AreEqual(expectedDrop, res.dropBounty);

            playerBounty = 101;
            dropPercentage = 0.5;
            expectedNew = 50;
            expectedDrop = 51;
            res = Utility.GetBountyDropInfo(playerBounty, dropPercentage);
            Assert.AreEqual(expectedNew, res.newBounty);
            Assert.AreEqual(expectedDrop, res.dropBounty);


            playerBounty = 2;
            dropPercentage = 0.05;
            expectedNew = 1;
            expectedDrop = 1;
            res = Utility.GetBountyDropInfo(playerBounty, dropPercentage);
            Assert.AreEqual(expectedNew, res.newBounty);
            Assert.AreEqual(expectedDrop, res.dropBounty);
        }

        
    }
}
