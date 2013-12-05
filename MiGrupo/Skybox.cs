using System;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.Terrain;
using AlumnoEjemplos.MiGrupo;

namespace AlumnoEjemplos.MiGrupo
{

    public class SkyBox
    {
        public TgcSkyBox skybox;
        private float lastUpdate = 0;
        public void init()
        {
            skybox = new TgcSkyBox();
            skybox.Center = new Vector3(0, 0, 0);
            skybox.Size = new Vector3(10000, 10000, 10000);
            string texturesPath = EjemploAlumno.media() + "Texturas\\Skybox\\";
           /* skybox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "up.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "dn.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "rt.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lf.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "ft.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "bk.jpg");
            * */
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "up.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "dn.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "bk.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "ft.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "rt.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lf.jpg");

            skybox.updateValues();
            skybox.AlphaBlendEnable = true;
        }
        public void render(float elapsedTime, Vector3 center)
        {
            if(center != skybox.Center)
            {
                skybox.Center = center;
                skybox.updateValues();
            }
            skybox.render();
        }
    }
}

