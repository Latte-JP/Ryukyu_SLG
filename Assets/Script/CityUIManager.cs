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

    public TMPro.TextMeshProUGUI unitTypeText;
    public TMPro.TextMeshProUGUI unitCountText;
    public TMPro.TextMeshProUGUI trainingLevelText;
    public TMPro.TextMeshProUGUI moraleText;



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
        goldDisplay.text = $"金: {data.goldStock:N0}";

        // 3. 食糧 (FoodStock)
        foodDisplay.text = $"食糧: {data.foodStock:N0}"; 

        // TODO: 人口、農業レベル、文化度などの表示をここに追加
        // populationDisplay.text = $"人口: {data.population:N0}"; 
            // ★★★ 新しいステータスを更新 ★★★
    foodIncomeText.text      = $"食糧収入: {currentCity.Data.foodIncome}";
    foodConsumptionText.text = $"食糧消費: {currentCity.Data.foodConsumption}";
    goldIncomeText.text      = $"金収入: {currentCity.Data.goldIncome}";
    goldConsumptionText.text = $"金消費: {currentCity.Data.goldConsumption}";

    unitTypeText.text        = $"兵種: {currentCity.Data.unitType1}";
    unitCountText.text       = $"兵数: {currentCity.Data.unitCount1}";
    trainingLevelText.text   = $"訓練度: {currentCity.Data.trainingLevel1}%";
    moraleText.text          = $"士気: {currentCity.Data.morale1}%";
        
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