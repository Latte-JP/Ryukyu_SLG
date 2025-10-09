using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理のために追加

public class CityClickDetector : MonoBehaviour
{
    private CityComponent cityComponent;

    void Start()
    {
        cityComponent = GetComponent<CityComponent>();
    }

    // マウスがオブジェクト上をクリックしたときに呼び出される
    void OnMouseDown()
    {
        if (cityComponent != null)
        {
            // ★★★ 修正箇所 ★★★
            // 1. GameManagerがロードされているか確認
            if (GameManager.Instance == null)
            {
                Debug.LogError("FATAL ERROR: GameManager.InstanceがNULLです。マップシーンの起動順序やGameManagerのAwakeを確認してください。");
                return; // GameManagerがない場合、即座に処理を中断
            }

            // 2.GameManagerに、どの城を操作するかを記憶させる
            GameManager.Instance.SetSelectedCity(cityComponent.Data.cityName); 
            
            // 3.CitySceneをロード
            SceneManager.LoadScene("CityScene"); 
            // 4. ログ出力（シーン遷移後にコンソールがクリアされる可能性あり）
            Debug.Log($"城 {cityComponent.Data.cityName} が選択され、CitySceneに遷移します。");
        }
    }
}