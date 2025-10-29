using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro; // TextMeshProを使用
// using UnityEditor.Experimental.GraphView; // 不要なusingは削除

public class CityUIManager : MonoBehaviour
{
    private CityComponent currentCity;

    // 訓練と交流と募兵のモードを定義
    public enum MilitaryActionMode { None, Training, MoraleBoost, Recruitment }
    // 現在のモードを記憶する変数
    private MilitaryActionMode currentMode = MilitaryActionMode.None;

    // 選択された武将データ
    private GeneralData selectedDeploymentGeneral;
    
    // 選択された兵種インデックス (1, 2, or 3)
    private int selectedTroopIndex = 1; // ★★★ selectedTroopIndex の定義はここに統一 ★★★


    [Header("背景ビジュアル")]
    public UnityEngine.UI.RawImage backgroundRawImage;
    public List<Texture2D> backgroundTextures;
    public List<string> textureIDs;

    [Header("UI要素（アクションボタン）")]
    public Button agricultureButton;
    public Button commerceButton;
    public Button tradeButton;
    public Button returnToMapButton;
    public Button deployButton;
    
    // 軍事行動のメインボタン
    public Button mainTrainingButton;
    public Button mainMoraleButton;
    public Button mainRecruitmentButton; // ExecuteRecruitment()を呼ぶボタン

    [Header("ステータス表示UI")]
    public TextMeshProUGUI cityNameDisplay;
    public TextMeshProUGUI goldDisplay;
    public TextMeshProUGUI foodDisplay;
    public TextMeshProUGUI populationDisplay;
    public TextMeshProUGUI agricultureLevelText;
    public TextMeshProUGUI commerceLevelText;
    public TextMeshProUGUI tradeLevelText;
    // 兵種別ステータス表示
    public TMPro.TextMeshProUGUI unitTypeText1;
    public TMPro.TextMeshProUGUI unitCountText1;
    public TMPro.TextMeshProUGUI trainingLevelText1;
    public TMPro.TextMeshProUGUI moraleText1;
    public TMPro.TextMeshProUGUI unitTypeText2;
    public TMPro.TextMeshProUGUI unitCountText2;
    public TMPro.TextMeshProUGUI trainingLevelText2;
    public TMPro.TextMeshProUGUI moraleText2;
    public TMPro.TextMeshProUGUI unitTypeText3;
    public TMPro.TextMeshProUGUI unitCountText3;
    public TMPro.TextMeshProUGUI trainingLevelText3;
    public TMPro.TextMeshProUGUI moraleText3;


    [Header("編成・部隊管理パネル")]
    public GameObject troopSelectionPanel;    // 訓練/交流/募兵用パネル
    public GameObject deploymentPanel;        // 全体編成パネル（出陣用）
    
    // ★★★ 修正箇所：重複を削除し、ここに最終定義を統合 ★★★
    public GameObject generalListPanel;       // 左側：武将リストScrollViewを囲むパネル
    public GameObject troopInputPanel;        // 右側：兵数入力とDeployボタンを囲むパネル
    
    public RectTransform generalListContent;   // 武将リストScrollViewのContent
    public GameObject generalItemPrefab;      // 武将リストアイテムのプレファブ
    public TMP_InputField troopInputField;    // 兵数入力フィールド
    
    [Header("兵種トグル")]
    public Toggle swordToggle;
    public Toggle bowToggle;
    public Toggle navyToggle;
    
    [Header("デプロイメント UI")]
    public Transform deployedContentParent; // ★新規: 右側 ScrollView の Content の Transform
    public GameObject deployedUnitItemPrefab; // ★新規: DeployedUnitItem.prefab への参照

    // ★新規: 出陣候補部隊を一時的に保持するリスト
    private List<TroopData> stagedTroops = new List<TroopData>();


    void Start()
    {
        // ... (Startメソッド内のロジックは省略) ...
    }

    private void InitializeUI()
    {
        // ... (InitializeUIメソッド内のロジックは省略) ...
    }
    
    public void UpdateCityUI()
    {
        // ... (UpdateCityUIメソッド内のロジックは省略) ...
    }
    
    // ... (ExecuteAgriculture, ExecuteCommerce, ExecuteTrade, ReturnToMap のメソッドは省略) ...

    // ★★★ 軍事アクション - モード設定 ★★★
    public void ExecuteTraining()
    {
        currentMode = MilitaryActionMode.Training;
        troopSelectionPanel.SetActive(true);
    }
    public void ExecuteMoraleBoost()
    {
        currentMode = MilitaryActionMode.MoraleBoost;
        troopSelectionPanel.SetActive(true);
    }
    public void ExecuteRecruitment()
    {
        currentMode = MilitaryActionMode.Recruitment;
        troopSelectionPanel.SetActive(true);
    }
    
    // ★★★ 部隊編成モードの開始 ★★★
    public void ExecuteDeploymentMode()
    {
        if (deploymentPanel != null)
        {
            deploymentPanel.SetActive(true);
            // 初期状態：武将リストを表示し、入力パネルを非表示
            if (generalListPanel != null) generalListPanel.SetActive(true);
            if (troopInputPanel != null) troopInputPanel.SetActive(false);
            LoadGeneralList();
        }
    }
    
    // ★★★ 武将リストの生成とフィルタリング ★★★
    public void LoadGeneralList()
    {
        // ... (LoadGeneralListメソッド内のロジックは省略) ...
    }
    
    // ★★★ 武将がリストで選択されたとき (左リスト -> 右パネル) ★★★
    public void SetSelectedGeneral(GeneralData general)
    {
        selectedDeploymentGeneral = general;
        
        // 1. UIの切り替え: 武将リストを非表示、兵数入力を表示
        if (generalListPanel != null && troopInputPanel != null) 
        {
            // 左側の武将リストを非表示にする
            generalListPanel.SetActive(false);  

            // 右側の兵数入力パネルを表示する
            troopInputPanel.SetActive(true);    
        }
        else
        {
            Debug.LogError("FATAL ERROR: generalListPanel または troopInputPanel が Inspector で未接続です！");
        }
        
        Debug.Log($"大将として {general.generalName} を選択しました。");
    }

    // ★★★ 部隊の確定と出陣 ★★★
    public void FinalizeDeployment()
    {
        // ... (FinalizeDeploymentメソッド内のロジックは省略) ...
    }
    
    /// <summary>
    /// 右側の出陣リストから部隊を外し、武将と兵数を都市に戻す。
    /// このメソッドは DeployedUnitItemController のボタンクリックによって呼び出される。
    /// </summary>
    /// <param name="troopToRemove">削除する部隊データ</param>
    /// <param name="itemObject">削除するUIオブジェクト</param>
    public void RemoveStagedTroop(TroopData troopToRemove, GameObject itemObject)
    {
        // 1. 【内部リストから削除】: CityUIManagerが持つリストから削除
        if (stagedTroops.Contains(troopToRemove))
        {
            stagedTroops.Remove(troopToRemove);

            // 2. 【都市に兵を返却】: currentCity変数（クラスの先頭で定義済み）を使用
            // ※ 注: currentCityクラスにReturnTroopsメソッドが実装されている必要があります
            if (currentCity != null)
            {
                currentCity.ReturnTroops(troopToRemove); 
            }
            
            // 3. 【UIの削除】
            Destroy(itemObject);

            // 4. 【左リストの更新】: 武将がフリーになったため、編成リストに再表示
            LoadGeneralList(); 

            Debug.Log($"【編成解除】{troopToRemove.general.generalName} の部隊を解除し、兵を都市に戻しました。");
        }
        else
        {
            Debug.LogWarning("解除しようとした部隊データが stagedTroops リストに見つかりませんでした。");
        }
    }



    // ... (補助関数 GetUnitPrefabByTroopIndex は省略) ...

    // ExecuteTroopAction, CalculateNegotiationBonus, CalculateCultureBonus, ExecuteTrade, LoadGeneralList... (その他のメソッド)
}