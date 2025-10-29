using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestButtonUI : MonoBehaviour
{
    public Button button;              // assign in prefab
    public Text titleText;             // optional, assign if using UI.Text
    public TMP_Text tmpTitleText;      // optional, assign if using TMP_Text

    public void SetTitle(string title)
    {
        if (titleText != null) titleText.text = title;
        else if (tmpTitleText != null) tmpTitleText.text = title;
        else Debug.LogWarning($"[QuestButtonUI] No Text/TMP_Text found in '{name}'");
    }
}
