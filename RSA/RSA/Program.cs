using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Encrypt");
            Console.WriteLine("2. Decrypt");
            Console.WriteLine("3. Quit");
            int option = int.Parse(Console.ReadLine());

            if (option == 1)
            {
                Console.WriteLine("Enter two prime numbers p and q:");
                BigInteger p = BigInteger.Parse(Console.ReadLine());
                BigInteger q = BigInteger.Parse(Console.ReadLine());
                BigInteger n = p * q;
                BigInteger phi = (p - 1) * (q - 1);

                BigInteger e = FindPublicExponent(phi);

                Console.WriteLine("Enter plaintext:");
                string plaintext = Console.ReadLine();

                BigInteger[] encrypted = EncryptString(plaintext, e, n);

                SaveCiphertextAndPublicKey(encrypted, n, e);

                Console.WriteLine("Encryption complete.");
            }
            else if (option == 2)
            {
                Tuple<BigInteger[], BigInteger, BigInteger> ciphertextAndPublicKey = ReadCiphertextAndPublicKey();

                BigInteger[] encrypted = ciphertextAndPublicKey.Item1;
                BigInteger n = ciphertextAndPublicKey.Item2;
                BigInteger e = ciphertextAndPublicKey.Item3;

                Console.WriteLine("p and q are required for decryption.");
                Console.WriteLine("Enter prime number p:");
                BigInteger p = BigInteger.Parse(Console.ReadLine());
                BigInteger q = n / p;

                BigInteger phi = (p - 1) * (q - 1);
                BigInteger d = ModInverse(e, phi);

                string decrypted = DecryptString(encrypted, d, n);

                Console.WriteLine("Decrypted plaintext: " + decrypted);

                SaveDecryptedText(decrypted);
            }
            else if (option == 3)
            {
                Console.WriteLine("Exiting program...");
                break;
            }
            else
            {
                Console.WriteLine("Invalid option.");
            }
        }
    }

    static BigInteger FindPublicExponent(BigInteger phi)
    {
        BigInteger e = 3;

        while (e < phi)
        {
            if (BigInteger.GreatestCommonDivisor(e, phi) == 1)
            {
                return e;
            }

            e += 2;
        }

        return GenerateRandomPrime(3, (int)phi);
    }

    static Random random = new Random();

    static BigInteger GenerateRandomPrime(int min, int max)
    {
        while (true)
        {
            BigInteger exponent = GenerateRandomBigInteger(min, max);
            if (IsPrime(exponent))
            {
                return exponent;
            }
        }
    }

    static BigInteger GenerateRandomBigInteger(BigInteger min, BigInteger max)
    {
        byte[] bytes = max.ToByteArray();
        BigInteger result;
        do
        {
            random.NextBytes(bytes);
            bytes[bytes.Length - 1] &= (byte)0x7F;
            result = new BigInteger(bytes);
        } while (result < min || result > max);
        return result;
    }

    static bool IsPrime(BigInteger number)
    {
        if (number <= 1)
        {
            return false;
        }
        if (number <= 3)
        {
            return true;
        }
        if (number % 2 == 0 || number % 3 == 0)
        {
            return false;
        }
        for (BigInteger i = 5; i * i <= number; i += 6)
        {
            if (number % i == 0 || number % (i + 2) == 0)
            {
                return false;
            }
        }
        return true;
    }

    static BigInteger[] EncryptString(string plaintext, BigInteger e, BigInteger n)
    {
        BigInteger[] encrypted = new BigInteger[plaintext.Length];

        for (int i = 0; i < plaintext.Length; i++)
        {
            encrypted[i] = BigInteger.ModPow(plaintext[i], e, n);
        }

        return encrypted;
    }

    static string DecryptString(BigInteger[] encrypted, BigInteger d, BigInteger n)
    {
        StringBuilder decrypted = new StringBuilder();

        foreach (BigInteger enc in encrypted)
        {
            char decryptedChar = (char)BigInteger.ModPow(enc, d, n);
            decrypted.Append(decryptedChar);
        }

        return decrypted.ToString();
    }

    static void SaveCiphertextAndPublicKey(BigInteger[] encrypted, BigInteger n, BigInteger e)
    {
        using (StreamWriter writer = new StreamWriter("C:\\Users\\rolan\\source\\repos\\RSA\\RSA\\keys.txt", false))
        {
            foreach (BigInteger enc in encrypted)
            {
                writer.WriteLine(enc);
            }
            writer.WriteLine(n);
            writer.WriteLine(e);
        }
    }

    static Tuple<BigInteger[], BigInteger, BigInteger> ReadCiphertextAndPublicKey()
    {
        using (StreamReader reader = new StreamReader("C:\\Users\\rolan\\source\\repos\\RSA\\RSA\\keys.txt"))
        {
            List<BigInteger> encrypted = new List<BigInteger>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                encrypted.Add(BigInteger.Parse(line));
            }
            BigInteger n = encrypted[encrypted.Count - 2];
            BigInteger e = encrypted[encrypted.Count - 1];
            encrypted.RemoveAt(encrypted.Count - 1);
            encrypted.RemoveAt(encrypted.Count - 1);

            return Tuple.Create(encrypted.ToArray(), n, e);
        }
    }

    static BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        BigInteger m0 = m;
        BigInteger y = 0, x = 1;

        if (m == 1)
            return 0;

        while (a > 1)
        {
            BigInteger q = a / m;

            BigInteger t = m;

            m = a % m;
            a = t;
            t = y;

            y = x - q * y;
            x = t;
        }

        if (x < 0)
            x += m0;

        return x;
    }

    static void SaveDecryptedText(string decryptedText)
    {
        using (StreamWriter writer = new StreamWriter("C:\\Users\\rolan\\source\\repos\\RSA\\RSA\\PlainText.txt"))
        {
            writer.WriteLine(decryptedText);
        }
    }
}
