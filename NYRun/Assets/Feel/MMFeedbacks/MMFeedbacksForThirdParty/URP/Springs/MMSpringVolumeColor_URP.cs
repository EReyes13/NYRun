#if MM_URP
using UnityEngine;
using UnityEngine.Rendering;
using System.Reflection;
using System;
using MoreMountains.Tools;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MM Spring Volume Color URP")]
	public class MMSpringVolumeColor_URP : MMSpringColorComponent<Volume>
	{
		[MMInspectorGroup("Volume Settings", true, 21)]
		[Tooltip("The name of the VolumeComponent (e.g. Bloom, WhiteBalance, or your custom CRTVolumeProfile)")]
		public string VolumeComponentName;
		[Tooltip("The name of the parameter field in that component (e.g. tintColor, backgroundColor)")]
		public string ParameterName;

		[MMInspectorGroup("CRT Override", true, 22)]
		public bool UseCRTOverride = false;
		public enum CRTColorParameters { TintColor, BackgroundColor, TrackingLinesColor }
		public CRTColorParameters CRTParameter;

		protected VolumeComponent _volumeComponent;
		protected FieldInfo _parameterField;
		protected VolumeParameter<Color> _colorParameter;

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
					_parameterField = _volumeComponent.GetType().GetField(ParameterName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
					if (_parameterField != null)
					{
						_colorParameter = _parameterField.GetValue(_volumeComponent) as VolumeParameter<Color>;
					}
				}
			}

			base.Initialization();
		}

		public override Color TargetColor
		{
			get
			{
				if (_colorParameter != null)
				{
					return _colorParameter.value;
				}
				return Color.white;
			}
			set
			{
				if (_colorParameter != null)
				{
					_colorParameter.Override(value);
				}
			}
		}
	}
}
#endif
