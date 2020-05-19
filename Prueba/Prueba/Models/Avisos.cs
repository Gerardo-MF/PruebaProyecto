using System;
using System.Collections.Generic;
using System.Text;

namespace Prueba.Models
{
    public class Avisos
    {
        public Int32 IdAvisosEnviados { get; set; }
        public String Titulo { get; set; }
        public String Conenido { get; set; }
        public Int32 Estatus { get; set; }
        public String ClaveMaestro { get; set; }
        public String ClaveAlumno { get; set; }
    }
}
