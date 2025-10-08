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
            // GameManagerに、どの城を操作するかを記憶させる
            GameManager.Instance.SetSelectedCity(cityComponent.Data.cityName); 
            
            // CitySceneをロード
            SceneManager.LoadScene("CityScene"); 
            
            Debug.Log($"城 {cityComponent.Data.cityName} が選択され、CitySceneに遷移します。");
        }
    }
}