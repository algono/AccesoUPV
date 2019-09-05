using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoUPV.Library.Connectors.Drive
{
    public static class DriveLetterTools
    {
        internal const string InvalidDriveLetterMessage = "The drive letter has to be an alphabetical letter (A-Z)";

        public static bool IsAvailable(char letter)
        {
            if (IsValid(letter))
            {
                return !Directory.Exists(ToDriveLetter(letter));
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(letter), letter, InvalidDriveLetterMessage);
            }
        }

        public static char ValidOrDefault(char letter) 
            => IsValid(letter) ? letter : default; 
        public static char ValidOrDefault(char letter, char defaultValue)
            => IsValid(letter) ? letter : defaultValue; 

        public static bool IsValid(char letter) => char.IsLetter(letter);

        public static char GetFirstAvailable()
        {
            List<char> availableLetters = GetDriveLetters(onlyIfAvailable: true);
            if (availableLetters.Count == 0) throw new NotAvailableDriveException();

            return availableLetters[0];
        }

        public static List<char> GetDriveLetters(bool onlyIfAvailable = false)
        {
            List<char> letters = new List<char>();

            for (char letter = 'Z'; letter >= 'A'; letter--)
            {
                if (!onlyIfAvailable || IsAvailable(letter))
                {
                    letters.Add(letter);
                }
            }

            return letters;
        }

        public static string ToDriveLetter(char letter) => (IsValid(letter) ? char.ToUpper(letter) : '?') + ":";
        public static char FromDriveLetter(string driveLetter) => driveLetter[0];

    }
}
