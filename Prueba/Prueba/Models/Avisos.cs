using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prueba.Models
{
    public class Avisos
    {
        [PrimaryKey]
        public Int32 IdAvisosEnviados { get; set; }
        public String Contenido { get; set; }
        public Int32 Estatus { get; set; }
        public DateTime fechaEnviado { get; set; }
        public Int32 IdMaestro { get; set; }
        public Int32 IdAlumno { get; set; }
        public String ClaveAlumno { get; set; }
    }
}
