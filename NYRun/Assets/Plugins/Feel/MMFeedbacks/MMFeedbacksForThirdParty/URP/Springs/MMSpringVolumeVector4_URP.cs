#if MM_URP
using UnityEngine;
using UnityEngine.Rendering;
using System.Reflection;
using System;
using MoreMountains.Tools;

namespace MoreMountains.Feedbacks
{
	[AddComponentMenu("More Mountains/Springs/MM Spring Volume Vector4 URP")]
	public class MMSpringVolumeVector4_URP : MMSpringVector4<Volume>
	{
		[MMInspectorGroup("Volume Settings", true, 21)]
		[Tooltip("The name of the VolumeComponent (e.g. Bloom, Vignette, or your custom CRTVolumeProfile)")]
		public string VolumeComponentName;
		[Tooltip("The name of the parameter field in that component (e.g. color, center)")]
		public string ParameterName;

		protected VolumeComponent _volumeComponent;
		protected FieldInfo _parameterField;
		protected VolumeParameter<Vector4> _vector4Parameter;

		protected override void Initialization()
		{
			if (Target == null)
			{
				Target = this.gameObject.GetComponent<Volume>();
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
						object val = _parameterField.GetValue(_volumeComponent);
						_vector4Parameter = val as VolumeParameter<Vector4>;
					}
				}
			}

			base.Initialization();
		}

		public override Vector4 TargetVector4
		{
			get
			{
				if (_vector4Parameter != null)
				{
					return _vector4Parameter.value;
				}
				return Vector4.zero;
			}
			set
			{
				if (_vector4Parameter != null)
				{
					_vector4Parameter.Override(value);
				}
			}
		}
	}
}
#endif
