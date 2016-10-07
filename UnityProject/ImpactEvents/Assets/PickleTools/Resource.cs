using UnityEngine;
using System.Collections;

namespace PickleTools.Resource {

	public delegate void ResourceHandler(Resource resource);

	[System.Serializable]
	public class Resource {
		public event ResourceHandler OnResourceChanged;

		public string Name = "none";
		public float Amount = 0.0f;
		public float Min = 0.0f;
		public float Max = 100.0f;

		public bool IsMax{
			get { return Amount.Equals(Max); }
		}
		public bool IsMin{
			get { return Amount.Equals(Min); }
		}

		public float Percent {
			get { return Amount / Max; }
		}

		public void Expend(float amount) {
			Amount -= amount;
			if (Amount < Min) {
				Amount = Min;
			}
			if(OnResourceChanged != null){
				OnResourceChanged(this);
			}
		}

		public void Add(float amount) {
			Amount += amount;
			if (Amount > Max) {
				Amount = Max;
			}
			if (OnResourceChanged != null) {
				OnResourceChanged(this);
			}
		}
	}


}
