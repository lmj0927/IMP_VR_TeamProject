using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System;
using System.Collections;

public class ItemBox : MonoBehaviour
{
    [Header("Event")]
    public Action OnBoxDepleted;

    [Header("UI")]
    public GameObject selectionPanel;

    [Header("Items")]
    public GameObject[] itemPrefabs;

    private bool isOpen = false;
    private Transform playerCamera;
    private Vector3 originalPanelScale;

    void Start()
    {
        playerCamera = Camera.main.transform;
        
        foreach (var btn in GetComponentsInChildren<ItemButton>())
            btn.targetBox = this;

        if (selectionPanel != null)
        {
            originalPanelScale = selectionPanel.transform.localScale; 
            selectionPanel.SetActive(false);
        }

        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
            interactable.selectEntered.AddListener(OnBoxSelected);
    }

    void Update()
    {
        if (isOpen && selectionPanel != null)
        {
            selectionPanel.transform.LookAt(playerCamera);
            selectionPanel.transform.Rotate(0, 180f, 0);
        }
    }

    void OnBoxSelected(SelectEnterEventArgs args)
    {
        if (isOpen)
        {
            isOpen = false;
            StartCoroutine(CloseAnimation(selectionPanel.transform));
        }
        else
        {
            isOpen = true;
            selectionPanel.SetActive(true);
            StartCoroutine(PopupAnimation(selectionPanel.transform));
        }
    }

    IEnumerator PopupAnimation(Transform target)
    {
        target.localScale = Vector3.zero;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.localScale = Vector3.Lerp(Vector3.zero, originalPanelScale, t);
            yield return null;
        }
        target.localScale = originalPanelScale;
    }

    IEnumerator CloseAnimation(Transform target)
    {
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.localScale = Vector3.Lerp(originalPanelScale, Vector3.zero, t);
            yield return null;
        }

        target.localScale = Vector3.zero;
        selectionPanel.SetActive(false);
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= itemPrefabs.Length) return;

        Vector3 spawnPos = transform.position + Vector3.up * 0.3f;
        Instantiate(itemPrefabs[index], spawnPos, Quaternion.identity);

        StartCoroutine(CloseAndDestroy());
    }

    IEnumerator CloseAndDestroy()
    {
        selectionPanel.SetActive(false);
        isOpen = false;
        yield return new WaitForSeconds(0.3f);
        OnBoxDepleted?.Invoke();
        Destroy(gameObject);
    }
}