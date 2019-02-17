using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MessagesCreator
{
    public class Program
    {
        #region Fields

        private const string MessagesFolderPath = "../../../../../Helden.Common/Network/Protocol/Messages";
        private const string MessageIdPattern = @"public const short MessageId = (\d);";
        private const string NamespaceStart = "Helden.Common.Network.Protocol.Messages.";

        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine("Messages Creator v0.1");
            Console.WriteLine($"Messages folder: {MessagesFolderPath}");

            do
            {
                // Want to create a message?
                short nextMessageId = GetNextMessageId();
                Console.Write($"Would you like to create message N°{nextMessageId}? (y/n)");
                var answer = Console.ReadKey().Key;
                Console.WriteLine();

                if (answer != ConsoleKey.Y)
                    break;

                (string content, string filename) = CreateMessage(nextMessageId);
                SaveMessage(content, filename);

                Console.WriteLine("Message saved!");
                Console.WriteLine("---------------------------------------------------");
            }
            while (true);

            Console.WriteLine("Press any key to exit..");
            Console.ReadKey();
        }

        #region Private Methods

        private static void SaveMessage(string content, string filename)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.WriteAllText(filename, content);
        }

        private static (string, string) CreateMessage(short id)
        {
            string name = GetMessageName();
            (string @namespace, string fullNamespace) = GetMessageNamespaces();

            var properties = GetMessageProperties();
            if (properties == null)
                return (null, null);

            var sb = new StringBuilder();

            // Usings
            sb.AppendLine("using System;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine();

            // Namespace - Start
            sb.AppendLine($"namespace {fullNamespace}");
            sb.AppendLine("{");

            // Class - Start
            sb.AppendLine($"\tpublic class {name} : IMessage");
            sb.AppendLine("\t{\n");

            // Message Id
            sb.AppendLine($"\t\tpublic const short MessageId = {id};");
            sb.AppendLine("\t\tpublic short Id => MessageId;");

            // Properties
            sb.AppendLine(string.Join("\n", properties.Select(p => p.ToPropertyLine)));
            sb.AppendLine();

            // Constructor
            sb.AppendLine($"\t\tpublic {name}() {{ }}");
            sb.AppendLine();

            // Serialize
            sb.AppendLine("\t\tpublic void Serialize(BinaryWriter writer)");
            sb.AppendLine("\t\t{");
            sb.AppendLine(string.Join("\n", properties.Select(p => p.ToWriteLine)));
            sb.AppendLine("\t\t}");
            sb.AppendLine();

            // Deserialize
            sb.AppendLine("\t\tpublic void Deserialize(BinaryReader reader)");
            sb.AppendLine("\t\t{");
            sb.AppendLine(string.Join("\n", properties.Select(p => p.ToReadLine)));
            sb.AppendLine("\t\t}");
            sb.AppendLine();

            // Class - End
            sb.AppendLine("\t}");

            // Namespace - End
            sb.AppendLine("}");

            return (sb.ToString(), $"{MessagesFolderPath}/{@namespace.Replace('.', '/')}/{name}.cs");
        }

        private static string GetMessageName()
        {
            Console.WriteLine("---------------------------------------------------");
            Console.Write("What is the name of the message? ");
            return Console.ReadLine();
        }

        private static (string @namespace, string fullNamespace) GetMessageNamespaces()
        {
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("What do you want the namespace of this message to be?");
            Console.Write(NamespaceStart);

            string answer = Console.ReadLine();
            string fullNamespace = NamespaceStart + answer;
            Console.WriteLine($"Namespace chosen: {fullNamespace}");

            return (answer, fullNamespace);
        }

        private static List<Property> GetMessageProperties()
        {
            var properties = new List<Property>();

            // Message's properties
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Please write the properties ([type] [name]).");
            Console.WriteLine("When you're done, type \"done\".");
            Console.WriteLine("If you want to cancel the operation, type \"cancel\".");

            do
            {
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (input.ToLower() == "done")
                    break;

                if (input.ToLower() == "cancel")
                    return null;

                var prop = input.Split(' ');

                // Invalid property
                if (prop.Length != 2)
                {
                    Console.WriteLine("Invalid property syntax.");
                    continue;
                }

                properties.Add(new Property
                {
                    Name = prop[1],
                    Type = prop[0]
                });
                Console.WriteLine($"Property \"{prop[1]}\" of type \"{prop[0]}\" added!");
            }
            while (true);

            return properties;
        }

        private static short GetNextMessageId()
        {
            var messagesDir = new DirectoryInfo(MessagesFolderPath);
            if (!messagesDir.Exists)
                throw new Exception("Messages folder doesn't exist");

            short maxId = -1;
            foreach (var file in messagesDir.GetFiles("*.cs", SearchOption.AllDirectories))
            {
                string content = File.ReadAllText(file.FullName);
                var reg = Regex.Match(content, MessageIdPattern);
                if (reg.Success)
                {
                    short id = short.Parse(reg.Groups[1].Value);
                    if (id > maxId)
                        maxId = id;
                }
            }

            return (short)(maxId + 1);
        }

        #endregion
    }
}