using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ExamCreator
{
    class ExamQuestion
    {
        public string QuestionIdentifier { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public ObservableCollection<string> Distractors { get; set; }
        public string Image { get; set; }

        public ExamQuestion()
        {
            Distractors = new ObservableCollection<string>();
            Question = "New Question";
            Answer = "Boilerplate Answer";
        }

        public ExamQuestion(XmlReader reader) : this()
        {
            QuestionIdentifier = reader.GetAttribute("identifier");
            Question = reader.GetAttribute("question");
            Image = reader.GetAttribute("image");

            while (!reader.EOF)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "Answer")
                            Answer = reader.ReadElementContentAsString();
                        else if (reader.Name == "Distractor")
                            Distractors.Add(reader.ReadElementContentAsString());
                        else
                            reader.Read();
                        break;
                    case XmlNodeType.EndElement:
                        return;
                    default:
                        reader.Read();
                        break;
                }
            }
        }

        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement("ExamQuestion");
            if (QuestionIdentifier != null && QuestionIdentifier != "")
                writer.WriteAttributeString("identifier", QuestionIdentifier);
            writer.WriteAttributeString("question", Question);
            if (Image != null && Image != "")
                writer.WriteAttributeString("image", Image);

            writer.WriteStartElement("Answer");
            writer.WriteString(Answer);
            writer.WriteEndElement();

            foreach (string d in Distractors)
            {
                writer.WriteStartElement("Distractor");
                writer.WriteString(d);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public override string ToString()
        {
            return Question;
        }
    }
}
