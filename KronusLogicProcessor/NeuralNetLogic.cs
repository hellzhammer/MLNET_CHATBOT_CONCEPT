using System;
using MathNet.Numerics.LinearAlgebra;
using System.IO;

namespace KronusLogicProcessor
{
    //3 Input - 1 Output

    //input/model types:
    /*

       <--0-->
        greeting = bool (t(1),f(0))

        <--1-->
        greeting types:
        0-disjunctive question greeting               
        1-general question greeting
        2-statement greeting

        <--2-->               
        greeting sub types:
        HangOut 0-hangout.
        HowAreYou 1-how are you?
        WhatsUp 2-whats up?
        Hey 3-Hey.

        --3-pretty good, you?       
        --4-not much, you?

        [outputs]
        responses:
        0-sorry i can't.
        1-not bad, you?
        2-not much, you?
        3-hey, how are you?

        4-not much.
        5-i am good.

        array layout:
        {0,1,2}               
    */

    public class NeuralNetworkBuilder
    {
        public NeuralNetwork neuralNetwork { get; set; }
        public void NetworkInit(double[,] traingingInputs = null, double[,] trainingOutputs = null)
        {


            if (traingingInputs == null)
            {
                traingingInputs = new double[,] { { 1, 0, 2},
                                                            { 1, 1, 1},
                                                            { 1, 1, 2},
                                                            { 1, 0, 0}};
            }

            if (trainingOutputs == null)
            {
                trainingOutputs = new double[,] { {0,1,0,0},
                                                             {0,1,0,0},
                                                             {0,0,1,0},
                                                             {1,0,0,0}};
            }

            neuralNetwork = new NeuralNetwork();


            Console.WriteLine("Start training ...");

            // Train the neural network using a training set.
            // Do it 25,000 times
            neuralNetwork.TrainNetwork(traingingInputs, trainingOutputs, 25000);
            Console.WriteLine("End training ...\n\n");

            // Predict
            Console.WriteLine("Considering new situation [1, 1, 3] -> ?\n");
            var result = neuralNetwork.Evaluate(new double[,] { { 1, 1, 3 } });

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }

    public class NeuralNetwork
    {
        Matrix<double> Weights;

        public NeuralNetwork()
        {
            Weights = BuildPredictionMatrix(3, 4);
        }

        private Matrix<double> SigmoidMethod(Matrix<double> x, bool derivative)
        {
            if (derivative)
            {
                return x.PointwiseMultiply(1 - x);
            }
            else
            {
                return 1 / (1 + (1 / x.PointwiseExp()));
            }
        }

        public void TrainNetwork(double[,] InputData, double[,] OutputData, int IterationCount)
        {
            var mInput = MatrixArray(InputData);
            var mOutput = MatrixArray(OutputData);

            for (int i = 0; i < IterationCount; i++)
            {
                var output = Evaluate(InputData);
                var error = mOutput - output;
                var adjustment = dotProduct(mInput.Transpose(), error.PointwiseMultiply(SigmoidMethod(output, true)));
                Weights += adjustment;
            }
        }

        public Matrix<double> Evaluate(double[,] inputs)
        {
            var minputs = MatrixArray(inputs);

            return SigmoidMethod(dotProduct(minputs, Weights), false);
        }

        private Matrix<double> dotProduct(Matrix<double> M1, Matrix<double> M2)
        {
            return M1 * M2;
        }

        private Matrix<double> BuildPredictionMatrix(int Rows, int Columns)
        {
            Random r = new Random();
            double[,] m = new double[Rows, Columns];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    double max = 1.0;
                    double min = -1.0;

                    double Scores = r.NextDouble() * (max - min) + min;
                    m[i, j] = Scores;
                }
            }

            return Matrix<double>.Build.DenseOfArray(m);
        }

        private Matrix<double> MatrixArray(double[,] array)
        {
            return Matrix<double>.Build.DenseOfArray(array);
        }
    }
}
