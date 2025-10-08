using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Listの操作に必要

public class GameManager : MonoBehaviour
{
    // シングルトン化のための静的変数
    public static GameManager Instance { get; private set; }
    public string selectedCityName; // 選択された城の名前を保持

    [Header("ゲーム情報")]
    public int currentTurn = 1;
    public int currentYear = 1400; // 例: 三山時代の開始年
    public int turnPerYear = 12; // 1年を12ターン（月）とする
    
    // 全ての城コンポーネントを管理するためのリスト
    public List<CityComponent> allCities = new List<CityComponent>();

    void Awake()
    {
        // シングルトン設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 初期化時に全てのCityComponentをリストに登録
        allCities = FindObjectsOfType<CityComponent>().ToList();
        Debug.Log($"ゲーム開始。管理対象の城: {allCities.Count} 箇所");
    }

    // === ターン終了処理のメソッド ===
    public void EndTurn()
    {
        // 1. 全ての城の毎月（ターン）処理を実行
        foreach (var city in allCities)
        {
            ApplyCityUpdates(city.Data);
        }

        // 2. ターンを進行
        currentTurn++;
        if (currentTurn > turnPerYear)
        {
            currentTurn = 1;
            currentYear++;
            Debug.Log($"新しい年 {currentYear} が始まりました！");
        }

        Debug.Log($"ターン {currentTurn} が終了しました。");

        // 3. UIの更新通知（後述のUIシステムで利用）
        // UIManager.Instance.UpdateUI(); 
    }

    // 城のステータス更新ロジック
    private void ApplyCityUpdates(CityData data)
    {
        // 資源の自動増減
        data.foodStock += data.agricultureLevel * 10 - data.population / 100; // 農業レベルと人口消費
        data.goldStock += data.commerceLevel * 5 - data.swordTroops.count / 50; // 商業レベルと兵の維持費
        
        // 人口増加
        data.population = Mathf.RoundToInt(data.population * (1f + data.populationGrowthRate));

        // TODO: 士気低下、文化度変動、疫病・イベント判定などをここに追加
    }
    public void SetSelectedCity(string cityName)
    {
    selectedCityName = cityName;
    }

    public CityComponent GetSelectedCityComponent()
    {
    // selectedCityNameを使用して、allCitiesリストから該当するCityComponentを検索して返す
    return allCities.Find(city => city.Data.cityName == selectedCityName);
    }
}