using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.MiGrupo
{
    class Bote
    {
        TgcMesh mesh_bote;
        private static float velocidad_rotacion;
        private static float velocidad_actual;
        private static float velocidad_desplazamiento;
        private float velocidad_desplazamiento_maxima = 600f;
        Oceano agua;
        float largo, ancho, alto;

        public Bote(Oceano agua)
        {
            this.agua = agua;
        }

        public void init()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            modifiers();

            //Cargar mesh 
            TgcSceneLoader loader = new TgcSceneLoader();
            mesh_bote = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\Canoa\\Canoa-TgcScene.xml").Meshes[0];
            mesh_bote.Position = new Vector3(0, 0, 0);
            mesh_bote.AutoTransformEnable = false;
            velocidad_rotacion = 200f;
            velocidad_actual = 0f;

            // Calcular dimensiones
            Vector3 BoundingBoxSize = mesh_bote.BoundingBox.calculateSize();

            largo = Math.Abs(BoundingBoxSize.Z);
            ancho = Math.Abs(BoundingBoxSize.X);
            alto = Math.Abs(BoundingBoxSize.Y);
            //Camara
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(mesh_bote.Position, 250, -800);
        }

        public void render(float elapsedTime)
        {
            velocidad_rotacion = (float)GuiController.Instance.Modifiers.getValue("Velocidad Rotación");
            velocidad_desplazamiento_maxima = (float)GuiController.Instance.Modifiers.getValue("Velocidad Máxima");
            this.procesarInputs(elapsedTime);
            Vector3 pos_ant = mesh_bote.Position;
            mesh_bote.Position = new Vector3(0, pos_ant.Y, 0);
            mesh_bote.render();
            mesh_bote.Position = pos_ant;
        }

        //Devuelve la diferencia de altura
        private float cambiar_altura(float rotAngle)
        {
            mesh_bote.Position = new Vector3(mesh_bote.Position.X, 13+agua.altura(new Vector2(mesh_bote.Position.X, mesh_bote.Position.Z)), mesh_bote.Position.Z);
            Vector3 barcoFrente;

            barcoFrente = mesh_bote.Position + new Vector3((float)Math.Sin(mesh_bote.Rotation.Y), 0, (float)Math.Cos(mesh_bote.Rotation.Y)) * (largo / 2);
            barcoFrente.Y = 10+agua.altura(new Vector2(barcoFrente.X, barcoFrente.Z));
            Vector3 vectorBarco = barcoFrente - mesh_bote.Position;
            vectorBarco.Normalize();
            
            mesh_bote.Transform = CalcularMatriz(mesh_bote.Position, mesh_bote.Scale, vectorBarco);
            mesh_bote.BoundingBox.transform(mesh_bote.Transform);
            return barcoFrente.Y - mesh_bote.Position.Y;
        }

        public void procesarInputs(float elapsedTime)
        {

            float rotacion = 0;
            bool rotar = false;

            velocidad_desplazamiento = (float)GuiController.Instance.Modifiers.getValue("Aceleración");
          
            //Derecha
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.RightArrow))
            {
                rotacion = velocidad_rotacion;
                rotar = true;
            }
            //Izquierda
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftArrow))
            {
                rotacion = -velocidad_rotacion;
                rotar = true;
            }

            //Si hubo rotacion
            float rotAngle = 0;
            if (rotar)
            {
                rotAngle = Geometry.DegreeToRadian(rotacion * elapsedTime);
                mesh_bote.rotateY(rotAngle);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            }

            float diferencia = cambiar_altura(rotAngle);
            //Adelante
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.UpArrow))
            {
                if (velocidad_desplazamiento == 0)
                    velocidad_actual = 0;
                else
                velocidad_actual += (velocidad_desplazamiento * elapsedTime);
            }
            //Atras
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.DownArrow))
            {
                velocidad_actual -= velocidad_desplazamiento * 1.25f * elapsedTime;
            }
            velocidad_actual -= 50 * elapsedTime;
            velocidad_actual = Math.Min(Math.Max(0, velocidad_actual), velocidad_desplazamiento_maxima);

            if (diferencia > 10)
                diferencia = 0.80f;
            else if (diferencia < -10)
                diferencia = 1.2f;
            else
                diferencia = 1f;

            //Vector3 lastPos = mesh_bote.Position;
            mesh_bote.moveOrientedY(diferencia * velocidad_actual * elapsedTime);
            GuiController.Instance.ThirdPersonCamera.Target = new Vector3(0, mesh_bote.Position.Y, 0);// mesh_bote.Position;
        }

        public static Matrix CalcularMatriz(Vector3 Pos, Vector3 Scale, Vector3 Dir)
        {
            Vector3 VUP = new Vector3(0, 1, 0);

            Matrix matWorld = Matrix.Scaling(Scale);
            // determino la orientacion
            Vector3 U = Vector3.Cross(VUP, Dir);
            U.Normalize();
            Vector3 V = Vector3.Cross(Dir, U);
            Matrix Orientacion;
            Orientacion.M11 = U.X;
            Orientacion.M12 = U.Y;
            Orientacion.M13 = U.Z;
            Orientacion.M14 = 0;

            Orientacion.M21 = V.X;
            Orientacion.M22 = V.Y;
            Orientacion.M23 = V.Z;
            Orientacion.M24 = 0;

            Orientacion.M31 = Dir.X;
            Orientacion.M32 = Dir.Y;
            Orientacion.M33 = Dir.Z;
            Orientacion.M34 = 0;

            Orientacion.M41 = 0;
            Orientacion.M42 = 0;
            Orientacion.M43 = 0;
            Orientacion.M44 = 1;
            matWorld = matWorld * Orientacion;

            // traslado solo la altura
            matWorld = matWorld * Matrix.Translation(0, Pos.Y, 0);
            return matWorld;
        }

        public void dispose()
        {
            mesh_bote.dispose();
        }

        public Vector3 getPos()
        {
            return mesh_bote.Position;
        }

        public void modifiers()
        {
            GuiController.Instance.Modifiers.addFloat("Velocidad Rotación", 10, 200, 75);
            GuiController.Instance.Modifiers.addFloat("Aceleración", 1, 500, 90);
            GuiController.Instance.Modifiers.addFloat("Velocidad Máxima", 1, 1500, 500);
        }
    }
}