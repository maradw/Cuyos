using UnityEngine;

namespace Game.Audio
{
    [CreateAssetMenu(fileName = "MusicData", menuName = "Game/Audio/Music")]
    public class MusicData : ScriptableObject
    {
        [Header("Audio")]
        public AudioClip Clip;

        [Range(0f, 1f)]
        public float Volume = 1f;

        public bool Loop = true;

        [Header("Transitions")]
        [Min(0f)]
        public float FadeIn = 1f;

        [Min(0f)]
        public float FadeOut = 1f;
    }
}