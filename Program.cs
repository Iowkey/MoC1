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
        static double[,] cipherTable = new double[20, 20];

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

            for(int k = 0; k < 20; k++)
            {
                for(int i = 0; i < 20; i++)
                {
                    for(int j = 0; j < 20; j++)
                    {
                        if(cipherTable[i, j] == k)
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

            for(int m = 0; m < 20; m++)
            {
                for(int c = 0; c < 20; c++)
                {
                    for(int k = 0; k < 20; k++)
                    {
                        if(cipherTable[k, m] == c)
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

            for(int m = 0; m < 20; m++)
            {
                for(int c = 0; c < 20; c++)
                {
                    conditionalDistribution[m, c] = jointDistribution[m, c] / ciphertexrDistribution[c];
                }
            }

            return conditionalDistribution;
        }

        static int BayesianDeterministicDecisionFunction(int ciphertext)
        {
            double[] ciphertextDistribution = CountCiphertextDistribution();
            double[,] jointDistribution = CountJointDistribution();
            double[,] conditionalDistribution = CountConditionalDistribution(ciphertextDistribution, jointDistribution);
            int message;
            double maxProbability = FindMaxValue(conditionalDistribution, ciphertext, out message);

            return message;
        }

        static void Main(string[] args)
        {
            
        }
    }
}
