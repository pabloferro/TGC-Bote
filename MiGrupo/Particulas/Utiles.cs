using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Shaders;


namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Clase que contiene metodos útiles para varios propósitos.
    /// </summary>
    public static class Utiles
    {
        #region ::ALEATORIO::
        private static Random Generador = new Random();
        /// <summary>
        /// Método para obtener un número int aleatorio con cotas.
        /// </summary>
        /// <param name="Min">Cota inferior.</param>
        /// <param name="Max">Cota superior.</param>
        /// <returns>int entre Min y Max</returns>
        public static int iAleatorio(int Min, int Max)
        {
            return Generador.Next(Min, Max);
        }

        /// <summary>
        /// Método para obtener un número float aleatorio.
        /// </summary>
        /// <returns>int aleatorio entre 0 y int.MaxValue</returns>
        public static int iAleatorio()
        {
            return Generador.Next();
        }

        /// <summary>
        /// Método para obtener un número float aleatorio con cotas.
        /// </summary>
        /// <param name="Min">Cota inferior.</param>
        /// <param name="Max">Cota superior.</param>
        /// <returns>float entre Min y Max</returns>
        public static float fAleatorio(float Min, float Max)
        {
            return Min + (Max - Min) * fAleatorio();
        }
        /// <summary>
        /// Método para obtener un número float aleatorio.
        /// </summary>
        /// <returns>float aleatorio entre 0.0 y 1.0</returns>
        public static float fAleatorio()
        {
            return (float)Generador.NextDouble();
        }
        #endregion

    }
}
