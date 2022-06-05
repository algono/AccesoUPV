using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AccesoUPV.Library.Connectors.Drive
{
    public static class DriveLetterTools
    {
        internal const string InvalidLetterMessage = "The letter has to be an alphabetical letter (A-Z).";
        internal const string InvalidDriveLetterMessage = "The drive letter has an invalid format. It should be a letter followed by a colon.";

        public static bool IsAvailable(char letter)
        {
            if (IsValid(letter))
            {
                return !Directory.Exists(ToDriveLetter(letter));
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(letter), letter, InvalidLetterMessage);
            }
        }

        public static char ValidOrDefault(char letter)
            => IsValid(letter) ? letter : default;
        public static char ValidOrDefault(char letter, char defaultValue)
            => IsValid(letter) ? letter : defaultValue;

        public const string DriveLetterPattern = @"^[a-zA-Z]:$";

        public static bool IsValid(char letter) => char.IsLetter(letter);
        public static bool IsValid(string driveLetter)
            => Regex.IsMatch(driveLetter, DriveLetterPattern);

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

        public static string ToDriveLetter(char letter)
        {
            if (IsValid(letter))
            {
                return char.ToUpper(letter) + ":";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(letter), letter, InvalidLetterMessage);
            }

        }

        public static char FromDriveLetter(string driveLetter)
        {
            if (IsValid(driveLetter))
            {
                return driveLetter[0];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(driveLetter), driveLetter, InvalidDriveLetterMessage);
            }
        }
    }
}
