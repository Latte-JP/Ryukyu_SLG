using TMPro; // ★これが重要★
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic; 

public class CityUIManager : MonoBehaviour
{
    private CityComponent currentCity;
    
    // UI要素をInspectorから接続
    //public Text cityNameText;
    //public Text goldText;
    public Button agricultureButton;
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



    [Header("背景ビジュアル")]
    public UnityEngine.UI.RawImage backgroundRawImage; // Scene内のRawImageに接続
    public List<Texture2D> backgroundTextures;        // Inspectorから全てのテクスチャを接続
    public List<string> textureIDs;                   // 対応するID（"IMAKIJIN_GUSUKU"など）

    [Header("ステータス表示UI")]
    public TextMeshProUGUI cityNameDisplay; // CityNameDisplayオブジェクトを接続
    public TextMeshProUGUI goldDisplay;     // GoldDisplayオブジェクトを接続
    public TextMeshProUGUI foodDisplay;     // FoodDisplayオブジェクトを接続

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
        returnToMapButton.onClick.AddListener(ReturnToMap);

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

        // TODO: 人口、農業レベル、文化度などの表示をここに追加
        // populationDisplay.text = $"人口: {data.population:N0}"; 
        // ★★★ 新しいステータスを更新 ★★★
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
            Debug.Log($"交易成功！金 +{goldGain}。成功率: {finalSuccessRate*100:F1}%");
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
}