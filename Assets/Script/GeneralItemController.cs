// GeneralItemController.cs を新規作成し、GeneralItemPrefabにアタッチ

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneralItemController : MonoBehaviour
{
    public Button selectButton;
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;

    private GeneralData myGeneralData;
    private CityUIManager uiManager;

    public void Initialize(GeneralData general, CityUIManager manager)
    {
        myGeneralData = general;
        uiManager = manager;

        // UI要素にデータを反映
        nameText.text = general.generalName;
        statsText.text = $"武力:{general.warfare} 統率:{general.leadership}";
        portraitImage.sprite = general.portraitImage;

        // ボタンが押されたとき、CityUIManagerに武将データを渡す
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => uiManager.SetSelectedGeneral(myGeneralData));
    }
}