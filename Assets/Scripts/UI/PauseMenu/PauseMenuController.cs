using UnityEngine;
using UnityEngine.Events;

public class PauseMenuController : MonoBehaviour
{
    public UnityEvent openPause;
    public UnityEvent closePause;
    private bool isPauseOpen = false;
    private bool isSettingsOpen = false;

    void Update()
    {   
        if (isSettingsOpen) {
            return;
        } else {
            if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPauseOpen) {
                    isPauseOpen = true;
                    Time.timeScale = 0f;
                    print("Open Pause Menu");
                    openPause.Invoke();
                } else {
                    isPauseOpen = false;
                    Time.timeScale = 1f;
                    closePause.Invoke();
                }
            }
        }
    }

    public void setIsPauseOpenFalse() {
        isPauseOpen = false;
        Time.timeScale = 1f;
    }

    public void setIsSettingsOpenFalse () {
        isSettingsOpen = false;
    }

    public void setIsSettingsOpenTrue () {
        isSettingsOpen = true;
    }
}
