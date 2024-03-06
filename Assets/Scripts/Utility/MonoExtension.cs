using System.Collections.Generic;
using UnityEngine;

namespace Utility {
	public interface IMonoExtension {
		
			public interface ITransformLookup {
				public Dictionary<string, Transform> TransformsLookup { get; set; }

				public void InitializeLookup(Transform source);

				public Transform GetChild(string childName);
				
			}
		// public abstract class TransformLookup {
		// 	
		// }

	}
}