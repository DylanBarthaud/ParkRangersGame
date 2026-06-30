using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;
using BlackboardSystem;
using System.Collections.Generic;
using Discord;
using Unity.Netcode;
using System;

// ============== INSTRUCTION ==============
// Create empty game object and add "Spline Container" component
// Add knots and draw line using the "Spline Edit Mode" in the Scene Tools
// Create another empty game object and add this script
// Select "Spline" as well as "Player" in the inspector
// Add sound to the object
// https://www.youtube.com/watch?v=5lEzchlQYKk

public class RiverSoundSpline : MonoBehaviour
{
    [Tooltip("Spline Path to follow")]
    public SplineContainer Spline;
    [Tooltip("Character to track")]
    public GameObject goPlayer;
    public Transform Player;

    public float loadTime = 2f;
    private void Awake()
    {
        EventManager.instance.playerSpawningComplete += SetPlayer;
    }

    private void SetPlayer()
    {
        Blackboard blackboard = BlackboardController.instance.GetBlackboard();
        List<BlackboardKey> playerKeys = GameManager.instance.playerBlackboardKeys;
        foreach (BlackboardKey key in playerKeys)
        {
            if (blackboard.TryGetValue(key, out PlayerInfo playerInfo))
            {
                if (playerInfo.id == Unity.Netcode.NetworkManager.Singleton.LocalClientId)
                    Player = playerInfo.playerCamera.transform;
            }
        }
    }

    void Update()
    {
        if (Player == null) return;  

        // Convert Player position to spline local space
        Vector3 localPlayerPos = Spline.transform.InverseTransformPoint(Player.position);

        // Find nearest point on spline
        SplineUtility.GetNearestPoint(Spline.Spline, localPlayerPos, out float3 nearestPointLocal, out float normalizedT);

        // Convert back to world space
        Vector3 nearestWorldPos = Spline.transform.TransformPoint(nearestPointLocal);

        // Set object position and rotation
        transform.position = nearestWorldPos;
        Vector3 tangent = Spline.transform.TransformDirection(Spline.Spline.EvaluateTangent(normalizedT));
        if (tangent != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(tangent);
    }
}