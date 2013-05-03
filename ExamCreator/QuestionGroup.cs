using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ExamCreator
{
    class QuestionGroup
    {
        public string GroupDescription { get; set; }
        public ObservableCollection<ExamQuestion> Questions { get; set; }

        public QuestionGroup()
        {
            Questions = new ObservableCollection<ExamQuestion>();
            GroupDescription = "New Question Group";
        }

        public QuestionGroup(XmlReader reader) : this()
        {
            GroupDescription = reader.GetAttribute("description");

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "ExamQuestion")
                            Questions.Add(new ExamQuestion(reader));
                        else
                            reader.Skip();
                        break;
                    case XmlNodeType.EndElement:
                        return;
                    default:
                        break;
                }
            }
        }

        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement("QuestionGroup");
            writer.WriteAttributeString("description", GroupDescription);

            foreach (ExamQuestion q in Questions)
                q.Save(writer);

            writer.WriteEndElement();
        }

        public override string ToString()
        {
            return GroupDescription;
        }
    }
}
