using System;
using System.Collections;
using System.Collections.Generic;
using Lancher.Game.Stage.Slot;

using UnityEngine;
using UnityEngine.UIElements;
using static   RunTime.Config.Define.Main3Static;
namespace UI
{
	public class Main : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			Test();
            var main2 = new GameObject("main2", typeof(Main2) );

			var main2Com = main2.GetComponent<Lancher.Game.Stage.Slot.Main2>();
		}

		// Update is called once per frame
		void Update()
		{
			
		}
	}
}

