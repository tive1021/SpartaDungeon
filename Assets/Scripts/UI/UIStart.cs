using TMPro;
using UnityEngine;

public class UIStart : MonoBehaviour
{
    [SerializeField] private GameObject uiPrefab;

    void Start()
    {
        InitializeUI();
    }

    void InitializeUI()
    {
        GameObject uiInstance = Instantiate(uiPrefab);

        // UI 인스턴스에서 Interaction 컴포넌트를 찾기
        Interaction interaction = FindObjectOfType<Interaction>();
        if (interaction != null)
        {
            // UI 인스턴스에서 promptText를 가져와 Interaction에 연결
            TextMeshProUGUI promptText = uiInstance.GetComponentInChildren<TextMeshProUGUI>();
            interaction.promptText = promptText;
        }
    }
}