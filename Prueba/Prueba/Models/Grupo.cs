using System;
using System.Collections.Generic;
using System.Text;

namespace Prueba.Models
{
    public class Grupo
    {
        public Int32 IdGrupo { get; set; }
        public List<Alumno> Alumnos { get; set; }
    }
}
