using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ExamCreator
{
    class ExamSection
    {
        public string SectionDescription { get; set; }
        public ObservableCollection<QuestionGroup> Groups { get; set; }

        public ExamSection()
        {
            Groups = new ObservableCollection<QuestionGroup>();
            SectionDescription = "New Section";
        }

        public ExamSection(XmlReader reader) : this()
        {
            SectionDescription = reader.GetAttribute("description");
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "QuestionGroup")
                            Groups.Add(new QuestionGroup(reader));
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
            writer.WriteStartElement("ExamSection");
            writer.WriteAttributeString("description", SectionDescription);

            foreach (QuestionGroup q in Groups)
                q.Save(writer);

            writer.WriteEndElement();
        }

        public override string ToString()
        {
            return SectionDescription;
        }
    }
}
