using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MagicLibrary.MathUtils.SemanticNetworkUtils.Graphs;

namespace MagicLibrary.MathUtils.SemanticNetworkUtils
{
    [Serializable]
    public class SemanticNetwork
    {
        public SemanticGraph Graph { get; set; }

        public SemanticNetwork(SemanticGraph graph)
        {
            this.Graph = graph;
        }

        public string Question(int conceptId1, int relationId, int conceptId2, bool fullDescription = false)
        {
            if (this.Graph.GetConceptById(conceptId1) == null && conceptId1 != SemanticNetwork.anyValue)
            {
                return String.Format("Не правильный запрос:\n - понятия с идентификатором {0} нет в базе\n", conceptId1);
            }

            if (this.Graph.GetConceptById(conceptId2) == null && conceptId2 != SemanticNetwork.anyValue)
            {
                return String.Format("Не правильный запрос:\n - понятия с идентификатором {0} нет в базе\n", conceptId2);
            }

            if (!this.Graph.RelationsDefinitions.Exists(rd => rd.Id == relationId) && relationId != SemanticNetwork.anyValue)
            {
                return String.Format("Не правильный запрос:\n - связи с идентификатором {0} нет в базе\n", relationId);
            }
            
            StringBuilder sb = new StringBuilder();

            if (conceptId1 == SemanticNetwork.anyValue)
            {
                this.Graph.GetVertices().OfType<ConceptVertex>().ToList().ForEach(delegate(ConceptVertex c)
                {
                    string s = this.Question(c.Id, relationId, conceptId2, fullDescription);

                    if (s != SemanticNetwork.error_message)
                        sb.Append(s);
                });
                //sb.Remove(sb.Length - 1, 1);
                //return sb.ToString();
            }
            else
            {
                var c1 = this.Graph.GetConceptById(conceptId1);

                if (conceptId2 == SemanticNetwork.anyValue)
                {
                    this.Graph.GetVertices().OfType<ConceptVertex>().ToList().ForEach(delegate(ConceptVertex c)
                    {
                        if (c.Id != conceptId1)
                        {
                            string s = this.Question(conceptId1, relationId, c.Id, fullDescription);
                            if (s != SemanticNetwork.error_message)
                                //if (fullDescription)
                                //{
                                //    sb.AppendLine(s);
                                //}
                                //else
                                {
                                    sb.Append(s);
                                }
                        }
                    });
                    //sb.Remove(sb.Length - 1, 1);
                    //return sb.ToString();
                }
                else
                {
                    var c2 = this.Graph.GetConceptById(conceptId2);

                    var relBetween = this.Graph.FindRelation(conceptId1, conceptId2);//GetRelation(conceptId1, conceptId2);

                    if (relBetween != null && (relBetween.Id == relationId || relationId == SemanticNetwork.anyValue))
                    {
                        if (relationId == SemanticNetwork.anyValue || fullDescription == false)
                            sb.AppendFormat(" - [{0}] {1} [{2}]\n", c1.Name, relBetween.Name, c2.Name);
                        else
                            sb.AppendFormat(" - {0}\n", SemanticNetwork.yes_message);
                    }
                    else
                    {
                        if(fullDescription)
                            sb.AppendFormat(" - {0}\n", SemanticNetwork.error_message);
                    }
                    //return String.Format("{0} {1} {2}", c1.Name, relBetween.Name, c2.Name);

                    //if (relationId != SemanticNetwork.anyValue && this.Graph.GetRelationDefinition(relationId).TypeId == SemanticGraph.RelationsTypes.ToList().IndexOf(""))
                    //    return SemanticNetwork.error_message;

                    //foreach (RelationArc arc in this.Graph.GetRelations(conceptId1))
                    //{
                    //    if (arc.TypeId == SemanticGraph.RelationsTypes.ToList().IndexOf("is-a")

                    //            ||

                    //        (
                    //            arc.TypeId == SemanticGraph.RelationsTypes.ToList().IndexOf("has-part") &&
                    //            (relationId == SemanticNetwork.anyValue || this.Graph.GetRelationDefinition(relationId).TypeId == SemanticGraph.RelationsTypes.ToList().IndexOf("has-part"))
                    //        )
                    //    )
                    //    {
                    //        string s = this.Question((arc.Head as ConceptVertex).Id, relationId, conceptId2);

                    //        //relBetween = this.Graph.GetRelation((arc.Head as ConceptVertex).Id, conceptId2);


                    //        if (s != SemanticNetwork.error_message)
                    //            //return s;
                    //            sb.AppendFormat("{0} {1} {2}\n", c1.Name, this.Graph.GetRelationDefinition(relationId).Name, c2.Name);
                    //            //sb.Append(s);
                    //    }
                    //}
                }
            }

            if (sb.Length != 0)
            {
                //sb.Remove(sb.Length - 1, 1);

                if (fullDescription)
                {
                    StringBuilder sb2 = new StringBuilder();
                    sb2.AppendFormat("[{0}] {1} [{2}]?\n",
                        conceptId1 == SemanticNetwork.anyValue ? SemanticNetwork.anyFirstConceptString : this.Graph.GetConceptNameById(conceptId1),
                        relationId == SemanticNetwork.anyValue ? SemanticNetwork.anyRelationString : this.Graph.GetRelationDefinition(relationId).Name,
                        conceptId2 == SemanticNetwork.anyValue ? SemanticNetwork.anySecondConceptString : this.Graph.GetConceptNameById(conceptId2));
                    sb2.Append(sb.ToString());

                    return sb2.ToString();
                }
                return sb.ToString();
            }

            return SemanticNetwork.error_message;
        }

        public const int anyValue = -1;
        public const string error_message = "нет";
        public const string anyFirstConceptString = "Кто";
        public const string anyRelationString = "имеет отношение к";
        public const string anySecondConceptString = "что-либо";
        public const string anyValueString = "?";
        public const string yes_message = "да";
    }
}
