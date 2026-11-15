#if MM_URP
using UnityEngine;
using UnityEngine.Rendering;
using System.Reflection;
using System;
using MoreMountains.Tools;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MM Spring Volume Float URP")]
	public class MMSpringVolumeFloat_URP : MMSpringFloatComponent<Volume>
	{
		[MMInspectorGroup("Volume Settings", true, 21)]
		[Tooltip("The name of the VolumeComponent (e.g. Bloom, WhiteBalance, or your custom CRTVolumeProfile)")]
		public string VolumeComponentName;
		[Tooltip("The name of the parameter field in that component (e.g. intensity, temperature)")]
		public string ParameterName;

		[MMInspectorGroup("CRT Override", true, 22)]
		public bool UseCRTOverride = false;
		public enum CRTFloatParameters { PixelSize, DistortionStrength, DistortionSmoothing, RGBStrength, ScanlineStrength, ScanlineSize, ScrollSpeed, RandomWear, AberrationStrength, TrackingSize, TrackingStrength, TrackingSpeed, TrackingJitter, TrackingColorDamage, TrackingLinesThreshold, Brightness, Contrast }
		public CRTFloatParameters CRTParameter;

		protected VolumeComponent _volumeComponent;
		protected FieldInfo _parameterField;
		protected VolumeParameter<float> _floatParameter;
		protected VolumeParameter<int> _intParameter;

		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<Volume>();
			}

			if (UseCRTOverride)
			{
				VolumeComponentName = "CRTSettings";
				ParameterName = CRTParameter.ToString();
				// Lowercase first letter for matching C# field names if necessary, 
				// but in CRTSettings.cs they are already exactly like the enum but starting with lowercase usually?
				// Actually let's just map them explicitly or assume lowercase first letter.
				ParameterName = char.ToLowerInvariant(ParameterName[0]) + ParameterName.Substring(1);
			}

			if (Target != null && !string.IsNullOrEmpty(VolumeComponentName) && !string.IsNullOrEmpty(ParameterName))
			{
				foreach (var component in Target.profile.components)
				{
					if (component.GetType().Name == VolumeComponentName || component.GetType().FullName == VolumeComponentName)
					{
						_volumeComponent = component;
						break;
					}
				}

				if (_volumeComponent != null)
				{
					_parameterField = _volumeComponent.GetType().GetField(ParameterName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
					if (_parameterField != null)
					{
						object val = _parameterField.GetValue(_volumeComponent);
						_floatParameter = val as VolumeParameter<float>;
						_intParameter = val as VolumeParameter<int>;
					}
				}
			}

			base.Initialization();
		}

		public override float TargetFloat
		{
			get
			{
				if (_floatParameter != null)
				{
					return _floatParameter.value;
				}
				if (_intParameter != null)
				{
					return _intParameter.value;
				}
				return 0f;
			}
			set
			{
				if (_floatParameter != null)
				{
					_floatParameter.Override(value);
				}
				else if (_intParameter != null)
				{
					_intParameter.Override(Mathf.RoundToInt(value));
				}
			}
		}
	}
}
#endif
