using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;

namespace AlumnoEjemplos.MiGrupo
{
   class Lluvia
    {
        public List<ParticleEmitter> Emisores = new List<ParticleEmitter>();
        public void init()
        {
            for (int i = 0; i < 70; i++)
            {
                ParticleEmitter Emisor = new ParticleEmitter(EjemploAlumno.media() + "Texturas\\lluvia.png", Utiles.iAleatorio(50, 100));
                float Viento = Utiles.fAleatorio(4, 6);
                Emisor.Speed = new Vector3(Viento, -Utiles.fAleatorio(150, 150), Viento);
                Emisor.Dispersion = Utiles.iAleatorio(50, 150);
                Emisor.MinSizeParticle = Utiles.fAleatorio(7, 10);
                Emisor.MaxSizeParticle = Utiles.fAleatorio(10, 15);
                Emisor.CreationFrecuency = Utiles.fAleatorio(0.01f, 0.5f);
                Emisor.ParticleTimeToLive = Utiles.fAleatorio(5, 7);
                Emisor.Distancia = Utiles.fAleatorio(1, 1);
                Emisores.Add(Emisor);
            }
        }

        public void render()
        {

            Vector3 Posicion = GuiController.Instance.CurrentCamera.getPosition();
            Vector3 LookAt = GuiController.Instance.CurrentCamera.getLookAt();
            Posicion.Y += Utiles.fAleatorio(80, 85);
            foreach (var Emisor in Emisores)
            {
                Posicion.X = LookAt.X + Utiles.fAleatorio(-500, 500);
                Posicion.Z = LookAt.Z + Utiles.fAleatorio(-600, 500);
                Emisor.Position = Posicion;
                Emisor.render();
            }
        }
    }
}
