using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneralItemController : MonoBehaviour
{
    // ★Inspectorで接続するUI要素★
    public Button selectButton;
    public RawImage portraitImage; // RawImageに変更

    public TextMeshProUGUI largeNameText;
    public TextMeshProUGUI statsText;

    private GeneralData myGeneralData;
    private CityUIManager uiManager;

    public void Initialize(GeneralData general, CityUIManager manager)
    {
        myGeneralData = general;
        uiManager = manager;

        // 1. UI要素にデータを反映
        largeNameText.text = general.generalName;
        
        // 能力値を表示（統率力特化の設計を反映）
        statsText.text = $"統率:{general.leadership} 武力:{general.warfare} 知略:{general.intelligence}";
        
        // 顔画像を表示 (SpriteをRawImageに表示)
        if (general.portraitImage != null)
        {
            portraitImage.texture = general.portraitImage.texture;
        }

        // 2. ボタンのOnClickイベント設定
        selectButton.onClick.RemoveAllListeners();
        // ボタンが押されたら、CityUIManagerのSetSelectedGeneralに武将データを渡す
        selectButton.onClick.AddListener(() => uiManager.SetSelectedGeneral(myGeneralData)); 
    }
}