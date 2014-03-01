using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MineSolver
{
    public class Field
    {
        //List of neighboring Field-Ids
        public int[] Neighbours; // always 8?

        //Field attributes
        public int Value { get; set; }
        public FieldType FieldType { get; set; }

        //Static variables
        public int Id { get; set; }
        //public bool IsCornor { get; set; }
        //public bool IsEdge { get; set; }
        public bool IsActive { get; set; }

        public Field()
        {
            FieldType = FieldType.Unknown;
        }
    }

    public enum FieldType
    {
        Mine,
        Unknown,
        Numbered
    }
}
