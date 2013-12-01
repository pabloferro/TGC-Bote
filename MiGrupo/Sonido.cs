using TgcViewer.Utils.Sound;
using TgcViewer;


namespace AlumnoEjemplos.MiGrupo
{
    public static class Sonido
    {
        public static TgcStaticSound ambientSoundLluvia;
        public static bool lluvia = false;
        public static void Init()
        {
            ambientSoundLluvia = new TgcStaticSound();
            ambientSoundLluvia.loadSound(EjemploAlumno.media() + "Sonidos\\Tormenta.wav");
            GuiController.Instance.Mp3Player.FileName = EjemploAlumno.media() + "Sonidos\\Oceano.mp3";
            GuiController.Instance.Mp3Player.play(true);
        }

        public static void Lluvia(bool on)
        {
            lluvia = on;
            if (lluvia)
                ambientSoundLluvia.play(true);
            else
                ambientSoundLluvia.stop();
        }

        public static void Dispose()
        {
            ambientSoundLluvia.stop();
            ambientSoundLluvia.dispose();
            GuiController.Instance.Mp3Player.closeFile();
        }
    }

}