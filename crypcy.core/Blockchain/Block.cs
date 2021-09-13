using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class Block
{
        public int Index { get; set; }
        public DateTime TimeStamp { get; set; }
        public string PreviousHash { get; set; }
        public string Hash { get; set; }
        public string Data { get; set; }     


        public Block(string previousHash, string data)
        {
            Index = 0;
            TimeStamp = DateTime.Now;
            PreviousHash = previousHash;
            Data = data.ToString();
            Hash = CalculateHash();
        }



        public  string GetStringForHash()
        {

            var data = "";
            data += TimeStamp;
            data += PreviousHash;
            data += Data;

            return data;
        }


        public string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();
            string data = GetStringForHash();

            byte[] inputBytes = Encoding.ASCII.GetBytes(data);
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

}