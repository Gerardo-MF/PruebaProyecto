using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prueba.Models
{
    public class Maestro
    {

        [PrimaryKey]
        public Int32 IdMaestro { get; set; }
        public Int32 IdEscuela { get; set; }
        public String NombreEscuela { get; set; }
        public Int32 IdGrupoMaestroAlumno { get; set; }



    }
}
