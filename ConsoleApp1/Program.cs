using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;



public class Program
{
    public static bool BlumCheck(BigInteger a, BigInteger b)
    {
        return (a % 4 == 3) && (b % 4 == 3);
    }
    private static string BitArrayToString(BitArray bits)
    {
        StringBuilder sb = new StringBuilder(bits.Length);
        foreach (bool bit in bits)
        {
            sb.Append(bit ? "1" : "0");
        }
        return sb.ToString();
    }

    public static BigInteger ModInverse(BigInteger a, BigInteger n)
    {
        BigInteger i = n, v = 0, d = 1;
        while (a > 0)
        {
            BigInteger t = i / a, x = a;
            a = i % x;
            i = x;
            x = d;
            d = v - t * x;
            v = x;
        }
        v %= n;
        if (v < 0) v = (v + n) % n;
        return v;
    }
    public static Tuple<BigInteger,string> Encrypt(string word, BigInteger p, BigInteger q, BigInteger seed)
    {
        string binaryWord = TextToBinary(word);
        BigInteger n = p * q;
        if ((BigInteger.GreatestCommonDivisor(n, seed) != 1 || !BlumCheck(p, q)))return null;
        BigInteger[] xarray = new BigInteger[binaryWord.Length];
        xarray[0] = seed;
        for (int i = 1; i < binaryWord.Length; i++)
        {
            xarray[i] = BigInteger.ModPow(xarray[i - 1], 2, n);
        }
        BitArray bbsBits = new BitArray(binaryWord.Length);
        for (int i = 0; i < binaryWord.Length; i++)
        {
            bbsBits[i] = !xarray[i].IsEven;
        }

        return new Tuple<BigInteger, string>(xarray[xarray.Length - 1], BitArrayToString(BinaryStringToBitArray(binaryWord).Xor(bbsBits)));
    }

    private static string TextToBinary(string word)
    {
        string russianAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        StringBuilder binary = new StringBuilder();

        foreach (char c in word.ToUpper())
        {
            int index = russianAlphabet.IndexOf(c);
            binary.Append(Convert.ToString(index, 2).PadLeft(5, '0'));
        }
        return binary.ToString();
    }

    private static BitArray BinaryStringToBitArray(string binary)
    {
        BitArray bitArray = new BitArray(binary.Length);
        for (int i = 0; i < binary.Length; i++)
        {
            bitArray[i] = binary[i] == '1';
        }
        return bitArray;
    }

    public static string Decrypt(BigInteger p, BigInteger q, BigInteger xm, string encryptedBinary)
    {
        int m = encryptedBinary.Length;
        BigInteger n = p * q;

        BigInteger alpha = BigInteger.ModPow((p + 1) / 4, m-1, p - 1);
        BigInteger beta = BigInteger.ModPow((q + 1) / 4, m-1, q - 1);


        BigInteger u = BigInteger.ModPow(xm, alpha, p);
        BigInteger v = BigInteger.ModPow(xm, beta, q);

        BigInteger a = ModInverse(p, q);
        BigInteger b = ModInverse(q, p);
        BigInteger x0 = (u * b * q + v * a * p) % n;

        BigInteger[] xarray = new BigInteger[m];
        xarray[0] = x0;
        for (int i = 1; i < m; i++)
        {
            xarray[i] = BigInteger.ModPow(xarray[i - 1], 2, n);
        }
        BitArray bbsBits = new BitArray(m);
        for (int i = 0; i < m; i++)
        {
            bbsBits[i] = (xarray[i] + BigInteger.Parse(encryptedBinary[i].ToString()))%2 == 1;
        }

        return BinaryToText(BitArrayToString(bbsBits));
    }
    public static string BinaryToText(string binary)
    {
        StringBuilder text = new StringBuilder();
        int length = (binary.Length / 5) * 5;

        for (int i = 0; i < length; i += 5)
        {
            string fiveBits = binary.Substring(i, 5);
            int index = Convert.ToInt32(fiveBits, 2);

            string russianAlphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            if (index >= 0 && index < russianAlphabet.Length)
            {
                text.Append(russianAlphabet[index]);
            }
        }
        return text.ToString();
    }

    public static Tuple<BigInteger, BigInteger, BigInteger, string>[] ReadFromText(string filename)// p q seed bin
    {
        var lines = File.ReadAllLines(filename).Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
        var result = new Tuple<BigInteger, BigInteger, BigInteger, string>[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            BigInteger item1 = BigInteger.Parse(parts[0]);
            BigInteger item2 = BigInteger.Parse(parts[1]);
            BigInteger item3 = BigInteger.Parse(parts[2]);
            string item4 = parts[3];

            result[i] = Tuple.Create(item1, item2, item3, item4);
        }
        return result;
    }

    static void Main()
    {

        Tuple<BigInteger, BigInteger, BigInteger, string>[] data = ReadFromText(@"C:\Users\user\source\repos\ConsoleApp1\ConsoleApp1\Text.txt");

        foreach (var item in data)
        {

                string decrypted = Decrypt(item.Item1, item.Item2, item.Item3, item.Item4);

                Console.Write(item.ToString() ,item.Item2,item.Item3,item.Item4);
                Console.WriteLine(decrypted);


        }
    }
}