using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Bountyhunt;
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

        [Test]
        public void ModeRotationTest()
        {
            // 6 total rounds to play
            var modeRotationList = new List<ModeRotationItem>(){
                new ModeRotationItem("1", 10),
                new ModeRotationItem("2", 10)
            };
            int repetitions = 3;


            // Test counter
            int index = 0;
            var item = Utility.GetCurrentRound(modeRotationList, index);
            Assert.AreEqual("1", item.GamemodeId);
            index = 1;
            item = Utility.GetCurrentRound(modeRotationList, index);
            Assert.AreEqual("2", item.GamemodeId);
            index = 2;
            item = Utility.GetCurrentRound(modeRotationList, index);
            Assert.AreEqual("1", item.GamemodeId);
            index = 5;
            item = Utility.GetCurrentRound(modeRotationList, index);
            Assert.AreEqual("2", item.GamemodeId);

            // Test Finished
            var finished = Utility.roationHasFinished(modeRotationList, repetitions, index);
            Assert.False(finished);
            index++;
            finished = Utility.roationHasFinished(modeRotationList, repetitions, index);
            Assert.True(finished);
        }
        
    }
}
