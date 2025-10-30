// WarlordItemController.cs (新規作成)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarlordItemController : MonoBehaviour
{
    public Button selectButton;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;

    private GeneralData myWarlordData;
    private CityUIManager uiManager;
    private string targetSector; // どの部門の担当を選ぶか ("Agriculture", "Commerce", "Trade")

    public void Initialize(GeneralData warlord, CityUIManager manager, string sector)
    {
        myWarlordData = warlord;
        uiManager = manager;
        targetSector = sector;

        nameText.text = warlord.generalName;
        // 担当部門に合った能力値を強調表示
        statsText.text = GetRelevantStats(warlord, sector); 

        selectButton.onClick.RemoveAllListeners();
        // ★重要: ボタン押下時に任命メソッドを呼び出す ★
        selectButton.onClick.AddListener(OnWarlordSelected);
    }
    
    public void OnWarlordSelected()
    {
        // 担当武将を任命し、パネルを閉じるロジックをUIManagerに委譲
        uiManager.FinalizeWarlordAssignment(myWarlordData, targetSector);
    }

    private string GetRelevantStats(GeneralData warlord, string sector)
    {
        // 以前の議論に基づき、能力値を表示
        if (sector == "Agriculture") return $"知略: {warlord.intelligence}"; // 農業は知略
        if (sector == "Commerce") return $"政治: {warlord.politics}";     // 商業は政治力
        if (sector == "Trade") return $"交易: {warlord.intelligence} / {warlord.culture}"; // 交易は知略と文化力
        return "";
    }
}