using UnityEngine;

public class BagUI : MonoBehaviour
{
    public GameObject bagPanel;
    public GameObject craftPanel;   // 合成台面板

    public void ToggleBag()
    {
        bool show = !bagPanel.activeSelf;
        bagPanel.SetActive(show);
        if (!show) craftPanel.SetActive(false);  // 关背包时也关合成台

        Time.timeScale = show ? 0f : 1f;
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;

        if (!show)
            SlotDragHandler.AutoStash(FindObjectOfType<WeaponStorage>());
    }

    void ToggleCraft()
    {
        bool show = !craftPanel.activeSelf;
        craftPanel.SetActive(show);
        bagPanel.SetActive(show);  // 背包同步开关

        Time.timeScale = show ? 0f : 1f;
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;

        if (!show)
            SlotDragHandler.AutoStash(FindObjectOfType<WeaponStorage>());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            ToggleBag();
        if (Input.GetKeyDown(KeyCode.C))
            ToggleCraft();
    }
}
