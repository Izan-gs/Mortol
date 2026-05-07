// Datos serializables que Unity guarda en el JSON local
[System.Serializable]
public class PreferencesData
{
    // Música activada o desactivada
    public bool music = true;

    // Sonidos activados o desactivados
    public bool sfx = true;

    // Volumen general
    public int volume = 50;

    // Vidas guardadas del nivel 1
    public int level1Lives = 0;

    // Vidas guardadas del nivel 2
    public int level2Lives = 0;
}