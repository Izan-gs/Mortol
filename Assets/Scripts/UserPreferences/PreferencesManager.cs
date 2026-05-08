using System.Diagnostics;
using System.IO;
using UnityEngine;

// Gestor de preferencias en Unity. Actúa como singleton y sincroniza el JSON local con Java/MySQL
public class PreferencesManager : MonoBehaviour
{
    public static PreferencesManager Instance { get; private set; }

    // Ruta del archivo JSON local
    private string path;

    // Ruta del ejecutable de Java y de los JAR necesarios
    private string javaPath;
    private string mysqlPath;
    private string jarPath;

    private bool databaseAvailable = true;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Rutas
        string basePath = Application.streamingAssetsPath + "/Java/";

        // Se usa java.exe para que Windows lo resuelva correctamente
        javaPath = "java.exe";
        jarPath = basePath + "preferences.jar";
        mysqlPath = basePath + "mysql-connector-j-9.7.0.jar";

        // Archivo local persistente de Unity
        path = Application.persistentDataPath + "/preferences.json";

        // Al arrancar, se cargan los datos de la base de datos
        ExportDatabase();
    }

    // Carga los datos desde el JSON local
    public PreferencesData Load()
    {
        if (!File.Exists(path))
            return new PreferencesData();

        return JsonUtility.FromJson<PreferencesData>(
            File.ReadAllText(path)
        );
    }

    // Guarda los datos en el JSON local
    public void Save(PreferencesData data)
    {
        File.WriteAllText(
            path,
            JsonUtility.ToJson(data, true)
        );
    }

    // Descarga los datos desde MySQL al JSON
    public void ExportDatabase()
    {
        RunJava("export");
    }

    // Sube los datos desde el JSON a MySQL
    public void ImportDatabase()
    {
        RunJava("import");
    }

    // Ejecuta el proceso Java con el modo indicado
    private void RunJava(string mode)
    {
        if (!databaseAvailable)
            return;

        try
        {
            if (!File.Exists(jarPath))
            {
                databaseAvailable = false;
                UnityEngine.Debug.LogWarning($"preferences.jar not found: {jarPath}");
                return;
            }

            if (!File.Exists(mysqlPath))
            {
                databaseAvailable = false;
                UnityEngine.Debug.LogWarning($"MySQL connector not found: {mysqlPath}");
                return;
            }

            Process process = new Process();

            process.StartInfo.FileName = "java";

            process.StartInfo.Arguments =
                $"-cp \"{mysqlPath};{jarPath}\" PreferencesService {mode}";

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            bool started = process.Start();

            if (!started)
            {
                databaseAvailable = false;
                UnityEngine.Debug.LogWarning("Failed to start Java process.");
                return;
            }

            // MAX WAIT TIME
            int timeoutMs = 3000;

            bool exited = process.WaitForExit(timeoutMs);

            // If database hangs / XAMPP offline / MySQL timeout
            if (!exited)
            {
                databaseAvailable = false;

                try
                {
                    process.Kill();
                }
                catch { }

                UnityEngine.Debug.LogWarning(
                    "Database connection timeout. Running in offline mode."
                );

                return;
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Java returned error
            if (!string.IsNullOrWhiteSpace(error))
            {
                databaseAvailable = false;

                UnityEngine.Debug.LogWarning(
                    $"Database unavailable:\n{error}"
                );

                return;
            }

            if (!string.IsNullOrWhiteSpace(output))
            {
                UnityEngine.Debug.Log(output);
            }
        }
        catch (System.Exception ex)
        {
            databaseAvailable = false;

            UnityEngine.Debug.LogWarning(
                $"Database disabled.\n{ex.Message}"
            );
        }
    }
}