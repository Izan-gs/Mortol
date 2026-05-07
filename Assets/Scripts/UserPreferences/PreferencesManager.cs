using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PreferencesManager : MonoBehaviour
{
    private string path;

    void Awake()
    {
        path = Application.persistentDataPath + "/preferences.json";

        ExportDatabase();
    }

    // ========================================= LOAD
    public PreferencesData Load()
    {
        if (!File.Exists(path))
            return new PreferencesData();

        return JsonUtility.FromJson<PreferencesData>(
            File.ReadAllText(path)
        );
    }

    // ========================================= SAVE
    public void Save(PreferencesData data)
    {
        File.WriteAllText(
            path,
            JsonUtility.ToJson(data, true)
        );
    }

    // ========================================= DB EXPORT
    public void ExportDatabase()
    {
        RunJava("export");
    }

    // ========================================= DB IMPORT
    public void ImportDatabase()
    {
        RunJava("import");
    }

    // ========================================= JAVA
    private void RunJava(string mode)
    {
        Process process = new Process();

        process.StartInfo.FileName = "java";

        process.StartInfo.Arguments =
            "-cp mysql-connector-j-9.7.0.jar;. PreferencesService " + mode;

        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;

        process.Start();
        process.WaitForExit();
    }
}