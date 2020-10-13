using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MoC_1
{
    class Program
    {
        static string[] variants = { "06", "18" };
        static double[] messagesDistribution = new double[20];
        static double[] keysDistribution = new double[20];
        static int[,] cipherTable = new int[20, 20];

        static double FindMaxValue(double[,] nestedArray, int ciphertext, out int m)
        {
            double maxValue = 0;
            int row = 0;

            for (int i = 0; i < 20; i++)
            {
                if (nestedArray[i, ciphertext] > maxValue)
                {
                    maxValue = nestedArray[i, ciphertext];
                    row = i;
                }
            }
            m = row;

            return maxValue;
        }

        static double[] CountCiphertextDistribution()
        {
            double[] distribution = new double[20];

            for (int k = 0; k < 20; k++)
            {
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 20; j++)
                    {
                        if (cipherTable[i, j] == k)
                        {
                            distribution[k] += messagesDistribution[j] * keysDistribution[i];
                        }
                    }
                }
            }

            return distribution;
        }

        static double[,] CountJointDistribution()
        {
            double[,] jointDistribution = new double[20, 20];

            for (int m = 0; m < 20; m++)
            {
                for (int c = 0; c < 20; c++)
                {
                    for (int k = 0; k < 20; k++)
                    {
                        if (cipherTable[k, m] == c)
                        {
                            jointDistribution[m, c] += messagesDistribution[m] * keysDistribution[k];
                        }
                    }
                }
            }

            return jointDistribution;
        }

        static double[,] CountConditionalDistribution(double[] ciphertexrDistribution, double[,] jointDistribution)
        {
            double[,] conditionalDistribution = new double[20, 20];

            for (int m = 0; m < 20; m++)
            {
                for (int c = 0; c < 20; c++)
                {
                    conditionalDistribution[m, c] = jointDistribution[m, c] / ciphertexrDistribution[c];
                }
            }

            return conditionalDistribution;
        }

        static int BayesianDeterministicDecisionFunction(int ciphertext)
        {
            int message;

            double[] ciphertextDistribution = CountCiphertextDistribution();
            double[,] jointDistribution = CountJointDistribution();
            double[,] conditionalDistribution = CountConditionalDistribution(ciphertextDistribution, jointDistribution);
            double maxProbability = FindMaxValue(conditionalDistribution, ciphertext, out message);

            return message;
        }

        static double AverageCostsOfDeterministicFunction()
        {
            int costFunction = 0;
            double averageCosts = 0;
            Random generator = new Random();
            int key = generator.Next(20);
            double[,] jointDistribution = CountJointDistribution();

            for (int m = 0; m < 20; m++)
            {
                for (int c = 0; c < 20; c++)
                {
                    if (BayesianDeterministicDecisionFunction(c) != m)
                    {
                        costFunction = 1;
                    }
                    else
                    {
                        costFunction = 0;
                    }
                    averageCosts += jointDistribution[m, c] * costFunction;
                }
            }

            return averageCosts;
        }

        static (List<int>, double) StohasticDecisionFunction(int ciphertext)
        {
            int message;
            List<int> messages = new List<int>();
            int counter = 0;

            double[] ciphertextDistribution = CountCiphertextDistribution();
            double[,] jointDistribution = CountJointDistribution();
            double[,] conditionalDistribution = CountConditionalDistribution(ciphertextDistribution, jointDistribution);
            double maxProbability = FindMaxValue(conditionalDistribution, ciphertext, out message);

            for (int m = 0; m < 20; m++)
            {
                if (conditionalDistribution[m, ciphertext] == maxProbability)
                {
                    counter++;
                    messages.Add(m);
                }
            }

            return (messages, (double)1 / counter);
        }

      

        static string GetAllMessagesFromList(List<int> list)
        {
            string allElements = "";
            for (int i = 0; i < list.Count; i++)
            {
                allElements += list[i] + ", ";
            }

            return allElements.Remove(allElements.Length - 2);
        }

        static void Main(string[] args)
        {
            using (StreamReader reader = new StreamReader($@"C:\Users\nazar\source\repos\MoC\MoC_1\prob_{variants[0]}.csv"))
            {
                while (!reader.EndOfStream)
                {
                    messagesDistribution = reader.ReadLine().Split(',').ToList().Select(x => Convert.ToDouble(x.Replace('.', ','))).ToArray();
                    keysDistribution = reader.ReadLine().Split(',').ToArray().Select(x => Convert.ToDouble(x.Replace('.', ','))).ToArray();
                }
            }
            using (StreamReader reader = new StreamReader($@"C:\Users\nazar\source\repos\MoC\MoC_1\table_{variants[0]}.csv"))
            {
                for (int i = 0; i < 20; i++)
                {
                    int[] line = reader.ReadLine().Split(',').ToList().Select(x => Convert.ToInt32(x.Replace('.', ','))).ToArray();
                    for (int j = 0; j < 20; j++)
                    {
                        cipherTable[i, j] = line[j];
                    }
                }
            }

            for (int i = 0; i < 20; i++)
            {
                int message = BayesianDeterministicDecisionFunction(i);
                Console.WriteLine(message);
            }

            for (int i = 0; i < 20; i++)
            {
                (List<int> messages, double probability) = StohasticDecisionFunction(i);
                Console.WriteLine($"If ciphertext is {i}, you will get {GetAllMessagesFromList(messages)} as a message with probability equal to {probability}");
            }

            Console.WriteLine(AverageCostsOfDeterministicFunction());
            Console.ReadLine();
        }
    }
}
