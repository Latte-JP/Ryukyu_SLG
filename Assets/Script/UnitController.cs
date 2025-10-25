using UnityEngine;

public class UnitController : MonoBehaviour
{
    [Header("部隊データ")]
    public TroopData troopData;         // 編成された部隊の全データ
    public Vector3 targetPosition;      // ユニットが移動すべき目標座標
    public int remainingMovement = 0;   // 残り移動力 (ターンごとにリセット)
    public string currentCityName = ""; // 部隊が最後にいた都市名

    // ユニットの初期化 (CityUIManagerから呼ばれる)
    public void Initialize(TroopData data, Vector3 startPos, string city)
    {
        this.troopData = data;
        this.currentCityName = city;
        this.transform.position = startPos;
        this.name = $"Unit: {data.general.generalName} ({data.unitName})";
        
        // 最初のターン移動力を設定
        ResetMovement(); 
    }

    // 毎ターン移動力をリセット
    public void ResetMovement()
    {
        // 知略と部隊の訓練度に基づいて移動力を計算
        int baseMove = 5; // 基本移動力 (マス数)
        int bonus = (int)(troopData.general.intelligence * 0.05f + troopData.training * 0.02f);
        remainingMovement = baseMove + bonus;
    }

    // ユニットを次のマスへ移動させるロジック (戦闘システムが使用)
    public void MoveTo(Vector3 position)
    {
        // 1. 移動コストを計算 (地形によるペナルティ、海人隊のボーナスなど)
        int movementCost = 1; // 仮のコスト
        
        if (movementCost <= remainingMovement)
        {
            this.targetPosition = position;
            remainingMovement -= movementCost;
            // 実際に移動をアニメーションさせる処理（ここでは省略）
            Debug.Log($"{troopData.unitName}を移動。残り移動力: {remainingMovement}");
        }
        else
        {
            Debug.Log("移動力が不足しています。");
        }
    }
}