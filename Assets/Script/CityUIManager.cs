using TMPro; // ★これが重要★
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class CityUIManager : MonoBehaviour
{
    private CityComponent currentCity;

    // UI要素をInspectorから接続
    //public Text cityNameText;
    //public Text goldText;
    public Button agricultureButton;
    public Button commerceButton;
    public Button tradeButton;
    public Button returnToMapButton;

    [Header("ステータステーブル")]
    public TMPro.TextMeshProUGUI foodIncomeText;
    public TMPro.TextMeshProUGUI foodConsumptionText;
    public TMPro.TextMeshProUGUI goldIncomeText;
    public TMPro.TextMeshProUGUI goldConsumptionText;

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
    public TMPro.TextMeshProUGUI agricultureLevelText;
    public TMPro.TextMeshProUGUI commerceLevelText;
    public TMPro.TextMeshProUGUI tradeLevelText;
    // 訓練と交流と募兵のモードを定義
    public enum MilitaryActionMode { None, Training, MoraleBoost, Recruitment }
    // 現在のモードを記憶する変数
    private MilitaryActionMode currentMode = MilitaryActionMode.None;

    [Header("背景ビジュアル")]
    public UnityEngine.UI.RawImage backgroundRawImage; // Scene内のRawImageに接続
    public List<Texture2D> backgroundTextures;        // Inspectorから全てのテクスチャを接続
    public List<string> textureIDs;                   // 対応するID（"IMAKIJIN_GUSUKU"など）

    [Header("ステータス表示UI")]
    public TextMeshProUGUI cityNameDisplay; // CityNameDisplayオブジェクトを接続
    public TextMeshProUGUI goldDisplay;     // GoldDisplayオブジェクトを接続
    public TextMeshProUGUI foodDisplay;     // FoodDisplayオブジェクトを接続
    public TextMeshProUGUI populationDisplay;     // PopulationDisplayオブジェクトを接続

    [Header("軍事行動UI")]
    // ★この変数にTroopSelectionPanelを接続★
    public GameObject troopSelectionPanel; 
    public Button mainTrainingButton;
    public Button mainMoraleButton; 
    
    [Header("部隊編成UI")]
    public GameObject deploymentPanel; // DeploymentPanelを接続
    //public Transform generalListContent; // GeneralListPanelの子要素を配置する親 (Content)
    public TMPro.TMP_InputField troopInputField; // TroopInput_Fieldを接続
    public Button deployButton; // DeployButtonを接続

    private GeneralData selectedGeneral; // 現在選択中の武将データ
    public GameObject generalListPanel;
    public GameObject troopInputPanel;

    [Header("編成リスト用参照")]
    public GameObject generalItemPrefab; // GeneralItemPrefabを接続
    public RectTransform generalListContent; // ScrollViewのContentオブジェクトを接続

    public Toggle swordToggle;
    public Toggle bowToggle;
    public Toggle navyToggle;
    // 選択された兵種インデックス (1, 2, or 3)
    private int selectedTroopIndex = 1;


    void Start()
    {
        // GameManagerから現在操作する城のコンポーネントを取得
        currentCity = GameManager.Instance.GetSelectedCityComponent();

        if (currentCity == null)
        {
            // 処理が失敗した場合、コンソールにエラーを出し、ここで処理を中断する
            Debug.LogError("★エラー: 操作対象の城データが見つかりません！MapSceneで城が選択されたか確認してください。★");
            return; // ★重要: currentCityがnullの場合、ここで処理を中断する★
        }
    
        InitializeUI();
        UpdateCityUI();
        // ★★★ 修正箇所：背景画像の切り替え ★★★
        string targetID = currentCity.Data.backgroundSceneID;
        // textureIDsリスト内でtargetIDを検索
        int index = textureIDs.IndexOf(targetID);
        if (index >= 0 && index < backgroundTextures.Count)
        {
            // 対応するテクスチャをRawImageに設定
            backgroundRawImage.texture = backgroundTextures[index];
            Debug.Log($"背景を {targetID} に変更しました。");
        }
        else
        {
            Debug.LogWarning($"背景ID '{targetID}' のテクスチャが見つかりません。");
        }
    }

    private void InitializeUI()
    {
        // UIボタンのリスナー設定
        agricultureButton.onClick.AddListener(ExecuteAgriculture);
        commerceButton.onClick.AddListener(ExecuteCommerce);
        tradeButton.onClick.AddListener(ExecuteTrade);
        returnToMapButton.onClick.AddListener(ReturnToMap);
        // トグルが切り替わったときのイベントリスナーを設定
        swordToggle.onValueChanged.AddListener((isOn) => { if (isOn) selectedTroopIndex = 1; });
        bowToggle.onValueChanged.AddListener((isOn) => { if (isOn) selectedTroopIndex = 2; });
        navyToggle.onValueChanged.AddListener((isOn) => { if (isOn) selectedTroopIndex = 3; });
        
        // 編成ボタンのリスナーを設定
        deployButton.onClick.AddListener(FinalizeDeployment);
        // TODO: 他のボタン (商業、交易、訓練など) もここに追加
    }
// パネルの情報を選択中の城データで更新
    public void UpdateCityUI()
    {
        if (currentCity == null) return;

        // データ (CityData) の参照を簡潔にする
        CityData data = currentCity.Data;

        // === Textコンポーネメントへのデータ反映 ===
        
        // 1. 城の名前
        cityNameDisplay.text = data.cityName;

        // 2. 金 (GoldStock)
        // 桁区切りなどを入れたい場合は ToString("N0") などを使用
        goldDisplay.text = $"{data.goldStock:N0}";

        // 3. 食糧 (FoodStock)
        foodDisplay.text = $"{data.foodStock:N0}";
        // 4. 人口 (currentPopulation)
        populationDisplay.text = $"{data.population:N0}";
        // TODO: 人口、農業レベル、文化度などの表示をここに追加
        // populationDisplay.text = $"人口: {data.population:N0}"; 
        // ★★★ 新しいステータスを更新 ★★★
        if (agricultureLevelText != null)
        {
            // 農業レベルの値をUIに表示する
            agricultureLevelText.text = data.agricultureLevel.ToString();
        }
        
        if (commerceLevelText != null)
        {
            // 商業レベルの値をUIに表示する
            commerceLevelText.text = data.commerceLevel.ToString();
        }       
        if (commerceLevelText != null)
        {
            // 交易レベルの値をUIに表示する
            tradeLevelText.text = data.tradeLevel.ToString();
        }       



        foodIncomeText.text      = $"{currentCity.Data.foodIncome}";
        foodConsumptionText.text = $"{currentCity.Data.foodConsumption}";
        goldIncomeText.text      = $"{currentCity.Data.goldIncome}";
        goldConsumptionText.text = $"{currentCity.Data.goldConsumption}";

        unitTypeText1.text        = $"{currentCity.Data.unitType1}";
        unitCountText1.text       = $"{currentCity.Data.unitCount1}";
        trainingLevelText1.text   = $"{currentCity.Data.trainingLevel1}%";
        moraleText1.text          = $"{currentCity.Data.morale1}%";
        unitTypeText2.text        = $"{currentCity.Data.unitType2}";
        unitCountText2.text       = $"{currentCity.Data.unitCount2}";
        trainingLevelText2.text   = $"{currentCity.Data.trainingLevel2}%";
        moraleText2.text          = $"{currentCity.Data.morale2}%";
        unitTypeText3.text        = $"{currentCity.Data.unitType3}";
        unitCountText3.text       = $"{currentCity.Data.unitCount3}";
        trainingLevelText3.text   = $"{currentCity.Data.trainingLevel3}%";
        moraleText3.text          = $"{currentCity.Data.morale3}%";        
        // ... (ボタンリスナーの再設定などの既存ロジック)
    }
    //    public void UpdateCityUI()
    //  {
    //    cityNameText.text = currentCity.Data.cityName;
    //  goldText.text = "金: " + currentCity.Data.goldStock.ToString();
    // TODO: 他のステータス表示もここに追加
    //}

    // 農業行動の実行
    public void ExecuteAgriculture()
    {
        int cost = 100;
        int effect = 1;
        currentCity.PerformAgricultureAction(cost, effect); // CityComponentのメソッド呼び出し

        UpdateCityUI(); // UIを再更新
    }

    // 商業行動（市場開拓）の実行
    public void ExecuteCommerce()
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
    }
    
        // 交易行動（市場開拓）の実行
    public void ExecuteTrade()
    {
        // 交易のコストと効果を定義
        int cost = 150;
        int effect = 1;

        // CityComponentに交易ロジックを処理させる
        string result = currentCity.PerformTradeAction(cost, effect);
        
        // 成功・失敗メッセージをログに出力
        Debug.Log($"港整備結果: {result}");

        // UIを再更新
        UpdateCityUI();
    }

    // マップシーンに戻る
    public void ReturnToMap()
    {
        SceneManager.LoadScene("MapScene");
    }
    // 武将の知略・スキルから交易成功率のボーナスを計算
    private float CalculateNegotiationBonus(GeneralData general)
    {
        if (general == null) return 0f;

        float bonus = general.intelligence * 0.002f; // 知略100で+20%ボーナス
    
        // スキル「MasterTrader」の効果を適用
        if (general.skill == SpecialSkill.MasterTrader)
        {
             bonus += 0.15f; // 固定でさらに+15%
        }
        return bonus;
    }

    // 武将の文化力から技術獲得率のボーナスを計算
    private float CalculateCultureBonus(GeneralData general)
    {
        if (general == null) return 0f;

        float bonus = general.culture * 0.001f; // 文化力100で+10%ボーナス
        return bonus;
    }
    // CityUIManager.cs の ExecuteTrade(string target) メソッド

    public void ExecuteTrade(string target)
    {
        int tradeCost = 500;
        if (currentCity.Data.goldStock < tradeCost)
        {
            Debug.Log("交易に必要な金が不足しています。");
            return;
        }
        currentCity.Data.goldStock -= tradeCost; // コスト消費

        // 城代武将の能力値を取得
        GeneralData cityGeneral = currentCity.Data.governingGeneral;

        // 1. 成功率の計算
        float negotiationBonus = CalculateNegotiationBonus(cityGeneral);
        float cultureBonus = CalculateCultureBonus(cityGeneral);

        float baseSuccessRate = 0.50f;
        // 最終成功率 = 基本 + 交渉ボーナス + 交易レベル
        float finalSuccessRate = baseSuccessRate + negotiationBonus + (currentCity.Data.tradeLevel * 0.01f);

        // 2. 技術獲得確率の計算
        float baseTechChance = 0.03f;
        float finalTechChance = baseTechChance + cultureBonus + (currentCity.Data.tradeLevel * 0.005f);

        // 3. 交易結果の判定
        if (Random.value < finalSuccessRate)
        {
            // === 交易成功 ===
            int goldGain = 1000 + (currentCity.Data.commerceLevel * 50);
            currentCity.Data.goldStock += goldGain;
            currentCity.Data.tradeLevel++;

            // 技術入手判定 (明国との交易を想定)
            if (target == "Ming" && !currentCity.Data.hasIronGunTech && Random.value < finalTechChance)
            {
                currentCity.Data.hasIronGunTech = true;
                Debug.Log("交易成功！★鉄砲技術を入手しました！★");
            }
            Debug.Log($"交易成功！金 +{goldGain}。成功率: {finalSuccessRate * 100:F1}%");
        }
        else
        {
            // === 交易失敗とリスク上昇 ===
            int riskLoss = currentCity.Data.goldStock / 20; // 5%の金損失
            currentCity.Data.goldStock -= riskLoss;
            currentCity.Data.tradeRiskFactor += 0.15f; // リスク上昇

            Debug.Log($"交易失敗。金 {riskLoss} を失い、交易リスクが上昇しました。");
        }

        UpdateCityUI();
    }

    public void ExecuteTraining()
    {
        currentMode = MilitaryActionMode.Training; // ★モードを訓練に設定★
        // ★TODO: ここで兵種選択パネル（例: TroopSelectionPanel）を表示する
        if (troopSelectionPanel != null)
        {
            troopSelectionPanel.SetActive(true);
        }
        Debug.Log("訓練ボタンが押されました。どの兵種を訓練するか選択してください。");
        // テストのため、ここでは強制的に1番目の兵種（剣兵など）を選択して実行します
        // ExecuteTroopTraining(1); // 実際のゲームでは削除
    }

    // 交流ボタンが押されたとき（兵種選択パネルを表示する）
    public void ExecuteMoraleBoost()
    {
        currentMode = MilitaryActionMode.MoraleBoost; // ★モードを交流に設定★
        // ★TODO: ここで兵種選択パネル（例: TroopSelectionPanel）を表示する
        if (troopSelectionPanel != null)
        {
            troopSelectionPanel.SetActive(true);
        }
        Debug.Log("交流ボタンが押されました。どの兵種と交流するか選択してください。");

        // テストのため、ここでは強制的に2番目の兵種（弓兵など）を選択して実行します
        // ExecuteTroopMoraleBoost(2); // 実際のゲームでは削除
    }
    // 募兵ボタンが押されたとき（兵種選択パネルを表示する）
    public void ExecuteRecruitment()
    {
        currentMode = MilitaryActionMode.Recruitment; // ★モードを募兵に設定★
        if (troopSelectionPanel != null)
        {
            troopSelectionPanel.SetActive(true);
        }
        Debug.Log("募兵モードに入ります。どの兵種を募兵するか選択してください。");
    }
    public void LoadGeneralList()
    {
        // 1. リスト要素をすべてクリア (スクロールリストの Content をリセット)
        // generalListContentはScroll ViewのContent RectTransformです
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
            // TODO: ここに「武将不在」メッセージを表示するUIロジックを追加
            return;
        }

        // 3. リストの動的生成
        foreach (GeneralData general in localGenerals)
        {
            // generalItemPrefab: 作成した武将アイテムのUIプレファブ
            // generalListContent: ScrollViewのContent RectTransform (生成場所)
            GameObject itemObj = Instantiate(generalItemPrefab, generalListContent);
            
            // 4. ItemControllerにデータを渡し、初期化
            GeneralItemController itemController = itemObj.GetComponent<GeneralItemController>();
            if (itemController != null)
            {
                // ItemControllerのInitializeメソッドを呼び出す
                itemController.Initialize(general, this); 
            }
        }
    
        Debug.Log($"【編成リスト】{localGenerals.Count}名の武将をロードしました。");
    }


    public void ExecuteDeploymentMode()
    {
        if (deploymentPanel != null)
        {
            deploymentPanel.SetActive(true);
            // パネルを開く際に武将リストを生成・更新するメソッドを呼び出す
            LoadGeneralList();
        }
    }
    // CityUIManager.cs に追加

    public void FinalizeDeployment()
    {
        if (selectedGeneral == null)
        {
            Debug.LogError("大将が選択されていません。");
            return;
        }
    
        int troopCount = 0;
        // 兵数入力フィールドから値を取得し、数値に変換
        if (!int.TryParse(troopInputField.text, out troopCount))
        {
            Debug.LogError("有効な兵数を入力してください。");
            return;
        }

        // 選択された兵種インデックス (1, 2, or 3)
        int troopIndex = selectedTroopIndex;

        // CityComponent.DeployTroop()の呼び出し
        TroopData newTroop = currentCity.DeployTroop(selectedDeploymentGeneral, troopIndex, troopCount);
        if (newTroop != null)
        {
            // 1. マップ上の配置座標を都市から取得 (CityComponentの座標を使用)
            Vector3 deployPosition = currentCity.transform.position + Vector3.up * 0.5f; // 地面から少し浮かせる        
            // 2. UnitPrefabを生成
            GameObject unitObj = Instantiate(GameManager.Instance.MilitaryUnitPrefab, deployPosition, Quaternion.identity);
            // 3. UnitControllerを取得し、初期化
            UnitController unitController = unitObj.GetComponent<UnitController>();
            if (unitController != null)
            {
                unitController.Initialize(newTroop, deployPosition, currentCity.Data.cityName);
            }
            Debug.Log($"【編成成功】{newTroop.unitName} 部隊が出陣準備完了。");
            deploymentPanel.SetActive(false);
        }

    }

    public void ExecuteTroopAction(int troopIndex)
    {
        string result = "ERROR: 不明な操作モードです。";
        int recruitAmount = 500; // 募兵数
        int goldCost = recruitAmount * 2; // 兵士1人あたり2金
        int foodCost = recruitAmount * 3; // 兵士1人あたり3食糧

        if (currentMode == MilitaryActionMode.Recruitment)
        {
            // ★募兵モードの場合のロジックを実行
            result = currentCity.PerformRecruitment(troopIndex, goldCost, foodCost, recruitAmount);
        }
        else if (currentMode == MilitaryActionMode.Training)
        {
            // 訓練モードの場合、訓練ロジックを実行
            result = currentCity.PerformTraining(troopIndex, 200, 10);
        }
        else if (currentMode == MilitaryActionMode.MoraleBoost)
        {
            // 交流モードの場合、交流ロジックを実行
            result = currentCity.PerformMoraleBoost(troopIndex, 150, 15);
        }


        Debug.Log($"[{currentMode}結果] {result}");

        // 後処理(UI更新とパネル非表示)
        UpdateCityUI();
        if (troopSelectionPanel != null)
        {
            troopSelectionPanel.SetActive(false);
        }
        currentMode = MilitaryActionMode.None; // モードをリセット
    }

    private GeneralData selectedDeploymentGeneral; // 出陣のために選択された武将

    public void SetSelectedGeneral(GeneralData general)
    {
        selectedDeploymentGeneral = general;

        // 選択された武将の情報を画面の別の場所（例: 確定エリア）に表示
        // (例: finalGeneralNameText.text = general.generalName;)
        // (A) 武将リストパネルを非表示にする (Scroll View自体、またはその親)
        if (generalListContent != null)
        {
            // ScrollViewのContentの親を非表示にするか、Contentの上にあるPanelを非表示にします。
            // ここでは、Scroll View全体（GeneralScrollView）を非表示にすることを推奨
            generalListContent.parent.parent.gameObject.SetActive(false); // Hierarchy構造に合わせて修正が必要
        }
        // (B) 兵数入力UI（TroopInputPanelなど）を表示する
        if (troopInputPanel != null) // TroopInputPanelをCityUIManagerに追加し接続が必要
        {
            generalListPanel.SetActive(false);  // 武将リストを非表示
            troopInputPanel.SetActive(true);    // 兵数入力を表示
        }

        Debug.Log($"大将として {general.generalName} を選択しました。");
    }
}