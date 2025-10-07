using UnityEngine;

// 城アイコン（プレファブ）にアタッチする
public class CityComponent : MonoBehaviour
{
    // 1. CityDataのインスタンスを保持
    // [SerializeField]とすることで、インスペクターから直接CityDataの値を編集可能
    [SerializeField] 
    public CityData Data;

    // 初期化メソッド
    public void InitializeCity(CityData initialData)
    {
        Data = initialData;
        gameObject.name = "City: " + Data.cityName; // オブジェクト名をデータに合わせて変更
    }

    // 内政行動の実行（例：農業）
    public void PerformAgricultureAction(int cost, int effect)
    {
        if (Data.goldStock >= cost)
        {
            Data.goldStock -= cost;
            Data.agricultureLevel += effect;
            Debug.Log($"{Data.cityName}で農業が実行されました。金消費: {cost}、農業レベル: {Data.agricultureLevel}");
        }
        else
        {
            Debug.Log($"金が足りません。農業を実行できませんでした。");
        }
    }

    // 他の内政・軍事行動 (商業、交易、訓練など) も同様にメソッド化していく
}