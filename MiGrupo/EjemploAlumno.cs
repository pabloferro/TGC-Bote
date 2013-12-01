using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using AlumnoEjemplos.MiGrupo;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo en Blanco. Ideal para copiar y pegar cuando queres empezar a hacer tu propio ejemplo.
    /// </summary>
    public class EjemploAlumno : TgcExample
    {

        Agua agua;
        Bote bote;
        Skybox skybox = new Skybox();
        Lluvia lluvia = new Lluvia();
        bool lluvia_on = false;

        //Para EnvironmentMap
        CubeTexture cubeMap;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "MiGrupo";
        }

        public override string getDescription()
        {
            return "Bote en el oceano - Flechas para mover";
        }

        public override void init()
        {
            cubeMap = TextureLoader.FromCubeFile(GuiController.Instance.D3dDevice, EjemploAlumno.media() + "Texturas\\Skybox\\Skybox.dds");
            GuiController.Instance.Modifiers.addBoolean("Lluvia", "Activar Lluvia", false);
            GuiController.Instance.Modifiers.addFloat("Reflexion", 0, 1, 0.2f);
            agua = new Agua(2000, 25, cubeMap);
            bote = new Bote(agua);
            bote.init();
            lluvia.init();
            skybox.init();
            Sonido.Init();

        }


        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            agua.render(elapsedTime);
            bote.render(elapsedTime);

            if ((bool)GuiController.Instance.Modifiers.getValue("Lluvia") != lluvia_on)
            {
                lluvia_on = (bool)GuiController.Instance.Modifiers.getValue("Lluvia");
                Sonido.Lluvia(lluvia_on);
            }

            skybox.render(elapsedTime, agua.getCenter());

            if (lluvia_on)
               lluvia.render();

            agua.updatePosition(new Vector2(bote.getPos().X, bote.getPos().Z));
        }

        public override void close()
        {
            agua.close();
            bote.dispose();
            Sonido.Dispose();
            cubeMap.Dispose();
        }

        public static string media()
        {
            return GuiController.Instance.AlumnoEjemplosMediaDir + "MiGrupo\\";
        }
    }
}
