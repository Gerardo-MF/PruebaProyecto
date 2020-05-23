using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prueba.Models
{
    public class Alumno
    {
        [PrimaryKey]
        public Int32 IdAlumno { get; set; }
        public String NombreEscuela { get; set; }
        public String Clave { get; set; }
        public String Nombre { get; set; }
        public Int32 IdMaestro { get; set; }

    }
}
