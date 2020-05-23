using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prueba.Models
{
   public class AvisosGenerales
    {
        public int IdAvisosGenerales { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaCaducidad { get; set; }
        public DateTime FechaEnviado { get; set; }
        public string Contenido { get; set; }
    }
}
