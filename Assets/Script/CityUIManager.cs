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
}