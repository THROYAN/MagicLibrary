﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    public interface IEdge : ICloneable
    {
        /// <summary>
        /// Граф, к которому относится ребро
        /// </summary>
        IGraph Graph { get; set; }

        /// <summary>
        /// Конечные вершины ребра {u,v} (Размер = 2)
        /// </summary>
        IVertex[] Vertices { get; set; }

        /// <summary>
        /// Копирует свойства в существующий объект класса из текущего
        /// </summary>
        /// <param name="edge"></param>
        void CopyTo(IEdge edge);
    }
}
