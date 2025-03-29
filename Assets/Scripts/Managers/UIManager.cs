using TMPro;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; } = null!;

        [SerializeField] private TMP_Text hintText;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetHint(string text)
        {
            hintText.gameObject.SetActive(true);
            hintText.text = text;
        }

        public void ClearHint()
        {
            hintText.gameObject.SetActive(false);
            hintText.text = string.Empty;
        }

    }
}