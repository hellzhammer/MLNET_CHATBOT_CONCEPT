using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace PerleyMLNeuralNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private int[] layers;//layers    
        private float[][] neurons;//neurons    
        private float[][] biases;//biasses    
        private float[][][] weights;//weights    
        private int[] activations;//layers
        public float fitness = 0;//fitness
        public void NeuralNetwork(int[] layers)
        {        
            this.layers = new int[layers.Length];        
            for (int i = 0; i < layers.Length; i++)        
            {            
                this.layers[i] = layers[i];        
            }        
            InitNeurons();        
            InitBiases();        
            InitWeights();    
        }

        //create empty storage array for the neurons in the network.
private void InitNeurons()
{        
  List<float[]> neuronsList = new List<float[]>();        
  for (int i = 0; i < layers.Length; i++)        
  {            
    neuronsList.Add(new float[layers[i]]);        
  }        
  neurons = neuronsList.ToArray();    
}

//initializes and populates array for the biases being held within the network.
private void InitBiases()    
{        
  List<float[]> biasList = new List<float[]>();        
  for (int i = 0; i < layers.Length; i++)        
  {            
    float[] bias = new float[layers[i]];            
    for (int j = 0; j < layers[i]; j++)            
    {                
      bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);            
    }            
    biasList.Add(bias);        
  }        
  biases = biasList.ToArray();    
}

//initializes random array for the weights being held in the network.
private void InitWeights()   
{        
  List<float[][]> weightsList = new List<float[][]>();        
  for (int i = 1; i < layers.Length; i++)        
  {            
    List<float[]> layerWeightsList = new List<float[]>();   
    int neuronsInPreviousLayer = layers[i - 1];            
    for (int j = 0; j < neurons[i].Length; j++)            
    {                 
      float[] neuronWeights = new float[neuronsInPreviousLayer];
      for (int k = 0; k < neuronsInPreviousLayer; k++)  
      {                                      
        neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f); 
      }               
      layerWeightsList.Add(neuronWeights);            
    }            
    weightsList.Add(layerWeightsList.ToArray());        
  }        
  weights = weightsList.ToArray();    
}
    }
}
