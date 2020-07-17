using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(UIController))]
public class UIControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Test donation"))
            {
                var ui = target as UIController;
                ui.OnDonationMade(new CoinSpawnController.DonationEventData()
                {
                    Name = "Test Name",
                    Amount = 5.43f,
                    Message = "Test Message",
                    ProfileURL = "https://pbs.twimg.com/profile_images/1264943635365277697/eSfno0BN_400x400.jpg"
                });
            }
        }
    }
}
