using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;

using crypcy.shared;

namespace crypcy.core
{
    class Program
    {
        static List<Blockchain> blockchains = new List<Blockchain>();
        static string chainName;
        static string fileName;
        static string filePath;
        static string dirPath;
        static string jsonString;

        static void Main(string[] args)
        {

            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23555);
            Peer peer = new Peer(serverEndpoint, "LocalTest");
            peer.ConnectOrDisconnect();

            fileName = "blockchains.json";
            dirPath = Path.Combine(Environment.CurrentDirectory, @"Data\");
            filePath = Path.Combine(dirPath, fileName);
            if (File.Exists(filePath))
            {
                jsonString = File.ReadAllText(filePath);

                if (new FileInfo(filePath).Length != 0)
                    blockchains = JsonSerializer.Deserialize<List<Blockchain>>(jsonString);
                else
                    System.Console.WriteLine("Блокчейнов не найдено");
            }
            else
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                File.CreateText(filePath);
            }

            System.Console.WriteLine("Введите 'exit' чтобы выйти");

            System.Console.WriteLine("Введите комманду");
            System.Console.WriteLine("1: Подключиться к серверу");
            System.Console.WriteLine("2: Подключиться к пиру");
            System.Console.WriteLine("3: Создать цепочку");
            System.Console.WriteLine("4: Добавить блок в цепочку");
            System.Console.WriteLine("5: Просмотреть цепочку");
            System.Console.WriteLine("6: Список цепочек");

            if (Console.ReadLine().ToUpper() == "EXIT")
            {
                Console.WriteLine("Shutting down...");
                Environment.Exit(0);
            }
            else
            {
                bool done = false;

                do
                {
                    string strSelection = Console.ReadLine();
                    int caseSwitch;

                    try
                    {
                        caseSwitch = int.Parse(strSelection);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("\r\nWhat?\r\n");
                        continue;
                    }
                    Console.WriteLine("Вы выбрали:  " + caseSwitch);

                    switch (caseSwitch)
                    {
                        case 1:
                            Console.WriteLine("Подключение к серверу.");
                            peer.ConnectOrDisconnect();
                            break;
                        case 2:
                            Console.WriteLine("Отправить сообщение:");
                            Console.WriteLine("Введите сообщение:");
                            string messageText = Console.ReadLine();
                            Console.WriteLine("Введите адресат:");
                            string sendTo = Console.ReadLine();
                            string sendIP = Console.ReadLine();
                            int sendPort =  int.Parse(Console.ReadLine()); 
                            Message M = new Message(peer.LocalPeerInfo.Name, sendTo, messageText);
                            peer.SendMessageUDP(M,new IPEndPoint(IPAddress.Parse(sendIP), sendPort));
                            break;
                        case 3:
                            Console.WriteLine("Блокчейн:");
                            Console.WriteLine("Введите имя цепочки :");
                            chainName = Console.ReadLine();
                            Blockchain blockchain = new Blockchain(chainName);
                            blockchains.Add(blockchain);
                            // save chain
                            jsonString = JsonSerializer.Serialize(blockchains);
                            File.WriteAllText(filePath, jsonString);
                            Console.WriteLine(File.ReadAllText(filePath));
                            continue;
                        case 4:
                            Console.WriteLine("Добавить блок:");
                            Console.WriteLine("Введите имя цепочки :");
                            chainName = Console.ReadLine();
                            Console.WriteLine("Введите данные для блока:");
                            string blockData = Console.ReadLine();

                            blockchains.Where(b => b.BlockchainName == chainName).ToList().ForEach(b => b.AddBlock(new Block(b.GetLatestBlock().Hash, blockData)));

                            jsonString = JsonSerializer.Serialize(blockchains);
                            File.WriteAllText(filePath, jsonString);
                            Console.WriteLine(File.ReadAllText(filePath));
                            break;
                        case 5:
                            Console.WriteLine("Введите имя цепочки :");
                            chainName = Console.ReadLine();
                            System.Console.WriteLine();

                            Blockchain selectedBlockchain = blockchains.First(x => x.BlockchainName == chainName);

                            foreach (Block b in selectedBlockchain.Chain)
                            {
                                System.Console.WriteLine($"Index: {b.Index}");
                                System.Console.WriteLine($"PreviousHash: {b.PreviousHash}");
                                System.Console.WriteLine($"Hash: {b.Hash}");
                                System.Console.WriteLine($"Data: {b.Data}");
                                System.Console.WriteLine("-------------------");
                            }
                            break;
                        case 6:
                            Console.WriteLine("Список цепочек:");
                            foreach (Blockchain b in blockchains)
                            {
                                System.Console.WriteLine(b.BlockchainName);
                            }
                            break;
                        case 0:
                            done = true;
                            break;
                        default:
                            Console.WriteLine("Комманда не распознанна, введите заного:\r", caseSwitch);
                            continue;
                    }
                }
                while (!done);
                System.Console.WriteLine("Пока");
            }
        }
    }
}
