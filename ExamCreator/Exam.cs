using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ExamCreator
{
    class Exam
    {
        public string ExamDescription { get; set; }
        public ObservableCollection<ExamSection> Sections { get; set; }

        /* Create a new Exam */
        public Exam()
        {
            Sections = new ObservableCollection<ExamSection>();
            ExamDescription = "New Exam";
        }

        /* Load an existing Exam */
        public Exam(string examFile) : this()
        {
            XmlReader reader = XmlReader.Create(examFile);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        /* Check if Element is Exam; if so, load attributes.
                         * Otherwise, check if ExamSection; if so, create new
                         * ExamSection. Otherwise, skip element.
                         */
                        if (reader.Name == "Exam")
                            ExamDescription = reader.GetAttribute("description");
                        else if (reader.Name == "ExamSection")
                            Sections.Add(new ExamSection(reader));
                        else
                            reader.Skip();
                        break;
                    default:
                        break;
                }
            }
        }

        public void Save(string examFile)
        {
            XmlWriter writer = XmlWriter.Create(examFile);
            writer.WriteStartElement("Exam");
            writer.WriteAttributeString("description", ExamDescription);

            foreach (ExamSection s in Sections)
                s.Save(writer);

            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }

        public override string ToString()
        {
            return ExamDescription;
        }
    }
}
