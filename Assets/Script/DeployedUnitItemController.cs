// DeployedUnitItemController.cs

using UnityEngine;
using UnityEngine.UI;

public class DeployedUnitItemController : MonoBehaviour
{
    [Header("UI Components")]
    public Image PortraitImage;
    public Text GeneralNameText;
    public Text StatsText;
    public Text TroopInfoText;
    public Button RemoveButton;

    // このアイテムが保持する部隊データ
    private TroopData deployedTroop;
    private CityUIManager uiManager; // 親のUIManagerへの参照

    public void Initialize(TroopData troop, CityUIManager manager)
    {
        deployedTroop = troop;
        uiManager = manager;

        // データの表示
        GeneralNameText.text = deployedTroop.general.generalName;
        PortraitImage.sprite = deployedTroop.general.portraitImage;
        
        // 能力値の表示（例: 武力と知力）
        StatsText.text = $"武力: {deployedTroop.general.warfare}\n知力: {deployedTroop.general.intelligence}";
        
        // 兵種と兵数
        TroopInfoText.text = $"{deployedTroop.unitName}\n兵数: {deployedTroop.unitCount.ToString("N0")}"; // N0で桁区切り表示

        // 解除ボタンにクリックイベントを設定
        RemoveButton.onClick.RemoveAllListeners();
        RemoveButton.onClick.AddListener(OnRemoveButtonClicked);
    }

    /// <summary>
    /// 解除ボタンが押された時の処理
    /// </summary>
    private void OnRemoveButtonClicked()
    {
        // CityUIManagerに部隊の解除処理を委譲する
        uiManager.RemoveStagedTroop(deployedTroop, this.gameObject);
    }

    /// <summary>
    /// 右側の出陣リストから部隊を外し、武将と兵数を都市に戻す。
    /// </summary>
    /// <param name="troopToRemove">削除する部隊データ</param>
    /// <param name="itemObject">削除するUIオブジェクト</param>
    /*public void RemoveStagedTroop(TroopData troopToRemove, GameObject itemObject)
    {
        if (stagedTroops.Contains(troopToRemove))
        {
            // 1. 【内部リストから削除】
            stagedTroops.Remove(troopToRemove);

            // 2. 【都市に兵を返却】
            // CityComponentに兵の返却処理を委譲するメソッドを実装します。
            currentCity.ReturnTroops(troopToRemove); 
            
            // 3. 【UIの削除】
            Destroy(itemObject);

            // 4. 【左リストの更新】
            // 武将がフリーになったため、左リストに再度表示するために再ロード
            LoadGeneralList(); 

            Debug.Log($"【編成解除】{troopToRemove.general.generalName} の部隊を解除し、兵を都市に戻しました。");
        }
    }*/
}