
using System.Collections.Generic;
using UnityEngine;

namespace SpriteTrailRenderer
{
    public class SpriteTrailObject : MonoBehaviour, IPoolable
    {
        public ReturnObjectToPool _returnToPool;

        public SpriteTrailRenderer _spriteTrailValues;

        private List<SpriteRenderer> _trailRenderers =
            new List<SpriteRenderer>();

        private List<Color> _originalColors =
            new List<Color>();

        private float _timeAlive;
        private bool _active;

        private Vector3 _baseScale;
        private Vector3 _moveDirection;

        public List<Color32> _colorsToUse;

        private void Awake()
        {
            _returnToPool = (x) => x.SetActive(false);

            _timeAlive = 0f;
            _active = false;
        }

        public void SetSpawnValues(
            SpriteRenderer[] sourceRenderers,
            Transform sourceRoot,
            SpriteTrailRenderer trailSettings,
            List<Color32> colors,
            Vector3 moveDirection)
        {
            _spriteTrailValues = trailSettings;
            _colorsToUse = colors;

            _timeAlive = 0f;
            _active = true;

            _moveDirection = moveDirection.normalized;

            _baseScale = sourceRoot.lossyScale;

            transform.localScale = _baseScale;

            // Create a renderer for every character part.
            while (_trailRenderers.Count < sourceRenderers.Length)
            {
                GameObject part =
                    new GameObject("TrailSpritePart");

                part.transform.SetParent(transform, false);

                SpriteRenderer newRenderer =
                    part.AddComponent<SpriteRenderer>();

                _trailRenderers.Add(newRenderer);
                _originalColors.Add(Color.white);
            }

            // Copy the character's current animated appearance.
            for (int i = 0; i < sourceRenderers.Length; i++)
            {
                SpriteRenderer source = sourceRenderers[i];

                SpriteRenderer target = _trailRenderers[i];

                if (source == null)
                {
                    target.enabled = false;
                    continue;
                }

                target.enabled = source.enabled;

                target.sprite = source.sprite;

                target.flipX = source.flipX;
                target.flipY = source.flipY;

                target.drawMode = source.drawMode;
                target.size = source.size;

                target.color = source.color;

                target.sharedMaterial = source.sharedMaterial;

                target.sortingLayerID =
                    source.sortingLayerID;

                target.sortingOrder =
                    source.sortingOrder - 1;

                target.maskInteraction =
                    source.maskInteraction;

                // Preserve each part's position.
                target.transform.localPosition =
                    sourceRoot.InverseTransformPoint(
                        source.transform.position
                    );

                target.transform.localRotation =
                    Quaternion.Inverse(sourceRoot.rotation) *
                    source.transform.rotation;

                Vector3 sourceScale =
                    source.transform.lossyScale;

                target.transform.localScale = new Vector3(
                    SafeDivide(sourceScale.x, _baseScale.x),
                    SafeDivide(sourceScale.y, _baseScale.y),
                    SafeDivide(sourceScale.z, _baseScale.z)
                );

                _originalColors[i] = source.color;
            }

            // Disable unused renderers.
            for (int i = sourceRenderers.Length;
                i < _trailRenderers.Count; i++)
            {
                _trailRenderers[i].enabled = false;
            }
        }

        private float SafeDivide(float value, float divisor)
        {
            return Mathf.Abs(divisor) < 0.0001f
                ? value
                : value / divisor;
        }

        private void Update()
        {
            if (!_active || _spriteTrailValues == null)
                return;

            _timeAlive += Time.deltaTime;

            // Move each afterimage continuously to the left.
            transform.position +=
                _moveDirection *
                _spriteTrailValues._trailMoveSpeed *
                Time.deltaTime;

            float percentDone = Mathf.Clamp01(
                _timeAlive /
                Mathf.Max(
                    _spriteTrailValues._trailLifetime,
                    0.001f
                )
            );

            // Change the size of each afterimage over time.
            float scaleX = Mathf.Lerp(
                _spriteTrailValues._startScale.x,
                _spriteTrailValues._endScale.x,
                percentDone
            );

            float scaleY = Mathf.Lerp(
                _spriteTrailValues._startScale.y,
                _spriteTrailValues._endScale.y,
                percentDone
            );

            transform.localScale = new Vector3(
                _baseScale.x * scaleX,
                _baseScale.y * scaleY,
                _baseScale.z
            );

            // Select the rainbow color.
            Color trailColor = Color.white;

            if (_spriteTrailValues._useSolidColors &&
                _colorsToUse != null &&
                _colorsToUse.Count > 0)
            {
                int colorIndex = Mathf.Min(
                    Mathf.FloorToInt(
                        _colorsToUse.Count * percentDone
                    ),
                    _colorsToUse.Count - 1
                );

                trailColor = _colorsToUse[colorIndex];
            }

            // Fade every character part.
            for (int i = 0; i < _trailRenderers.Count; i++)
            {
                SpriteRenderer renderer = _trailRenderers[i];

                if (renderer == null || !renderer.enabled)
                    continue;

                Color newColor =
                    _spriteTrailValues._useSolidColors
                    ? trailColor
                    : _originalColors[i];

                if (_spriteTrailValues._alphaUpdateOn)
                {
                    newColor.a *= Mathf.Lerp(
                        _spriteTrailValues._maxAlpha,
                        _spriteTrailValues._minAlpha,
                        percentDone
                    );
                }

                renderer.color = newColor;
            }

            // Return the afterimage to the pool.
            if (_timeAlive >= _spriteTrailValues._trailLifetime)
            {
                _active = false;

                _returnToPool(gameObject);
            }
        }

        public void SetReturnToPool(
            ReturnObjectToPool returnDelegate)
        {
            _returnToPool = returnDelegate;
        }
    }
}
