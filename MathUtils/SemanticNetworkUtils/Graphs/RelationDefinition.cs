using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLibrary.MathUtils.SemanticNetworkUtils.Graphs
{
    [Serializable]
    public class RelationDefinition : ICloneable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }

        public RelationDefinition(int id, string name, int typeId = RelationDefinition.newRelationId)
        {
            this.Id = id;
            this.Name = name;
            this.TypeId = typeId;
        }

        public bool IsTransitive()
        {
            return this.TypeId == RelationDefinition.inheritableId || this.TypeId == RelationDefinition.transitiveId;
        }

        public bool IsInheritable()
        {
            return this.TypeId == RelationDefinition.inheritableId;
        }

        public const int inheritableId = 1;
        public const int transitiveId = 2;
        public const int newRelationId = 0;

        public virtual object Clone()
        {
            return new RelationDefinition(this.Id, this.Name, this.TypeId);
        }
    }
}
