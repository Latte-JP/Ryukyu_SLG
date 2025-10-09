using TMPro; // ★これが重要★
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CityUIManager : MonoBehaviour
{
    private CityComponent currentCity;
    
    // UI要素をInspectorから接続
    public Text cityNameText;
    public Text goldText;
    public Button agricultureButton;
    public Button returnToMapButton;

    [Header("ステータス表示UI")]
    public TextMeshProUGUI cityNameDisplay; // CityNameDisplayオブジェクトを接続
    public TextMeshProUGUI goldDisplay;     // GoldDisplayオブジェクトを接続
    public TextMeshProUGUI foodDisplay;     // FoodDisplayオブジェクトを接続

    void Start()
    {
        // GameManagerから現在操作する城のコンポーネントを取得
        currentCity = GameManager.Instance.GetSelectedCityComponent();

        if (currentCity != null)
        {
            InitializeUI();
            UpdateCityUI();
        }
        else
        {
            Debug.LogError("操作対象の城データが見つかりません！");
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