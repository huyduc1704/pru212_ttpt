using UnityEngine;
using UnityEditor;
using System.IO;

public class ExportSpritesToPNG
{
    [MenuItem("Tools/Export Selected Sprites To PNG")]
    static void ExportSelectedSprites()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Sprite sprite)
            {
                var tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
                var pixels = sprite.texture.GetPixels(
                    (int)sprite.rect.x,
                    (int)sprite.rect.y,
                    (int)sprite.rect.width,
                    (int)sprite.rect.height);
                tex.SetPixels(pixels);
                tex.Apply();

                var bytes = tex.EncodeToPNG();
                var folderPath = "Assets/ExportedSprites";
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var filePath = $"{folderPath}/{sprite.name}.png";
                File.WriteAllBytes(filePath, bytes);

                Debug.Log($"Exported {sprite.name} to {filePath}");
            }
        }

        AssetDatabase.Refresh();
    }
}