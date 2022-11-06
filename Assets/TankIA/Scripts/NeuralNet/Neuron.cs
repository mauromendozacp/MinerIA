using UnityEngine;
using System.Collections;

[System.Serializable]
public class Neuron
{
	public float[] weights;
	public float bias;
	public float p; 

	public int WeightsCount
	{
		get { return weights.Length; }
	}

	public Neuron(int weightsCount, float bias, float p)
	{
		weights = new float[weightsCount];

		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = Random.Range(-1.0f, 1.0f);
		}

		this.bias = bias;
		this.p = p;
	}

	public float Synapsis(float[] input)
	{
		float a = 0;

		for (int i = 0; i < input.Length; i++)
		{
			a += weights[i] * input[i];
		}

		a += bias * weights[weights.Length - 1];

		return Sigmoid(a);
	}

	public int SetWeights(float[] newWeights, int fromId)
	{
		for (int i = 0; i < weights.Length; i++)
		{
			this.weights[i] = newWeights[i + fromId];
		}

		return fromId + weights.Length;
	}

	public float[] GetWeights()
	{
		return this.weights;
	}

	private float Sigmoid(float a)
	{
		return 1.0f / (1.0f + Mathf.Exp(-a / p));
	}
}
