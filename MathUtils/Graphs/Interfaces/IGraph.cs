using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.Graphs
{
    public interface IGraph
    {
        /// <summary>
        /// Порядок (количество вершин) графа
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Размер (количество рёбер) графа
        /// </summary>
        int Size { get; }

        ///// <summary>
        ///// Список вершин
        ///// </summary>
        //List<IVertex> Vertices { get; set; }

        ///// <summary>
        ///// Список рёбер
        ///// </summary>
        //List<IEdge> Edges { get; set; }

        object[] VerticesValues { get; }

        /// <summary>
        /// Матрица инцидентности
        /// </summary>
        Matrix IncidentsMatrix { get; }

        /// <summary>
        /// Верхние заголовки матрицы инцидентности
        /// (В двудольном графе верхние и левые заголовки отличаются).
        /// </summary>
        string[] IncidentsMatrixTopHeaders { get; }

        /// <summary>
        /// Левые заголовки матрицы инцидентности
        /// (В двудольном графе верхние и левые заголовки отличаются).
        /// </summary>
        string[] IncidentsMatrixLeftHeaders { get; }

        /// <summary>
        /// Вершина в графе
        /// </summary>
        /// <param name="vertexValue">"Содержимое" вершины</param>
        /// <returns>Вершина во всей своей красе</returns>
        IVertex this[object vertexValue] { get; }

        /// <summary>
        /// Ребро между двумя вершинами
        /// </summary>
        /// <param name="uValue">"Содеримое" вершины</param>
        /// <param name="vValue">"Содеримое" вершины</param>
        /// <returns>Целое ребро</returns>
        IEdge this[object uValue, object vValue] { get; }

        /// <summary>
        /// Добавление вершины в граф
        /// </summary>
        /// <param name="vertexValue">"Содержимое" вершины</param>
        void AddVertex(object vertexValue);

        IVertex GetVertex(Predicate<IVertex> match);
        List<IVertex> GetVertices(Predicate<IVertex> match);
        List<IVertex> GetVertices();

        IEdge GetEdge(Predicate<IEdge> match);
        List<IEdge> GetEdges(Predicate<IEdge> match);
        List<IEdge> GetEdges();

        /// <summary>
        /// Создание вершины соотвествующей типу текущего графа.
        /// Вершина не добавляется в граф!
        /// </summary>
        /// <param name="vertexValue">Значение вершины</param>
        /// <returns></returns>
        IVertex CreateVertex(object vertexValue);

        /// <summary>
        /// Создание дуги соотвествующей типу текущего графа.
        /// Дуга не добавляется в граф!
        /// </summary>
        /// <param name="u">Хвост</param>
        /// <param name="v">Голова</param>
        /// <returns></returns>
        IEdge CreateEdge(object u, object v);

        /// <summary>
        /// Удаление вершины
        /// </summary>
        /// <param name="vertexValue">"Содержимое" вершины</param>
        void RemoveVertex(object vertexValue);

        /// <summary>
        /// Добавление ребра между двумя вершинами
        /// </summary>
        /// <param name="u">"Содержимое" вершины</param>
        /// <param name="v">"Содержимое" вершины</param>
        void AddEdge(object u, object v);

        /// <summary>
        /// Удаление ребра между двумя вершинами
        /// </summary>
        /// <param name="u">"Содержимое" вершины</param>
        /// <param name="v">"Содержимое" вершины</param>
        void RemoveEdge(object u, object v);

        /// <summary>
        /// Удаление рёбер, концом которых является вершина
        /// </summary>
        /// <param name="vertexValue">"Содержимое" вершины</param>
        void RemoveEdges(object vertexValue);

        /// <summary>
        /// Событие добавления вершины. Должно срабатывать перед добавлением.
        /// </summary>
        event EventHandler<VerticesModifiedEventArgs> OnAddVertex;

        /// <summary>
        /// Событие удаления вершины. Должно срабатывать перед удалением.
        /// </summary>
        event EventHandler<VerticesModifiedEventArgs> OnRemoveVertex;

        /// <summary>
        /// Событие добавления вершины. Должно срабатывать после добавления.
        /// </summary>
        event EventHandler<VerticesModifiedEventArgs> OnVertexAdded;

        /// <summary>
        /// Событие удаления вершины. Должно срабатывать после удаления.
        /// </summary>
        event EventHandler<VerticesModifiedEventArgs> OnVertexRemoved;

        /// <summary>
        /// Событие добавления ребра. Должно срабатывать перед добавлением.
        /// </summary>
        event EventHandler<EdgesModifiedEventArgs> OnAddEdge;

        /// <summary>
        /// Событие удаления ребра. Должно срабатывать перед удалением.
        /// </summary>
        event EventHandler<EdgesModifiedEventArgs> OnRemoveEdge;

        /// <summary>
        /// Событие добавления ребра. Должно срабатывать после добавления.
        /// </summary>
        event EventHandler<EdgesModifiedEventArgs> OnEdgeAdded;

        /// <summary>
        /// Событие удаления ребра. Должно срабатывать после удаления.
        /// </summary>
        event EventHandler<EdgesModifiedEventArgs> OnEdgeRemoved;

        /// <summary>
        /// Копирование текущего объекта в объект, передаваемый как параметр.
        /// </summary>
        /// <param name="graph">Куда копировать</param>
        void CopyTo(out IGraph graph);

        /// <summary>
        /// Копирование параметров текущего объекта в уже существующий объект.
        /// </summary>
        /// <param name="graph"></param>
        void CopyTo(IGraph graph);

        /// <summary>
        /// Создаёт неполную копию графа
        /// </summary>
        /// <returns></returns>
        object Clone();

        void ClearEventHandlers();

        void GraphMerge(IGraph graph);

        IGraph GetMergedCopy(IGraph graph);
    }

    public class VerticesModifiedEventArgs : EventArgs
    {
        /// <summary>
        /// Вершина, над которой выполняется модификация.
        /// </summary>
        public IVertex Vertex { get; set; }

        public object VertexValue { get; set; }
        
        /// <summary>
        /// Статус модификации. Показывается характер модификации, либо тип ошибки.
        /// Если установить значение, то будет выполняться соотвествующая модификация.
        /// </summary>
        public ModificationStatus Status { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modification">Тип модификации.</param>
        /// <param name="successful">Удачность модификации.</param>
        /// <param name="vertex">Вершина, над которой выполняется модификация.</param>
        public VerticesModifiedEventArgs(ModificationStatus status, IVertex vertex, object vertexValue)
        {
            this.Vertex = vertex;
            this.VertexValue = vertexValue;
            Status = status;
        }
    }

    public class EdgesModifiedEventArgs : EventArgs
    {
        /// <summary>
        /// Ребро, над которым выполняется модификация.
        /// </summary>
        public IEdge Edge { get; set; }

        public object u { get; set; }
        
        public object v { get; set; }

        /// <summary>
        /// Статус модификации. Показывается характер модификации, либо тип ошибки.
        /// Если установить значение, то будет выполняться соотвествующая модификация.
        /// </summary>
        public ModificationStatus Status { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modification">Тип модификации.</param>
        /// <param name="successful">Удачность модификации.</param>
        /// <param name="edge">Ребро, над которым выполняется модификация.</param>
        public EdgesModifiedEventArgs(ModificationStatus status, IEdge edge, object u,object v)
        {
            this.Edge = edge;
            this.u = u;
            this.v = v;
            Status = status;
        }
    }

    public enum ModificationStatus { Successful, AlreadyExist, NotExist, Error, InvalidParameters ,Canceled }
}
