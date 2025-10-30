using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro; // TextMeshProを使用
using System.Linq;

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


    [Header("Background Visual")]
    public UnityEngine.UI.RawImage backgroundRawImage;
    public List<Texture2D> backgroundTextures;
    public List<string> textureIDs;

    [Header("Action Button UI")]
    public Button agricultureButton;
    public Button commerceButton;
    public Button tradeButton;
    public Button returnToMapButton;
    public Button deployButton;
    
    // 軍事行動のメインボタン
    public Button mainTrainingButton;
    public Button mainMoraleButton;
    public Button mainRecruitmentButton; // ExecuteRecruitment()を呼ぶボタン

    [Header("Status UI")]
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
    public TMPro.TextMeshProUGUI foodIncomeText;
    public TMPro.TextMeshProUGUI foodConsumptionText;
    public TMPro.TextMeshProUGUI goldIncomeText;
    public TMPro.TextMeshProUGUI goldConsumptionText;


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

    [Header("武将任命UI")]
    public GameObject warlordSelectionPanel; // WarlordSelectionPanelを接続
    public RectTransform warlordListContent; // ScrollViewのContentを接続
    public GameObject warlordItemPrefab;     // WarlordItemPrefabを接続

    private string currentAssignmentSector; // 現在任命中の部門 ("Agriculture"など)


    void Start()
    {
        // GameManagerから現在操作する城のコンポーネントを取得
        currentCity = GameManager.Instance.GetSelectedCityComponent();

        if (currentCity == null)
        {
            Debug.LogError("★エラー: 操作対象の城データが見つかりません！MapSceneで城が選択されたか確認してください。★");
            return; 
        }
    
        InitializeUI();
        UpdateCityUI();
        
        // 背景画像の切り替えロジック
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
        // アクションボタンのリスナー設定
        agricultureButton.onClick.RemoveAllListeners();
        commerceButton.onClick.RemoveAllListeners();
        tradeButton.onClick.RemoveAllListeners();

        agricultureButton.onClick.AddListener(ExecuteAgriculture);
        commerceButton.onClick.AddListener(ExecuteCommerce);
        tradeButton.onClick.AddListener(ExecuteTrade);
        
        returnToMapButton.onClick.RemoveAllListeners(); // 二重登録を防ぐ
        returnToMapButton.onClick.AddListener(ReturnToMap);

        // 軍事行動メインボタン
        mainTrainingButton.onClick.AddListener(ExecuteTraining);
        mainMoraleButton.onClick.AddListener(ExecuteMoraleBoost);
        mainRecruitmentButton.onClick.AddListener(ExecuteRecruitment);

        // 兵種トグルリスナー (排他制御)
        swordToggle.onValueChanged.AddListener((isOn) => { if (isOn) selectedTroopIndex = 1; });
        bowToggle.onValueChanged.AddListener((isOn) => { if (isOn) selectedTroopIndex = 2; });
        navyToggle.onValueChanged.AddListener((isOn) => { if (isOn) selectedTroopIndex = 3; });

        // 編成ボタンのリスナー (出陣確定)
        deployButton.onClick.AddListener(FinalizeDeployment);
    }
    public void ReturnToMap()
    {
        SceneManager.LoadScene("RyukyuMapScene"); // または "RyukyuMapScene"
    }

    public void UpdateCityUI()
    {
        // ★防御ロジック: currentCityがnullでないことを保証★
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
        // (他の unit 2, 3 も同様に更新するロジックが続く)
    }
    /*public void ExecuteAgriculture() // ★必ず 'public' であることを確認★
    {
        int cost = 100;
        int effect = 1;
        currentCity.PerformAgricultureAction(cost, effect); 

        UpdateCityUI(); // UIを再更新
    }*/

    public void ExecuteAgriculture()
    {
        if (currentCity == null) return;
        
        // ★★★ 修正箇所: レベルアップ処理を停止し、武将選択へ移行 ★★★
        
        currentAssignmentSector = "Agriculture"; // 部門を設定
        LoadWarlordList("Agriculture");         // 武将リストを生成
        warlordSelectionPanel.SetActive(true);  // パネルを表示
        
        // Debug.Log("武将選択モードに入りました。");
    }

    // 商業行動（市場開拓）の実行
    /*public void ExecuteCommerce() // ★必ず public であることを確認★
    {
        // 商業のコストと効果を定義
        int cost = 150;
        int effect = 1;

        // CityComponentに商業ロジックを処理させる
        string result = currentCity.PerformCommerceAction(cost, effect);

        // 成功・失敗メッセージをログに出力
        Debug.Log($"市場開拓結果: {result}");

        // UIを再更新
        UpdateCityUI();
    }*/

    public void ExecuteCommerce()
    {
        if (currentCity == null) return;
        
        // ★★★ 修正箇所: レベルアップ処理を停止し、武将選択へ移行 ★★★
        
        currentAssignmentSector = "Commerce"; // 部門を設定
        LoadWarlordList("Commerce");         // 武将リストを生成
        warlordSelectionPanel.SetActive(true);  // パネルを表示
        
        // Debug.Log("武将選択モードに入りました。");
    }




    // 交易行動（港整備）の実行
    /*public void ExecuteTrade() // ★必ず public であることを確認★
    {
        // 交易のコストと効果を定義 (ExecuteTrade(string target)のロジックとは別)
        int cost = 150;
        int effect = 1;

        // CityComponentに交易ロジックを処理させる
        string result = currentCity.PerformTradeAction(cost, effect);

        // 成功・失敗メッセージをログに出力
        Debug.Log($"港整備結果: {result}");

        // UIを再更新
        UpdateCityUI();
    }*/
    
    public void ExecuteTrade()
    {
        if (currentCity == null) return;
        
        // ★★★ 修正箇所: レベルアップ処理を停止し、武将選択へ移行 ★★★
        
        currentAssignmentSector = "Trade"; // 部門を設定
        LoadWarlordList("Trade");         // 武将リストを生成
        warlordSelectionPanel.SetActive(true);  // パネルを表示
        
        // Debug.Log("武将選択モードに入りました。");
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

    public void LoadWarlordList(string sector)
    {
        // リストをクリア
        foreach (Transform child in warlordListContent)
        {
            Destroy(child.gameObject);
        }

        // 現在選択中の城のIDを取得 (GameManagerで管理しているはず)
        // NOTE: あなたのプロジェクトの変数名に合わせて currentCity.cityLocationID または currentCity.locationID の部分を調整してください。
        // 2. CityData経由でLocationIDを取得
        Location selectedCityLocationID = currentCity.Data.cityLocationID;

        // TODO: ここでは、全武将リストを取得するロジックが必要です (GameManager.allGenerals)
        List<GeneralData> allGenerals = GameManager.Instance.allGenerals; // 全武将リストを想定

        // 3. ★★★ フィルタリングロジックの追加 ★★★
        List<GeneralData> localGenerals = allGenerals
            // 武将の現在地が、選択中の町と一致するかどうかでフィルタリング
            .Where(general => general.currentAssignedLocation == selectedCityLocationID)
            .ToList();

        // 4. フィルタリングされたリストに対してアイテムを生成
        foreach (GeneralData general in localGenerals)
        {
            GameObject itemObj = Instantiate(warlordItemPrefab, warlordListContent);
            WarlordItemController itemController = itemObj.GetComponent<WarlordItemController>();

            if (itemController != null)
            {
                // データを渡し、初期化
                itemController.Initialize(general, this, sector);
            }
        }
    }

    public void FinalizeWarlordAssignment(GeneralData selectedWarlord, string sector)
    {
        // 0. 現在の都市コンポーネントを取得 (currentCityはCityComponent型と仮定)
        CityComponent targetCity = currentCity;
        // 1. コストと効果のパラメータを定義
        int cost = 0;
        int incomeIncrease = 0;
        int baseAbility = 0; // 費用と効果の基準となる武将能力値
        // 2. セクター（内政種類）に基づき、コストと能力値を決定
        if (sector == "Agriculture") // 農地開拓
        {
            cost = 100; // 仮の開拓費
            baseAbility = selectedWarlord.politicalAbility; // 政治力を利用
        }
        else if (sector == "Commerce") // 市場開発
        {
            cost = 120; // 仮の開発費
            baseAbility = selectedWarlord.intelligence; // 知略を利用
        }
        else // その他の内政 (必要に応じて追加)
        {
            Debug.LogError("未知の内政セクター: " + sector);
            return;
        }

        // 3. コストの支払い確認
        if (GameManager.Instance.money < cost)
        {
            // 金が足りない場合の処理
            Debug.Log("金が不足しています。任命をキャンセルします。");
            // UIでエラーメッセージを表示する処理 (オプション)
            return;
        }

        // 4. 金の減少 (消費)
        GameManager.Instance.money -= cost;

        // 5. 収入増加量の計算
        // 収入増加 = 基本能力値 / 10 + 係数 (例として1.5倍)
        incomeIncrease = (int)(baseAbility * 1.5f);

        // 6. 収入の増加 (効果の発動)
        if (sector == "Agriculture")
        {
            // 食糧収入の増加
            targetCity.Data.foodIncome += incomeIncrease;

            // 農業レベルを武将の能力値に基づいて上昇させるロジック（以前の実装を再利用）
            GameManager.Instance.TryLevelUp(targetCity, "Agriculture", baseAbility);

            Debug.Log($"{targetCity.Data.cityName} の農業収入が {incomeIncrease} 増加しました。");
        }
        else if (sector == "Commerce")
        {
            // 金収入の増加
            targetCity.Data.goldIncome += incomeIncrease;

            // 商業レベルを武将の能力値に基づいて上昇させるロジック
            GameManager.Instance.TryLevelUp(targetCity, "Commerce", baseAbility);

            Debug.Log($"{targetCity.Data.cityName} の商業収入が {incomeIncrease} 増加しました。");
        }

        // 7. 武将の任命状態を更新 (この武将は今ターン活動済みとする)
        selectedWarlord.isBusy = true; // 仮のフラグ設定

        // 8. パネルを閉じる
        warlordSelectionPanel.SetActive(false);

        // 9. UIの更新をGameManagerに要求 (全体の資源表示を更新)
        GameManager.Instance.UpdateAllUI(); // 仮のメソッド名
    }
    
    /*
        // 1. 担当武将を任命
        // TODO: ここで GameManager.currentAgricultureWarlord = selectedWarlord のような割り当てロジックが必要
        
        // 2. レベルアップの実行
        string log;
        // GameManager.TryLevelUp(sector, out log); // レベルアップロジックの呼び出し
        
        // 3. 交易の場合、能力値成長を実行
        if (sector == "Trade")
        {
            // log += GameManager.Instance.RandomlyImproveWarlordStat(selectedWarlord); // 能力成長ロジックの呼び出し
        }

        // 4. UIの更新とパネルを閉じる
        // logMessageText.text = log; // ログ表示
        warlordSelectionPanel.SetActive(false);
        UpdateCityUI();
    }*/


    
    // ★★★ 武将リストの生成とフィルタリング ★★★
    public void LoadGeneralList()
    {
        // 1. リスト要素をすべてクリア (Contentをリセット)
        foreach (Transform child in generalListContent)
        {
            Destroy(child.gameObject);
        }
    
        // 2. 現在の城の武将リストを取得 (Location Enumに基づいてフィルタリング)
        string currentCityName = currentCity.Data.cityName;
        List<GeneralData> localGenerals = GameManager.Instance.GetGeneralsInCity(currentCityName);

        if (localGenerals == null || localGenerals.Count == 0)
        {
            Debug.Log("この城には現在、出陣可能な武将がいません。");
            return;
        }

        // 3. リストの動的生成
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
            // 5. 【マップ上へのユニット配置】
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
            UpdateCityUI();
        }
    }
    
    // CityUIManager.cs のクラス内に追加

// 補助関数 (兵種インデックスに基づき、適切なユニットプレファブを返す)
    private GameObject GetUnitPrefabByTroopIndex(int index)
    {
        // ★暫定的なロジック: 実際のプレファブはGameManagerに接続する必要があります★
        // (ここでは、仮にResourcesからロードするロジックを再定義します)
        if (index == 1) return Resources.Load<GameObject>("Prefabs/Unit_Sword_Prefab"); 
        if (index == 2) return Resources.Load<GameObject>("Prefabs/Unit_Bow_Prefab"); 
        if (index == 3) return Resources.Load<GameObject>("Prefabs/Unit_Navy_Prefab"); 
        
        // エラーを防ぐため、GameManagerに接続されているデフォルトのプレファブを返します
        return GameManager.Instance.MilitaryUnitPrefab; 
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