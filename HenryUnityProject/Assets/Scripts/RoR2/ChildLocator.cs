using UnityEngine;
using System;

public class ChildLocator : MonoBehaviour
{
	[Serializable]
	private struct NameTransformPair
	{
		public string name;
		public Transform transform;
	}

	[SerializeField]
	private NameTransformPair[] transformPairs;
}
