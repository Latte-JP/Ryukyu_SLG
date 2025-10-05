using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMap : MonoBehaviour
{
    // この関数をボタンのクリックイベントに設定する
    public void GoBackToMap()
    {
        SceneManager.LoadScene("RyukyuMapScene"); // " "の中は自分のマップシーンの名前に変更してください
    }
}