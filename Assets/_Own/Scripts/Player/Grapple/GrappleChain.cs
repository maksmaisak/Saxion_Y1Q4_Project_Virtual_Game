using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

#pragma warning disable 0649

[RequireComponent(typeof(Grapple))]
public class GrappleChain : MonoBehaviour
{
    [SerializeField] Transform playerEndpoint;
    [SerializeField] Transform chainLinkPrefab;
    [SerializeField] Vector3 chainLinkRelativeRotationEulerAngles;

    private Grapple grapple;

    private Transform linksTransform;
    private Stack<Transform> links = new Stack<Transform>();

    private ObjectPool<Transform> linksPool; 

    private float oneLinkLength = 0.2f; // full is 0.27f
    private float currentLength { get { return links.Count * oneLinkLength; } }

    private Quaternion chainLinkRelativeRotation;

    void Start()
    {
        Assert.IsNotNull(playerEndpoint);
        Assert.IsNotNull(chainLinkPrefab);

        grapple = GetComponent<Grapple>();

        linksTransform = new GameObject("ChainLinks").transform;
        linksTransform.SetParent(transform, worldPositionStays: false);

        linksPool = new ObjectPool<Transform>(
            () => Instantiate(chainLinkPrefab, linksTransform),
            link => link.gameObject.SetActive(false)
        );
    }

    void Update()
    {
        chainLinkRelativeRotation = Quaternion.Euler(chainLinkRelativeRotationEulerAngles);

        Vector3 delta = playerEndpoint.position - transform.position;

        float desiredLength = grapple.isConnected ? grapple.ropeLength : delta.magnitude;

        if (desiredLength > 0.1f)
        {
            desiredLength += oneLinkLength * 2f;
        }

        while (desiredLength > currentLength)
        {
            AddLink();
        }

        while (currentLength > desiredLength)
        {
            RemoveLink();
        }

        if (desiredLength >= oneLinkLength * 0.5f)
        {
            linksTransform.rotation = Quaternion.LookRotation(delta.normalized);
        }
    }

    private void AddLink()
    {
        Transform link = linksPool.GetObject();
        link.localPosition = Vector3.forward * oneLinkLength * links.Count;
        link.localRotation = chainLinkRelativeRotation;
        link.gameObject.SetActive(true);
        links.Push(link);
    }

    private void RemoveLink()
    {
        Transform link = links.Pop();
        linksPool.ReleaseObject(link);
    }
}
