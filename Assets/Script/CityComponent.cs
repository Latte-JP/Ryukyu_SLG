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
    // ----------------------------------------------------
    // 兵種を指定して訓練を実行
    // ----------------------------------------------------
    public string PerformTraining(int troopIndex, int goldCost, int effect)
    {
        if (Data.goldStock < goldCost)
        {
            return "ERROR: 資金不足";
        }

        // ★★★ 修正箇所1: ローカル変数を宣言し、参照ポインタの初期化はしない ★★★
        ref int targetTraining = ref Data.trainingLevel1;
        ref int targetMorale = ref Data.morale1;
        string unitName = "";

        // どの兵種を対象にするか、インデックスで分岐
        switch (troopIndex)
        {
            case 1:
                targetTraining = ref Data.trainingLevel1;
                targetMorale = ref Data.morale1;
                unitName = Data.unitType1;
                break;
            case 2:
                targetTraining = ref Data.trainingLevel2;
                targetMorale = ref Data.morale2;
                unitName = Data.unitType2;
                break;
            case 3:
                targetTraining = ref Data.trainingLevel3;
                targetMorale = ref Data.morale3;
                unitName = Data.unitType3;
                break;
            default:
                return "ERROR: 不正な兵種インデックス";
        }
    
        Data.goldStock -= goldCost;
    
        // 訓練度の更新 (最大100まで)
        targetTraining = Mathf.Min(100, targetTraining + effect);
    
        // 訓練度の上昇は士気を一時的に低下させる
        targetMorale = Mathf.Max(0, targetMorale - (effect / 2)); 

        return $"{unitName}の訓練を実行。訓練度: {targetTraining} (+{effect})。士気は{targetMorale}に低下。";
    }

    // ----------------------------------------------------
    // 【新しいメソッド】兵種を指定して交流を実行
    // ----------------------------------------------------
    public string PerformMoraleBoost(int troopIndex, int goldCost, int effect)
    {
        if (Data.goldStock < goldCost)
        {
            return "ERROR: 資金不足";
        }
        // ★★★ 修正箇所1: ローカル変数を宣言し、参照ポインタの初期化はしない ★★★
        ref int targetTraining = ref Data.trainingLevel1; // 初期値は暫定的に1を設定
        ref int targetMorale = ref Data.morale1;        // 初期値は暫定的に1を設定
        string unitName = "";

        // どの兵種を対象にするか、インデックスで分岐
        switch (troopIndex)
        {
            case 1:
                targetTraining = ref Data.trainingLevel1;
                targetMorale = ref Data.morale1;
                unitName = Data.unitType1;
                break;
            case 2:
                targetTraining = ref Data.trainingLevel2;
                targetMorale = ref Data.morale2;
                unitName = Data.unitType2;
                break;
            case 3:
                targetTraining = ref Data.trainingLevel3;
                targetMorale = ref Data.morale3;
                unitName = Data.unitType3;
                break;
            default:
                return "ERROR: 不正な兵種インデックス";
        }
    
        Data.goldStock -= goldCost;

        // 士気の更新 (最大100まで)
        targetMorale = Mathf.Min(100, targetMorale + effect);
    
        // 士気の上昇は訓練の集中力を一時的に低下させる
        targetTraining = Mathf.Max(0, targetTraining - (effect / 3));

        return $"{unitName}との交流を実行。士気: {targetMorale} (+{effect})。訓練度は{targetTraining}に低下。";
    }

    // 他の内政・軍事行動 (商業、交易、訓練など) も同様にメソッド化していく
}