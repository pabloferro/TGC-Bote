using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MiGrupo
{
    class Agua
    {
        Vector2 center;
        VertexBuffer vertexBuffer;
        List<CustomVertex.PositionColored> vertexList;
        int size;
        float radio;
        float cuadSize;
        float timer = 0;
        Effect effect;
        CubeTexture cubeMap;

        Device d3dDevice;
        Vector2 pos = new Vector2(0, 0);

        Color color = Color.FromArgb(0, 57, 130);

        struct Wave
        {
            public float freq;  // 2*PI / wavelength
            public float amp;   // amplitude
            public float phase; // speed * 2*PI / wavelength
            public Vector2 dir;
        }

        public const int num_waves = 2;
        public const float freqMult = 100000;
        Wave[] waves = new Wave[num_waves];

        public Agua(int size, int cuadSize, CubeTexture cubeMap)
        {
            this.cubeMap = cubeMap;
            //{ 0.042, 20, 0.5, float2(1, 0) }
            waves[0].freq = 0.0125f;
            waves[0].amp = 75;
            waves[0].phase = 0.5f;
            waves[0].dir = new Vector2(0, 1);
            //{ 0.005, 30, 0.35, float2(0.9, 0.3) },
            waves[1].freq = 0.005f;
            waves[1].amp = 35;
            waves[1].phase = 0.25f;
            waves[1].dir = new Vector2(0.9f, 0.3f);
            //{ 0.25, 0.25, 4.5, float2(0, 1) }
	        //{ 0.29, 0.25, 4.75, float2(1, 0) }


            //Modifiers
            //Modifiers de la luz
            GuiController.Instance.Modifiers.addVertex2f("1) Dirección", new Vector2(-1, -1), new Vector2(1, 1), waves[0].dir);
            GuiController.Instance.Modifiers.addFloat("1) Amplitud", 0, 200, waves[0].amp);
            GuiController.Instance.Modifiers.addFloat("1) Frecuencia", 0, 1250, waves[0].freq * freqMult);
            GuiController.Instance.Modifiers.addFloat("1) Fase", 0, 3, waves[0].phase);
            GuiController.Instance.Modifiers.addVertex2f("2) Dirección", new Vector2(-1, -1), new Vector2(1, 1), waves[1].dir);
            GuiController.Instance.Modifiers.addFloat("2) Amplitud", 0, 200, waves[1].amp);
            GuiController.Instance.Modifiers.addFloat("2) Frecuencia", 0, 1250, waves[1].freq * freqMult);
            GuiController.Instance.Modifiers.addFloat("2) Fase", 0, 3, waves[1].phase);
            
            this.size = size;
            this.radio = size / 2;
            this.cuadSize = cuadSize;
            d3dDevice = GuiController.Instance.D3dDevice;
            this.effect = TgcShaders.loadEffect(EjemploAlumno.media() + "Shaders\\agua.fx");
            center = new Vector2(0, 0);
            createVertexBuffer(size, cuadSize);
        }

        private void createVertexBuffer(int size, int cuadSize)
        {
            vertexList = new List<CustomVertex.PositionColored>();

            for (int x = -1; x <= 1; x++)
                for (int z = -1; z <= 1; z++)
                    if(x==0&&z==0)
                        crearAgua(size, cuadSize, vertexList, new Vector2(x * size - radio, z * size - radio));
                    else
                        crearAgua(size, cuadSize * 2, vertexList, new Vector2(x * size -radio, z * size - radio));

            //Almacenar información en VertexBuffer
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored), vertexList.Count, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Default);
            vertexBuffer.SetData(vertexList.ToArray(), 0, LockFlags.None);
        }

        private void crearAgua(int size, int cuadSize, List<CustomVertex.PositionColored> vertexList, Vector2 top_left)
        {
            for (float x = top_left.X ; x <= top_left.X + size; x += cuadSize)
            {
                for (float z = top_left.Y; z <= top_left.Y + size; z += cuadSize)
                {
                    vertexList.AddRange(this.verticesCuadrado(new Vector2(x, z), cuadSize, 0));
                }
            }
        }

        public void updatePosition(Vector2 pos)
        {
            if (pos.X - (center.X + radio) > -150)
            {
                moverAgua(1, 0);
                center.X += radio;
            }
            if (pos.X - (center.X - radio) < 150)
            {
                moverAgua(-1, 0);
                center.X -= radio;
            }
            if (pos.Y - (center.Y + radio) > -150)
            {
                moverAgua(0, 1);
                center.Y += radio;
            }
            if (pos.Y - (center.Y - radio) < 150)
            {
                moverAgua(0, -1);
                center.Y -= radio;
            }

            this.pos = pos;
        }

        public void moverAgua(int x, int z)
        {
            List<CustomVertex.PositionColored> tmpVertexList = new List<CustomVertex.PositionColored>();
            foreach (CustomVertex.PositionColored v in vertexList)
            {
                tmpVertexList.Add(new CustomVertex.PositionColored(v.X + radio * x, 0, v.Z + radio * z, color.ToArgb()));
            }
            vertexBuffer.Dispose();
            vertexList = tmpVertexList;
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored), vertexList.Count, d3dDevice, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Default);
            vertexBuffer.SetData(vertexList.ToArray(), 0, LockFlags.None);
        }

        public void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            timer += elapsedTime;

            //Actualizar Technique
            this.effect.Technique = "Oceano";

            //Cargar variables de shader
            this.effect.SetValue("timer", timer);
            Vector3 eyePosition = GuiController.Instance.CurrentCamera.getPosition();
            this.effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(eyePosition));
            this.effect.SetValue("texCubeMap", cubeMap);
            this.effect.SetValue("reflection", (float)GuiController.Instance.Modifiers.getValue("Reflexion"));
            //Olas+
            int i = 1;
            foreach (Wave w in waves)
            {
                loadWavesFromModifier();
                this.effect.SetValue("freq" + i.ToString(), w.freq);
                this.effect.SetValue("amp" + i.ToString(), w.amp);
                this.effect.SetValue("phase" + i.ToString(), w.phase);
                this.effect.SetValue("dir" + i.ToString(), TgcParserUtils.vector2ToFloat2Array(w.dir));
                i++;
            }

            d3dDevice.VertexFormat = CustomVertex.PositionColored.Format;
            d3dDevice.SetStreamSource(0, vertexBuffer, 0);

            GuiController.Instance.Shaders.setShaderMatrix(this.effect, Matrix.Identity);
            int numPasses = effect.Begin(0);

            //Iniciar Shader e iterar sobre sus Render Passes
            for (int n = 0; n < numPasses; n++)
            {
                //Iniciar pasada de shader
                effect.BeginPass(n);
                d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.triangle_count());
                effect.EndPass();
            }
            //Finalizar shader
            effect.End();
            //d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.triangle_count());
        }

        public void close()
        {
            //liberar VertexBuffer
            vertexBuffer.Dispose();
        }

        private CustomVertex.PositionColored[] verticesCuadrado(Vector2 pos, float lado, float altura)
        {
            CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[6];
            vertices[0] = new CustomVertex.PositionColored(pos.X, altura, pos.Y, color.ToArgb());
            vertices[1] = new CustomVertex.PositionColored(pos.X, altura, pos.Y + lado, color.ToArgb());
            vertices[2] = new CustomVertex.PositionColored(pos.X + lado, altura, pos.Y + lado, color.ToArgb());

            vertices[3] = new CustomVertex.PositionColored(pos.X, altura, pos.Y, color.ToArgb());
            vertices[4] = new CustomVertex.PositionColored(pos.X + lado, altura, pos.Y + lado, color.ToArgb());
            vertices[5] = new CustomVertex.PositionColored(pos.X + lado, altura, pos.Y, color.ToArgb());
            return vertices;
        }

        public int triangle_count()
        {
            return (int)(Math.Pow(size / cuadSize, 2) * 2) * 9;
        }

        public float altura(Vector2 pos)
        {
            float altura = 0;
            foreach(Wave w in waves)
                altura+= evaluateWave(w, pos);
            return altura;
        }

        float evaluateWave(Wave w, Vector2 pos)
        {
            return w.amp * (float)Math.Sin(Vector2.Dot(w.dir, pos) * w.freq + timer * w.phase);
        }

        // derivative of wave function
        float evaluateWaveDeriv(Wave w, Vector2 pos)
        {
            return w.freq * w.amp * (float)Math.Cos(Vector2.Dot(w.dir, pos) * w.freq + timer * w.phase);
        }

        public Vector3 getCenter()
        {
            return new Vector3(center.X, 0, center.Y);
        }

        private void loadWavesFromModifier()
        {
            for (int x = 0; x < num_waves; x++)
            {
                waves[x].dir = (Vector2)GuiController.Instance.Modifiers[(x+1).ToString() + ") Dirección"];
                waves[x].dir.Normalize();
                waves[x].amp = (float)GuiController.Instance.Modifiers[(x + 1).ToString() + ") Amplitud"];
                waves[x].freq = ((float)GuiController.Instance.Modifiers[(x + 1).ToString() + ") Frecuencia"]) / freqMult;
                waves[x].phase = (float)GuiController.Instance.Modifiers[(x + 1).ToString() + ") Fase"];
            }
        }
    }
}
