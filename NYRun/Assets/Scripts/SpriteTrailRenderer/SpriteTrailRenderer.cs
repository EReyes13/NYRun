
using System.Collections.Generic;
using UnityEngine;

namespace SpriteTrailRenderer
{
    public enum SpawnType
    {
        TIME,
        DISTANCE
    };

    public class SpriteTrailRenderer : MonoBehaviour
    {
        [Header("Trail Settings")]
        public Vector2 _startScale = Vector2.one;
        public Vector2 _endScale = Vector2.one;

        public float _trailLifetime = 1.2f;
        public SpawnType _spawnType = SpawnType.TIME;

        public float _timeBetweenTrailSpawn = 0.05f;
        public float _distanceBetweenTrailSpawn = 0.5f;

        public float _trailBehindDistance = 0.15f;

        [Header("Character Rainbow")]
        public bool _rainbowCharacter = true;
        public float _rainbowSpeed = 1f;

        [Header("Trail Movement")]
        public float _trailMoveSpeed = 1.5f;

        [Header("Trail Colors")]
        public bool _alphaUpdateOn = true;
        public float _maxAlpha = 0.8f;
        public float _minAlpha = 0f;

        public bool _useSolidColors = true;
        public bool _rainbowMode = true;

        public List<Color32> _userSelectedColorPalette;

        private SpriteRenderer[] _spriteRenderers;
        private Color[] _originalColors;

        private float _lastSpawnTime;
        private Vector2 _lastSpawnLocation;

        private ObjectPool<SpriteTrailObject> _spriteObjects;

        private readonly static List<Color32> _rainbowColors =
            new List<Color32>
        {
            new Color32(255, 0, 0, 255),
            new Color32(255, 100, 0, 255),
            new Color32(255, 255, 0, 255),
            new Color32(0, 255, 0, 255),
            new Color32(0, 100, 255, 255),
            new Color32(75, 0, 255, 255),
            new Color32(180, 0, 255, 255),
            new Color32(255, 0, 150, 255)
        };

        private void Start()
        {
            _spriteRenderers =
                GetComponentsInChildren<SpriteRenderer>(true);

            if (_spriteRenderers.Length == 0)
            {
                Debug.LogError(
                    "SpriteTrailRenderer: No SpriteRenderers found!"
                );

                enabled = false;
                return;
            }

            _originalColors = new Color[_spriteRenderers.Length];

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _originalColors[i] = _spriteRenderers[i].color;
            }

            _lastSpawnLocation = transform.position;

            _spriteObjects = new ObjectPool<SpriteTrailObject>(
                SpawnTrailObject(), 3, 2
            );
        }

        private void Update()
        {
            if (_spriteRenderers == null ||
                _spriteRenderers.Length == 0)
                return;

            // Continuously apply rainbow colors to the character.
            if (_rainbowCharacter)
            {
                float hue = Mathf.Repeat(
                    Time.time * _rainbowSpeed, 1f
                );

                Color rainbowColor =
                    Color.HSVToRGB(hue, 1f, 1f);

                for (int i = 0; i < _spriteRenderers.Length; i++)
                {
                    if (_spriteRenderers[i] == null)
                        continue;

                    Color original = _originalColors[i];

                    _spriteRenderers[i].color = new Color(
                        rainbowColor.r * original.r,
                        rainbowColor.g * original.g,
                        rainbowColor.b * original.b,
                        original.a
                    );
                }
            }

            // Continuously spawn afterimages.
            if (_spawnType == SpawnType.TIME &&
                Time.time - _lastSpawnTime >= _timeBetweenTrailSpawn)
            {
                SpriteTrailObject trailObject =
                    _spriteObjects.GetObjectFromPool();

                SetValues(trailObject);

                trailObject.gameObject.SetActive(true);

                _lastSpawnTime = Time.time;
            }
            else if (_spawnType == SpawnType.DISTANCE &&
                (_lastSpawnLocation -
                (Vector2)transform.position).magnitude >
                _distanceBetweenTrailSpawn)
            {
                SpriteTrailObject trailObject =
                    _spriteObjects.GetObjectFromPool();

                SetValues(trailObject);

                trailObject.gameObject.SetActive(true);

                _lastSpawnLocation = transform.position;
            }
        }

        private GameObject SpawnTrailObject()
        {
            GameObject newGameObject =
                new GameObject("SpriteTrailObject");

            newGameObject.AddComponent<SpriteTrailObject>();

            return newGameObject;
        }

        private void SetValues(SpriteTrailObject trailObject)
        {
            // Always move the trail to the left.
            Vector3 directionBehind = Vector3.left;

            Vector3 trailPosition =
                transform.position +
                directionBehind * _trailBehindDistance;

            trailObject.transform.position = trailPosition;

            trailObject.transform.rotation = transform.rotation;

            trailObject.transform.localScale = transform.lossyScale;

            trailObject.SetSpawnValues(
                _spriteRenderers,
                transform,
                this,
                _rainbowMode
                    ? _rainbowColors
                    : _userSelectedColorPalette,
                directionBehind
            );
        }
    }
}

