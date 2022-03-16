using System.Threading;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace decimoor
{
    class Program
    {
        static public DateTime time = DateTime.Now;
        static public string directory = "someFolder";
        static public string letters = "abcdefghijklmnopqrstuvwxyz";
        static public Dictionary<int, string[]> dic = new Dictionary<int, string[]>
    {
      {1, new string[1]{letters} },
      {2, new string[2]{ "abcdefghijklmn", "opqrstuvwxyz" } },
      {3, new string[4]{ "abcdef", "ghijklm", "nopqrst", "uvwxyz"} },
      {4, new string[8]{ "abc", "def", "ghi", "jkl", "mno", "pqr", "stu", "vwxyz" } }
    };

        static public void Main()
        {
            SHA256 sha256 = SHA256.Create();

            byte[] origin = ReadHash();
            int mode = ReadMode();

            Thread[] threads = new Thread[Convert.ToInt32(Math.Pow(2, mode - 1))];

            for (int i = 0; i < threads.Length; i++)
            {
                ThreadInfo threadInfo = new ThreadInfo(mode, i, origin);
                threads[i] = new Thread(threadInfo.StartBruteForce);
                threads[i].Name = $"{i + 1}";
                threads[i].Start();
            }
        }
        record class ThreadInfo(int mode, int thread, byte[] origin)
        {
            public void StartBruteForce()
            {
                string[] parts = dic[mode];
                string part = parts[thread];

                SHA256 sha256 = SHA256.Create();
                string toCompare = "AAAAA";
                foreach (char letter in part)
                {
                    foreach (char letter2 in letters)
                    {
                        foreach (char letter3 in letters)
                        {
                            foreach (char letter4 in letters)
                            {
                                foreach (char letter5 in letters)
                                {
                                    char[] chars = { letter, letter2, letter3, letter4, letter5 };
                                    toCompare = new string(chars);
                                    byte[] newHashValues = sha256.ComputeHash(Encoding.ASCII.GetBytes(toCompare));
                                    if (CompareHashes(newHashValues, origin))
                                    {
                                        int t = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - time.Second - time.Minute * 60 - time.Hour * 3600;
                                        Console.WriteLine($"The password is: {toCompare}, thread's name: {Thread.CurrentThread.Name}, BruteForce took {t} seconds");
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        static public int ReadMode()
        {
            int mode = 0;
            bool right = true;
            do
            {
                right = true;
                Console.WriteLine("Введите режим работы:\n 1 - однопоточный\n 2 - двухпоточный\n 3 -- четырехпоточный\n 4 - восьмипоточный");
                Console.Write("Режим работы: ");
                mode = Convert.ToInt32(Console.ReadLine());
                if (mode > 4 || mode < 0)
                {
                    Console.WriteLine("Вы ввели неверный режим работы, попробуйте заново");
                    right = false;
                }

            }
            while (!right);

            return mode;
        }
        static public byte[] ReadHash()
        {
            byte[] hash = new byte[32];
            string strHash = "";
            bool right = true;
            do
            {
                right = true;
                Console.WriteLine("Введите хэш: ");
                strHash = Console.ReadLine();
                if (strHash.Length != 64)
                {
                    Console.WriteLine("Хэш код должен содержать 64 шестнадцатиричных цифр");
                    right = false;
                }
            }
            while (!right);
            return ConvertHexStringToByte(strHash);
        }
        public static void PrintByteArray(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine($"{array[i]:X2}");
                if ((i % 4) == 3) Console.WriteLine(" ");
            }
            Console.WriteLine();
        }
        public static byte[] ConvertHexStringToByte(string hexSequence)
        {
            var gg = new Dictionary<char, byte>
      {
        { '0', 0},
        { '1', 1},
        { '2', 2},
        { '3', 3},
        { '4', 4},
        { '5', 5},
        { '6', 6},
        { '7', 7},
        { '8', 8},
        { '9', 9},
        { 'a', 10},
        { 'b', 11},
        { 'c', 12},
        { 'd', 13},
        { 'e', 14},
        { 'f', 15}
       };
            byte[] toReturn = new byte[32];
            int index = 0;
            for (int i = 0; i < hexSequence.Length; i += 2)
            {
                int number = gg[hexSequence[i]] * 16 + gg[hexSequence[i + 1]];
                toReturn[index++] = (byte)number;
            }
            return toReturn;
        }
        public static bool CompareHashes(byte[] valueToCompare, byte[] value)
        {
            for (int i = 0; i < valueToCompare.Length; i++)
            {
                if (valueToCompare[i] != value[i])
                    return false;
            }
            return true;
        }
    }
}