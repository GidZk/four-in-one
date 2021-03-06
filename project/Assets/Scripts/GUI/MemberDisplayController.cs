﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MemberDisplayController : MonoBehaviour
{
    public Image[] members;
    public Sprite ActiveSprite;
    public Sprite InactiveSprite;

    private void Awake()
    {
        foreach (var im in members)
        {
            im.preserveAspect = true;
        }

        SetNumberJoined(1);
    }

    public Color Color = UnityEngine.Color.clear;

    public void SetNumberJoined(int n)
    {
        if (n < 0 || n > members.Length)
        {
            Debug.Log("Tried do set to more members than possible, very strange ");
            return;
        }

        foreach (var image in members)
        {
            image.color = Color.white;
            image.sprite = InactiveSprite;
        }

        foreach (var image in members.Take(n))
        {
            image.color = Color;
            image.sprite = ActiveSprite;
        }
    }
}