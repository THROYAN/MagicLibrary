using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.Graphs;

namespace MagicLibrary.MathUtils.SemanticNetworkUtils.Graphs
{
    [Serializable]
    public class RelationArc : Arc
    {
        public int Id { get; set; }

        public SemanticGraph SemanticGraph { get { return this.Graph as SemanticGraph; } }

        public string Name
        {
            get
            {
                return this.SemanticGraph.GetRelationDefinition(this.Id).Name;
            }
        }

        public int TypeId
        {
            get
            {
                return this.SemanticGraph.GetRelationDefinition(this.Id).TypeId;
            }
        }

        public RelationDefinition RelationDefinition
        {
            get
            {
                return this.SemanticGraph.GetRelationDefinition(this.Id);
            }
            set
            {
                this.Id = value.Id;
            }
        }

        public RelationArc(SemanticGraph graph, string tailName, int id, string headName)
            : base(graph, tailName, headName)
        {
            this.Id = id;
        }

        public RelationArc(SemanticGraph graph, int tailId, int id, int headId)
            : base(graph, graph.GetConceptNameById(tailId), graph.GetConceptNameById(headId))
        {
            this.Id = id;
        }
    }
}
