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

    public void UpdateCityUI()
    {
        cityNameText.text = currentCity.Data.cityName;
        goldText.text = "金: " + currentCity.Data.goldStock.ToString();
        // TODO: 他のステータス表示もここに追加
    }

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