using UnityEngine;
using UnityEngine.UI; // UIコンポーネントを使うために必要

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // シングルトン
    public GameObject cityInfoPanel; // Inspectorから接続するパネル

    [Header("UI要素（パネル内）")]
    public Text cityNameText;
    public Text goldText;
    public Button agricultureButton; // 農業ボタン

    private CityComponent selectedCity; // 現在選択中の城

    void Awake() { Instance = this; }

    // 城がクリックされたときに呼び出される
    public void ShowCityPanel(CityComponent city)
    {
        selectedCity = city;
        cityInfoPanel.SetActive(true); // パネルを表示
        UpdateCityPanelUI();
    }

    // パネルの情報を選択中の城データで更新
    public void UpdateCityPanelUI()
    {
        if (selectedCity == null) return;

        cityNameText.text = selectedCity.Data.cityName;
        goldText.text = "金: " + selectedCity.Data.goldStock.ToString();

        // 農業ボタンのリスナーを再設定
        agricultureButton.onClick.RemoveAllListeners();
        agricultureButton.onClick.AddListener(ExecuteAgriculture);
    }

    // 農業ボタンが押されたときの処理
    public void ExecuteAgriculture()
    {
        int cost = 100;
        int effect = 1;
        
        selectedCity.PerformAgricultureAction(cost, effect); // CityComponentのメソッドを呼び出し
        
        UpdateCityPanelUI(); // UIを再更新
    }
}