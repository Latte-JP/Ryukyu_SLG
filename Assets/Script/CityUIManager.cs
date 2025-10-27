using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro; // TextMeshProを使用

// ★注意: UnityEditor.Experimental.GraphViewの使用は不要なため削除済み★

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
    private int selectedTroopIndex = 1;


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
    public GameObject generalListPanel;       // 左側：武将リストScrollViewを囲むパネル
    public GameObject troopInputPanel;        // 右側：兵数入力とDeployボタンを囲むパネル
    public RectTransform generalListContent;   // 武将リストScrollViewのContent
    public GameObject generalItemPrefab;      // 武将リストアイテムのプレファブ
    public TMP_InputField troopInputField;    // 兵数入力フィールド
    
    [Header("兵種トグル")]
    public Toggle swordToggle;
    public Toggle bowToggle;
    public Toggle navyToggle;
    private int selectedTroopIndex = 1; // 選択された兵種インデックス

    [Header("デプロイメント UI")]
    public GameObject generalListPanel; // 左側の所属武将リスト（既存）
    public GameObject troopInputPanel;  // 兵種・兵数入力エリア（既存）
    public Transform deployedContentParent; // ★新規: 右側 ScrollView の Content の Transform
    public GameObject deployedUnitItemPrefab; // ★新規: DeployedUnitItem.prefab への参照

    // ★新規: 出陣候補部隊を一時的に保持するリスト
    private List<TroopData> stagedTroops = new List<TroopData>();

    void Start()
    {
        currentCity = GameManager.Instance.GetSelectedCityComponent();

        if (currentCity == null)
        {
            Debug.LogError("★エラー: 操作対象の城データが見つかりません！★");
            return;
        }
    
        InitializeUI();
        UpdateCityUI();
        
        // 背景画像の切り替え
        string targetID = currentCity.Data.backgroundSceneID;
        int index = textureIDs.IndexOf(targetID);
        if (index >= 0 && index < backgroundTextures.Count && backgroundRawImage != null)
        {
            backgroundRawImage.texture = backgroundTextures[index];
            Debug.Log($"背景を {targetID} に変更しました。");
        }
    }

    private void InitializeUI()
    {
        // 既存のボタンリスナー設定
        agricultureButton.onClick.AddListener(ExecuteAgriculture);
        commerceButton.onClick.AddListener(ExecuteCommerce);
        tradeButton.onClick.AddListener(ExecuteTrade);
        returnToMapButton.onClick.AddListener(ReturnToMap);
        
        // 軍事行動メインボタン
        mainTrainingButton.onClick.AddListener(ExecuteTraining);
        mainMoraleButton.onClick.AddListener(ExecuteMoraleBoost);
        mainRecruitmentButton.onClick.AddListener(ExecuteRecruitment); // 募兵ボタンの接続
        
        // 兵種トグルリスナー (排他制御)
        swordToggle.onValueChanged.AddListener((isOn) => { if (isOn) selectedTroopIndex = 1; });
        bowToggle.onValueChanged.AddListener((isOn) => { if (isOn) selectedTroopIndex = 2; });
        navyToggle.onValueChanged.AddListener((isOn) => { if (isOn) selectedTroopIndex = 3; });
        
        // 編成ボタンのリスナー (出陣確定)
        deployButton.onClick.AddListener(FinalizeDeployment);
    }
    
    // ★★★ 内政ステータスの更新 ★★★
    public void UpdateCityUI()
    {
        if (currentCity == null) return;
        CityData data = currentCity.Data;

        // 基本情報
        cityNameDisplay.text = data.cityName;
        goldDisplay.text = $"{data.goldStock:N0}";
        foodDisplay.text = $"{data.foodStock:N0}";
        populationDisplay.text = $"{data.population:N0}";

        // レベル情報
        agricultureLevelText.text = data.agricultureLevel.ToString();
        commerceLevelText.text = data.commerceLevel.ToString();
        tradeLevelText.text = data.tradeLevel.ToString();

        // 資源収支
        foodIncomeText.text = $"{currentCity.Data.foodIncome}";
        foodConsumptionText.text = $"{currentCity.Data.foodConsumption}";
        goldIncomeText.text = $"{currentCity.Data.goldIncome}";
        goldConsumptionText.text = $"{currentCity.Data.goldConsumption}";

        // 軍事パラメーター
        unitTypeText1.text = $"{currentCity.Data.unitType1}";
        unitCountText1.text = $"{currentCity.Data.unitCount1}";
        trainingLevelText1.text = $"{currentCity.Data.trainingLevel1}%";
        moraleText1.text = $"{currentCity.Data.morale1}%";
        // ... unit 2, 3 も同様に更新
    }
    
    // ★★★ 内政アクション ★★★
    public void ExecuteAgriculture()
    {
        currentCity.PerformAgricultureAction(100, 1);
        UpdateCityUI();
    }
    public void ExecuteCommerce()
    {
        string result = currentCity.PerformCommerceAction(150, 1);
        Debug.Log($"市場開拓結果: {result}");
        UpdateCityUI();
    }
    public void ExecuteTrade()
    {
        string result = currentCity.PerformTradeAction(150, 1);
        Debug.Log($"港整備結果: {result}");
        UpdateCityUI();
    }
    public void ReturnToMap()
    {
        SceneManager.LoadScene("MapScene");
    }

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
        foreach (Transform child in generalListContent)
        {
            Destroy(child.gameObject);
        }
    
        string currentCityName = currentCity.Data.cityName;
        List<GeneralData> localGenerals = GameManager.Instance.GetGeneralsInCity(currentCityName);

        if (localGenerals == null || localGenerals.Count == 0)
        {
            Debug.Log("この城には現在、出陣可能な武将がいません。");
            return;
        }

        foreach (GeneralData general in localGenerals)
        {
            GameObject itemObj = Instantiate(generalItemPrefab, generalListContent);
            GeneralItemController itemController = itemObj.GetComponent<GeneralItemController>();
            if (itemController != null)
            {
                itemController.Initialize(general, this); 
            }
        }
        Debug.Log($"【編成リスト】{localGenerals.Count}名の武将をロードしました。");
    }
    
    /// <summary>
    /// 選択された武将、兵種、兵数に基づき、部隊を編成し出陣リストに追加します。
    /// </summary>
    public void DeployAndStageTroop()
    {
        // 1. 選択チェック
        if (selectedDeploymentGeneral == null)
        {
            Debug.LogError("大将が選択されていません。");
            return;
        }
        
        // 2. 兵数入力の検証
        int troopCount = 0;
        if (!int.TryParse(troopInputField.text, out troopCount) || troopCount <= 0)
        {
            Debug.LogError("有効な兵数（1以上の数値）を入力してください。");
            return;
        }

        // 3. 兵種インデックスの取得
        int troopIndex = selectedTroopIndex; 
        
        if (troopIndex == 0) // トグルが何も選択されていない場合
        {
            Debug.LogError("編成する兵種を選択してください。");
            return;
        }

        // 4. CityComponent.DeployTroop()の呼び出しと部隊生成
        // DeployTroopが、都市の兵数を削減し、新しいTroopDataオブジェクトを返します。
        TroopData newTroop = currentCity.DeployTroop(selectedDeploymentGeneral, troopIndex, troopCount);

        if (newTroop != null)
        {
            // 5. 【出陣リスト (内部データ) へ追加】
            // ★右側のリスト管理のために、内部リストに部隊を追加★
            // TODO: stagedTroops リストを CityUIManager に追加し、ここで Add 処理を行います。
            // stagedTroops.Add(newTroop); 
            
            // 6. 【右側リストUIへの表示】
            // deployedUnitItemPrefab を ContentParent に生成し、newTroop データを渡します。
            // GameObject unitItemObj = Instantiate(deployedUnitItemPrefab, deployedContentParent);
            // unitItemObj.GetComponent<DeployedUnitItemController>().Initialize(newTroop, this);
            
            // 7. 【左側リストの更新】
            // 武将が部隊長になったため、左リストから除外するために再ロード
            LoadGeneralList();
            
            // 8. 【出陣後の後処理】
            // 選択状態をリセットし、UIを元に戻します。
            selectedDeploymentGeneral = null;
            troopInputField.text = ""; 
            
            // 兵数入力パネルを非表示に戻し、武将リストパネルを再表示
            if (generalListPanel != null && troopInputPanel != null) 
            {
                generalListPanel.SetActive(true);  
                troopInputPanel.SetActive(false);    
            }
            
            Debug.Log($"【編成成功】武将: {newTroop.general.generalName} の{newTroop.unitName}部隊が出陣リストに追加されました。");
        }
        
        // ※注意：FinalizeDeployment()は、FinalizeDeployment()ロジックを修正して
        // DeployAndStageTroop()に役割を移行したため、その後のロジック修正が必要です。
    }


    // ★★★ 武将がリストで選択されたとき (左リスト -> 右パネル) ★★★
    public void (GeneralData general)
    {
        selectedDeploymentGeneral = general;
        
        // 1. UIの切り替え: 武将リストを非表示、兵数入力を表示
        if (generalListPanel != null && troopInputPanel != null) 
        {
            generalListPanel.SetActive(false);  
            troopInputPanel.SetActive(true);    
        }
        
        // 2. 確定エリア（右側）に武将名を表示するロジックなどをここに追加

        Debug.Log($"大将として {general.generalName} を選択しました。");
    }
    
    // ★★★ 部隊の確定と出陣 ★★★
    public void FinalizeDeployment()
    {
        // 1. 選択チェック
        if (selectedDeploymentGeneral == null)
        {
            Debug.LogError("大将が選択されていません。");
            return;
        }
    
        // 2. 兵数入力の検証
        int troopCount = 0;
        if (!int.TryParse(troopInputField.text, out troopCount) || troopCount <= 0)
        {
            Debug.LogError("有効な兵数（1以上の数値）を入力してください。");
            return;
        }

        // 3. 兵種インデックスの取得
        int troopIndex = selectedTroopIndex; 
        
        // 4. DeployTroop()の呼び出し
        TroopData newTroop = currentCity.DeployTroop(selectedDeploymentGeneral, troopIndex, troopCount);

        if (newTroop != null)
        {
            // 5. 【マップ上へのユニット配置】(GameManagerへの追跡)
            GameObject unitPrefab = GetUnitPrefabByTroopIndex(troopIndex);
            Vector3 deployPosition = currentCity.transform.position + Vector3.up * 0.5f; 
            
            GameObject unitObj = Instantiate(unitPrefab, deployPosition, Quaternion.identity);
            UnitController unitController = unitObj.GetComponent<UnitController>();
            
            if (unitController != null)
            {
                unitController.Initialize(newTroop, deployPosition, currentCity.Data.cityName);
                GameManager.Instance.AddActiveUnit(unitController);
            }

            Debug.Log($"【編成成功】武将: {newTroop.general.generalName} の{newTroop.unitName}部隊が出陣準備完了。");
            
            // 6. パネルを閉じる
            deploymentPanel.SetActive(false);
            
            // TODO: CityUIを更新する UpdateCityUI()を忘れずに呼び出す
            UpdateCityUI();
        }
    }

    // 補助関数 (GameManagerまたはCityUIManagerに追加)
    private GameObject GetUnitPrefabByTroopIndex(int index)
    {
        // ★暫定的なロジック: 実際のプレファブはGameManagerに接続する必要があります
        if (index == 1) return Resources.Load<GameObject>("Prefabs/Unit_Sword_Prefab"); // 例
        if (index == 2) return Resources.Load<GameObject>("Prefabs/Unit_Bow_Prefab"); // 例
        if (index == 3) return Resources.Load<GameObject>("Prefabs/Unit_Navy_Prefab"); // 例
        
        return GameManager.Instance.MilitaryUnitPrefab; // デフォルト
    }

    /// <summary>
    /// 部隊編成パネル内で武将が選択されたときに呼び出されます。
    /// 選択された武将をselectedDeploymentGeneralに設定し、UIを兵数入力画面に切り替えます。
    /// </summary>
    public void SetSelectedGeneral(GeneralData general)
    {
        // 1. 選択された武将データを記憶
        selectedDeploymentGeneral = general;
        
        // 2. UIの切り替え: 武将リストを非表示、兵数入力を表示
        if (generalListPanel != null && troopInputPanel != null) 
        {
            // 左側の武将リストを非表示にする
            generalListPanel.SetActive(false);  

            // 右側の兵数入力パネルを表示する
            troopInputPanel.SetActive(true);    
        }
        else
        {
            // 接続漏れを防ぐためのエラーログ（デバッグ用）
            Debug.LogError("FATAL ERROR: generalListPanel または troopInputPanel が Inspector で未接続です！");
        }
        
        // 3. デバッグログ
        Debug.Log($"大将として {general.generalName} を選択しました。");
    }

    // ExecuteTroopAction, CalculateNegotiationBonus, CalculateCultureBonus, ExecuteTrade, LoadGeneralList... (その他のメソッド)
}