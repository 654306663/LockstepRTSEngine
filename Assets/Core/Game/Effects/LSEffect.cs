﻿using UnityEngine;
using System.Collections; using FastCollections;
using System;
using RTSLockstep.Data;
namespace RTSLockstep
{
	public class LSEffect : MonoBehaviour
	{

		public float Duration {get; set;}

		#region For external use
		//TODO: Add needed variables for external mechanisms (I.e. Sinus).
		#endregion

		#region Default Values
		Vector3 defaultScale;
		[SerializeField]
		private float defaultDuration;

		#endregion

		public string MyEffectCode { get; private set; }
		public Transform CachedTransform {get; private set;}
		public GameObject CachedGameObject {get; private set;}
        public Vector3 StartPos {get; set;}
        public Vector3 EndPos {get; set;}
		public Transform Target { get; set;}


		public ParticleSystem CachedShuriken {get; private set;}
        private ParticleSystem.MinMaxCurve StartSpeed {
            get; set;
        }
        public float Speed {
            set {
                var main = CachedShuriken.main;
                main.startSpeed = value;
            }
        }
        private ParticleSystem.MinMaxCurve StartSize {get; set;}
        public float Size {
            set {
                var main = CachedShuriken.main;
                main.startSize = value;
            }
        }


		public int ID { get; private set; }

		public event Action OnSetup;
		public event Action OnInitialize;
		public event Action OnLateInitialize;
		public event Action OnVisualize;
		public event Action OnDeactivate;
		/// <summary>
		/// Called when this effect is first created.
		/// </summary>
		/// <param name="myEffectCode">My effect code.</param>
        internal void Setup (string myEffectCode)
		{
			MyEffectCode = myEffectCode;
			CachedTransform = base.transform;
			CachedGameObject = base.gameObject;
			CachedShuriken = base.gameObject.GetComponent<ParticleSystem>();
			if(CachedShuriken){
                var main = CachedShuriken.main;
                StartSpeed = main.startSpeed;
                StartSize = main.startSize;
			}
			if (OnSetup .IsNotNull ())
				OnSetup ();
			GameObject.DontDestroyOnLoad (CachedGameObject);

			defaultScale = CachedTransform.localScale;
		}

		/// <summary>
		/// For internal use.
		/// </summary>
		/// <param name="id">Identifier.</param>
        internal bool Create (int id)
		{
			if (CachedGameObject == null)
			{
				EffectManager.DestroyEffect (this);
				return false;
			}
			ID = id;

			//Reset variables
			Duration = defaultDuration;
			CachedTransform.localScale = defaultScale;

			return true;
		}

		/// <summary>
		/// Call this when all parameters are supplied as desired.
		/// </summary>
		internal void Initialize ()
		{
			CachedGameObject.SetActive (true);
			StartCoroutine (LifeTimer ());
			if (OnInitialize .IsNotNull ())
				OnInitialize ();

            if (CachedShuriken != null) {
                var main = CachedShuriken.main;
                main.startSpeed = StartSpeed;
                main.startSize = StartSize;
                CachedShuriken.Play();
            }

			lateInitialized = false;
		}

		bool lateInitialized = false;
		/// <summary>
		/// Called every Update frame.
		/// </summary>
        internal void Visualize ()
		{
			if (lateInitialized == false) {
				lateInitialized = true;
				if (OnLateInitialize != null)
					OnLateInitialize ();
			}
			if (OnVisualize .IsNotNull ()) 
				OnVisualize ();
		}
			
		/// <summary>
		/// Called when this effect is deactivated. Perform resets here.
		/// </summary>
        internal void Deactivate ()
		{
            if (CachedShuriken != null) {
                CachedShuriken.Stop();
            }
			if (CachedTransform != null) {
				CachedTransform.SetParent(null);
			}

			if (CachedGameObject != null) {
				CachedGameObject.SetActive(false);
			}

			if (OnDeactivate .IsNotNull ())
				OnDeactivate ();
		}

		IEnumerator LifeTimer ()
		{
			//yield return Duration;
			yield return new WaitForSeconds(Duration);
			EffectManager.EndEffect (this);
		}
	}
}