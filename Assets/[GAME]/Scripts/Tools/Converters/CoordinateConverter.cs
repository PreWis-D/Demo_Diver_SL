using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public static class CoordinateConverter
{
    public static Vector2 WorldToViewportPointConvert(Transform target, Vector2 canvasSize)
    {
        Camera camera = SL.Get<CamerasService>().GetMainCamera();

        Vector2 viewPortPos3d = camera.WorldToViewportPoint(target.position);
        Vector2 viewPortRelative = new Vector2(viewPortPos3d.x - 0.5f, viewPortPos3d.y - 0.5f);
        Vector2 indicatorsScreenPosition = new Vector2(viewPortRelative.x * canvasSize.x, viewPortRelative.y * canvasSize.y);

        return indicatorsScreenPosition + (canvasSize / 2);
    }
}