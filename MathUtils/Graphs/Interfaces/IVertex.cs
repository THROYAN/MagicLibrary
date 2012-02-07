using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    public interface IVertex
    {
        /// <summary>
        /// Граф, к которому относится вершина
        /// </summary>
        IGraph Graph { get; set; }

        /// <summary>
        /// "Содержимое" вершины
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Степень вершины - количество рёбер входящих и выходящих из вершины
        /// </summary>
        int Degree { get; }

        void CopyTo(out IVertex vertex);

        /// <summary>
        /// Копирует свойства в существующий объект класса из текущего
        /// </summary>
        /// <param name="vertex"></param>
        void CopyTo(IVertex vertex);

        /// <summary>
        /// Создаёт неполную копию вершины.
        /// </summary>
        /// <returns></returns>
        object Clone();
    }
}
