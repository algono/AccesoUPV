using System;
using AccesoUPV.Library.Connectors.Drive;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AccesoUPV.UnitTests.NetworkDrives.DriveLetter
{
    [TestClass]
    public class DriveLetterTests
    {
        private static char GenerateRandomLetter()
        {
            Random random = new Random();
            int offset = random.Next(0, 26);
            return (char)('A' + offset);
        }

        private static char GenerateRandomNonLetter()
        {
            char res;
            Random random = new Random();
            
            do
            {
                res = (char) random.Next(char.MinValue, char.MaxValue);
            }
            while (char.IsLetter(res));

            return res;
        }

        [TestMethod]
        public void ToDriveLetter_Valid()
        {
            char letter = GenerateRandomLetter();
            string driveLetter = DriveLetterTools.ToDriveLetter(letter);
            Assert.AreEqual(letter + ":", driveLetter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToDriveLetter_NotALetter()
        {
            char nonLetter = GenerateRandomNonLetter();
            DriveLetterTools.ToDriveLetter(nonLetter);
        }

        [TestMethod]
        public void FromDriveLetter_Valid()
        {
            string driveLetter = GenerateRandomLetter() + ":";
            char letter = DriveLetterTools.FromDriveLetter(driveLetter);
            Assert.AreEqual(driveLetter, letter + ":");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromDriveLetter_NotALetter()
        {
            string driveLetter = GenerateRandomNonLetter() + ":";
            DriveLetterTools.FromDriveLetter(driveLetter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FromDriveLetter_InvalidPattern()
        {
            DriveLetterTools.FromDriveLetter("drive123");
        }
    }
}
