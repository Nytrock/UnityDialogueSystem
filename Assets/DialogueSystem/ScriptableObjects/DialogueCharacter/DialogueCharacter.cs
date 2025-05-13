using UnityEngine;

[CreateAssetMenu(menuName = nameof(DialogueCharacter))]
public class DialogueCharacter : ScriptableObject {
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private DialogueCharacterEmotionSprite[] _emotionSprites;
    [SerializeField] private PitchableAudioInfo _voiceInfo;

    public PitchableAudioInfo VoiceInfo => _voiceInfo;
    public string Name => $"{nameof(DialogueCharacter)}.{name}";
    public Sprite Icon => _defaultSprite;

    public Sprite GetEmotionSprite(DialogueCharacterEmotion emotion) {
        foreach (var emotionSprite in _emotionSprites)
            if (emotionSprite.Emotion == emotion)
                return emotionSprite.Sprite;
        return _defaultSprite;
    }
}
