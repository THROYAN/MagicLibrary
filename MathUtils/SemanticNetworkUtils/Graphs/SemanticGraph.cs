using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.SemanticNetworkUtils.Graphs
{
    [Serializable]
    public class SemanticGraph : DirectedGraph
    {
        private long _max_id = 10000;

        public List<RelationDefinition> RelationsDefinitions { get; set; }

        private int _currentId;
        public int _currentArcId;
        public SemanticGraph()
            : base()
        {
            RelationsDefinitions = new List<RelationDefinition>();
            this._currentId = 1;
            this._currentArcId = -1;

            SemanticGraph.SetDefaultEventHandlers(this);
        }

        public static void SetDefaultEventHandlers(SemanticGraph graph)
        {
            DirectedGraph.SetDefaultEventHandlers(graph);

            graph.OnAddVertex += new EventHandler<VerticesModifiedEventArgs>(graph.graph_OnAddVertex);
            graph.OnAddEdge += new EventHandler<EdgesModifiedEventArgs>(graph.graph_OnAddEdge);
        }

        public override IVertex CreateVertex(object vertexValue)
        {
            return new ConceptVertex(this, this._currentId, vertexValue.ToString());
        }

        public override IEdge CreateEdge(object u, object v)
        {
            return new RelationArc(this, u.ToString(), this._currentArcId, v.ToString());
        }

        void graph_OnAddEdge(object sender, EdgesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                // block loops
                if (e.u.Equals(e.v))
                {
                    e.Status = ModificationStatus.AlreadyExist;
                    return;
                }
                // invalid id
                if (this.GetRelationDefinition(this._currentArcId) == null)
                {
                    e.Status = ModificationStatus.InvalidParameters;
                    return;
                }
            }
        }

        void graph_OnAddVertex(object sender, VerticesModifiedEventArgs e)
        {
            if (e.Status == ModificationStatus.Successful)
            {
                // check exists ids
                if (this.GetVertices(v => (v as ConceptVertex).Id == this._currentId).Count != 0)
                {
                    e.Status = ModificationStatus.AlreadyExist;
                    return;
                }
            }
        }

        public string GetConceptNameById(int conceptId)
        {
            var c = this.GetVertex(v => (v as ConceptVertex).Id == conceptId) as ConceptVertex;

            return c != null ? c.Name : null;
        }

        public void AddRelationType(int id, string name, int typeId = RelationDefinition.newRelationId)
        {
            this.RelationsDefinitions.Add(new RelationDefinition(id, name, typeId));
        }

        public void AddConcept(int id, string name)
        {
            this._currentId = id;
            this.AddVertex(name);
        }

        public void AddConcept(string name)
        {
            this._currentId = this.GetFreeConceptId();
            this.AddVertex(name);
        }

        public void AddRelation(int conceptId1, int relationId, int conceptId2)
        {
            this._currentArcId = relationId;

            this.AddArc(this.GetConceptNameById(conceptId1), this.GetConceptNameById(conceptId2));

            this._currentArcId = -1;
        }

        public ConceptVertex GetConceptById(int conceptId)
        {
            return this.GetVertex(v => (v as ConceptVertex).Id == conceptId) as ConceptVertex;
        }

        public RelationArc GetRelation(int conceptId1, int conceptId2)
        {
            var c1 = this.GetConceptById(conceptId1);
            var c2 = this.GetConceptById(conceptId2);
            return this[c1.Value, c2.Value] as RelationArc;
        }

        public RelationDefinition GetRelationDefinition(int id)
        {
            return this.RelationsDefinitions.Find(rd => rd.Id == id);
        }

        public RelationArc FindRelation(int conceptId1, int conceptId2)
        {
            var c1 = this.GetConceptById(conceptId1);
            var c2 = this.GetConceptById(conceptId2);
            var r = this[c1.Value, c2.Value] as RelationArc;
            if (r != null)
                return r;
            foreach (RelationArc arc in this.GetRelations(conceptId1))
            {
                if (arc.RelationDefinition.IsInheritable())
                {
                    var rArc = this.FindRelation((arc.Head as ConceptVertex).Id, conceptId2);
                    if (rArc != null)
                        return rArc;
                }
                else
                {
                    if (arc.RelationDefinition.IsTransitive())
                    {
                        var rArc = this.FindTransitiveRelation((arc.Head as ConceptVertex).Id, conceptId2, arc.Id);
                        if (rArc != null)
                            return rArc;
                    }
                }
            }
            return null;
        }

        private RelationArc FindTransitiveRelation(int conceptId1, int conceptId2, int relationId)
        {
            var c1 = this.GetConceptById(conceptId1);
            var c2 = this.GetConceptById(conceptId2);
            var r = this[c1.Value, c2.Value] as RelationArc;
            if (r != null && r.Id == relationId)//r.RelationDefinition.IsTransitive() && !r.RelationDefinition.IsInheritable())
                return r;
            foreach (RelationArc arc in this.GetRelations(conceptId1))
            {
                if (relationId == arc.Id)//!arc.RelationDefinition.IsInheritable() && arc.RelationDefinition.IsTransitive())
                {
                    var rArc = this.FindTransitiveRelation((arc.Head as ConceptVertex).Id, conceptId2, relationId);
                    if (rArc != null)
                        return rArc;
                }
            }
            return null;
        }

        public RelationArc[] GetRelations(int conceptId)
        {
            var c = this.GetConceptById(conceptId);

            return this.GetEdges(e => (e as Arc).Tail.Value.Equals(c.Value)).OfType<RelationArc>().ToArray();
        }

        public override Matrix IncidentsMatrix
        {
            get
            {
                Matrix matrix = new Matrix(VerticesValues.Length, VerticesValues.Length);
                for (int i = 0; i < VerticesValues.Length; i++)
                {
                    for (int j = 0; j < VerticesValues.Length; j++)
                    {
                        matrix[i, j] = this[VerticesValues[i], VerticesValues[j]] == null ?
                            //this[VerticesValues[j], VerticesValues[i]] == null ?
                                                new MatrixElementWithVariables(" ") :
                            //new DoubleMatrixElement(-1) :
                                            new MatrixElementWithVariables(
                                                (this[VerticesValues[i], VerticesValues[j]] as RelationArc).Name);
                    }
                }
                return matrix;
            }
        }

        public override void CopyTo(IGraph graph)
        {
            base.CopyTo(graph);

            var s = graph as SemanticGraph;
            s.RelationsDefinitions = new List<RelationDefinition>(this.RelationsDefinitions);

            SemanticGraph.SetDefaultEventHandlers(graph as SemanticGraph);
        }

        public override object Clone()
        {
            SemanticGraph s = new SemanticGraph();

            this.CopyTo(s);

            return s;
        }

        public int GetFreeConceptId()
        {
            for (int i = 0; i <= this._max_id; i++)
            {
                if (this.GetConceptById(i) == null)
                    return i;
            }
            return 0;
        }
    }
}
