// UnitSlotItemController.cs (新規作成)

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSlotItemController : MonoBehaviour
{
    public Button selectButton;
    public TextMeshProUGUI summaryText; // 武将名/兵種/兵数概要
    // public Image portraitImage; // 顔アイコン (必要に応じて追加)

    private CityUIManager uiManager;
    private int slotIndex; // グローバルリスト (stagedTroopSlots) 内のインデックス

    /// <summary>
    /// スロットの初期化とデータ表示
    /// </summary>
    public void Initialize(TroopData troop, int index, CityUIManager manager)
    {
        slotIndex = index;
        uiManager = manager;

        // データ表示: スロットが空か、編成済みかで表示を切り替える
        if (troop == null)
        {
            summaryText.text = $"スロット {index + 1}: (空)";
        }
        else
        {
            summaryText.text = $"{troop.general.generalName} / {troop.unitName} {troop.count}";
        }

        // ★★★ 選択イベントの設定 ★★★
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSlotSelected);
    }

    private void OnSlotSelected()
    {
        // CityUIManagerに、選択されたスロットのインデックスを渡し、右側パネルを更新させる
        uiManager.SelectDeploymentSlot(slotIndex);
    }
}