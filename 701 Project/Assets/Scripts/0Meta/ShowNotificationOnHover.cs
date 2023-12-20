using UnityEngine;

public class ShowNotificationOnHover : MonoBehaviour
{
    [SerializeField] GameObject notificationGO;

    public void OnEnter() { notificationGO.SetActive(true); }

    public void OnExit() { notificationGO.SetActive(false); }
}