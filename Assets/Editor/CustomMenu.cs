using UnityEngine;
using UnityEditor;
using System.IO;
using UnityMeshSimplifier;
using UnityEditor.SceneManagement;

public class CustomMenu
{
    private const string IsGeneratedFieldName = "isGenerated";
    // 定义菜单项的路径，例如：Tools/My Button
    [MenuItem("Tools/My Button")]

    public static void MyButtonFunction()
    {
        var path = Application.dataPath+ "/BakedMeshes/";
        var dir = new DirectoryInfo(path);
        var x = dir.GetFiles("*.asset");
        for(int i = 0; i < x.Length; i++)
        {
            // 将文件的完整路径转换为相对于 Assets 的路径
            string assetPath = "Assets" + x[i].FullName.Replace("\\", "/").Replace(Application.dataPath, "");

            var m = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
            var nname = x[i].Name.Replace(x[i].Extension,"");
            GameObject fx = new GameObject(nname);
            fx.AddComponent<MeshFilter>().sharedMesh = m;
            fx.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard")); // 使用标准材质
            var lod = fx.AddComponent<LODGeneratorHelper>(); 
            lod.Levels[0].Quality = 0.5f;
            lod.Levels[1].Quality = 0.3f;
            lod.Levels[2].Quality = 0.02f;
            GenerateLODs(lod);
            GameObject.DestroyImmediate(fx);
        }
        //AssetDatabase.LoadAssetAtPath<>
    }
    private static void GenerateLODs(LODGeneratorHelper lodGeneratorHelper)
    {
        try
        {
            EditorUtility.DisplayProgressBar("Generating LODs", "Generating LODs...", 0f);
            var lodGroup = LODGenerator.GenerateLODs(lodGeneratorHelper);
            if (lodGroup != null)
            {
                using (var serializedObject = new SerializedObject(lodGeneratorHelper))
                {
                    var isGeneratedProperty = serializedObject.FindProperty(IsGeneratedFieldName);
                    serializedObject.UpdateIfRequiredOrScript();
                    isGeneratedProperty.boolValue = true;
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex, lodGeneratorHelper);
            DisplayError("Failed to generate LODs!", ex.Message, "OK", lodGeneratorHelper);
        }
        finally
        {
            MarkSceneAsDirty(lodGeneratorHelper);
            EditorUtility.ClearProgressBar();
        }
    }
    private static void MarkSceneAsDirty(LODGeneratorHelper lodGeneratorHelper)
    {
        EditorSceneManager.MarkSceneDirty(lodGeneratorHelper.gameObject.scene);
    }

    private static void DisplayError(string title, string message, string ok, Object context)
    {
        EditorUtility.DisplayDialog(title, message, ok);
    }
}
