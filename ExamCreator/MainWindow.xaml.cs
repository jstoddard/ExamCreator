using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExamCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Exam Exam;

        public MainWindow()
        {
            InitializeComponent();
            /* No editing until an exam is opened or created */
            DisableEverything();

            Exam = null;
        }

        private void DisableEverything()
        {
            foreach (UIElement e in ExamGrid.Children)
            {
                if (e is Button || e is TextBox)
                    e.IsEnabled = false;
                if (e is ListBox)
                    (e as ListBox).ItemsSource = null;
            }
            foreach (UIElement e in QuestionDetailsStackPanel.Children)
            {
                if (e is TextBox || e is Button)
                    e.IsEnabled = false;
            }
        }

        private void DisableQuestionGroup()
        {
            QuestionGroupDelete.IsEnabled = false;
            QuestionGroupCreate.IsEnabled = false;
            QuestionGroupDescription.IsEnabled = false;
            QuestionGroupDescription.IsEnabled = false;
            QuestionGroupListBox.ItemsSource = null;
            DisableQuestions();
        }

        private void DisableQuestions()
        {
            QuestionDelete.IsEnabled = false;
            QuestionCreate.IsEnabled = false;
            QuestionIdentifier.IsEnabled = false;
            Question.IsEnabled = false;
            Answer.IsEnabled = false;
            QuestionSave.IsEnabled = false;
            QuestionsListBox.ItemsSource = null;
            DisableDistractors();
        }

        private void DisableDistractors()
        {
            DistractorDelete.IsEnabled = false;
            DistractorCreate.IsEnabled = false;
            DistractorTextBox.IsEnabled = false;
            DistractorSave.IsEnabled = false;
            DistractorsListBox.ItemsSource = null;
        }

        /* MENU BAR CALLBACKS */
        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Exam = new Exam();
            ExamSectionListBox.ItemsSource = Exam.Sections;
            ExamSectionCreate.IsEnabled = true;
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML documents (.xml)|*.xml";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                Exam = new Exam(dlg.FileName);
                ExamSectionListBox.ItemsSource = Exam.Sections;
                ExamSectionCreate.IsEnabled = true;
            }
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "exam.xml";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML documents (.xml)|*.xml";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                Exam.Save(dlg.FileName);
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /* EXAM SECTION CALLBACKS */
        private void ExamSectionCreate_Click(object sender, RoutedEventArgs e)
        {
            // Create a new ExamSection
            ExamSection section = new ExamSection();
            Exam.Sections.Add(section);
            // Select the newly added item (by selecting the last item in the ListBox)
            ExamSectionListBox.SelectedIndex = ExamSectionListBox.Items.Count - 1;
        }

        private void ExamSectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExamSectionListBox.SelectedIndex == -1)
            {
                // Nothing is selected
                ExamSectionDescription.Text = "";
                ExamSectionDescription.IsEnabled = false;
                ExamSectionSave.IsEnabled = false;
                ExamSectionDelete.IsEnabled = false; // You can't delete nothing!
                DisableQuestionGroup();
            }
            else
            {
                // Show selection in description TextBox, allow updating
                ExamSection section = ExamSectionListBox.SelectedItem as ExamSection;
                ExamSectionDescription.Text = section.SectionDescription;
                ExamSectionDescription.IsEnabled = true;
                ExamSectionSave.IsEnabled = true;
                ExamSectionDelete.IsEnabled = true;
                // Set Question Group, enable adding new groups to selected section
                QuestionGroupListBox.ItemsSource = section.Groups;
                QuestionGroupCreate.IsEnabled = true;
            }
        }

        private void ExamSectionDelete_Click(object sender, RoutedEventArgs e)
        {
            Exam.Sections.Remove(ExamSectionListBox.SelectedItem as ExamSection);
        }

        private void ExamSectionSave_Click(object sender, RoutedEventArgs e)
        {
            ExamSection section = ExamSectionListBox.SelectedItem as ExamSection;
            section.SectionDescription = ExamSectionDescription.Text;
            ExamSectionListBox.Items.Refresh();
        }

        private void ExamSectionDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                ExamSectionSave_Click(sender, null);
        }

        /* QUESTION GROUP CALLBACKS */
        private void QuestionGroupCreate_Click(object sender, RoutedEventArgs e)
        {
            // Create a new QuestionGroup
            QuestionGroup group = new QuestionGroup();
            // This QuestionGroup belongs in the currently selected ExamSection
            (ExamSectionListBox.SelectedItem as ExamSection).Groups.Add(group);
            // Select the newly added QuestionGroup
            QuestionGroupListBox.SelectedIndex = QuestionGroupListBox.Items.Count - 1;
        }

        private void QuestionGroupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionGroupListBox.SelectedIndex == -1)
            {
                // Nothing selected
                QuestionGroupDescription.Text = "";
                QuestionGroupDescription.IsEnabled = false;
                QuestionGroupSave.IsEnabled = false;
                QuestionGroupDelete.IsEnabled = false;
                DisableQuestions();
            }
            else
            {
                // Show selection in description TextBox, allow updating
                QuestionGroup group = QuestionGroupListBox.SelectedItem as QuestionGroup;
                QuestionGroupDescription.Text = group.GroupDescription;
                QuestionGroupDescription.IsEnabled = true;
                QuestionGroupSave.IsEnabled = true;
                QuestionGroupDelete.IsEnabled = true;
                // Set Question List, enable adding new questions
                QuestionsListBox.ItemsSource = group.Questions;
                QuestionCreate.IsEnabled = true;
            }
        }

        private void QuestionGroupDelete_Click(object sender, RoutedEventArgs e)
        {
            (ExamSectionListBox.SelectedItem as ExamSection).Groups.Remove(QuestionGroupListBox.SelectedItem as QuestionGroup);
        }

        private void QuestionGroupSave_Click(object sender, RoutedEventArgs e)
        {
            QuestionGroup group = QuestionGroupListBox.SelectedItem as QuestionGroup;
            group.GroupDescription = QuestionGroupDescription.Text;
            QuestionGroupListBox.Items.Refresh();
        }

        private void QuestionGroupDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                QuestionGroupSave_Click(sender, null);
        }

        /* QUESTIONS CALLBACKS */
        private void QuestionCreate_Click(object sender, RoutedEventArgs e)
        {
            // Create a new ExamQuestion
            ExamQuestion question = new ExamQuestion();
            // This ExamQuestion belongs in the currently selected QuestionGroup
            (QuestionGroupListBox.SelectedItem as QuestionGroup).Questions.Add(question);
            // Select the newly added question
            QuestionsListBox.SelectedIndex = QuestionsListBox.Items.Count - 1;
        }

        private void QuestionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionsListBox.SelectedIndex == -1)
            {
                // Nothing selected
                QuestionIdentifier.Text = "";
                Question.Text = "";
                Answer.Text = "";
                Image.Text = "";
                QuestionIdentifier.IsEnabled = false;
                Question.IsEnabled = false;
                Answer.IsEnabled = false;
                Image.IsEnabled = false;
                QuestionSave.IsEnabled = false;
                QuestionDelete.IsEnabled = false;
                DisableDistractors();
            }
            else
            {
                // Show details of selected question, allow updating
                ExamQuestion question = (QuestionsListBox.SelectedItem as ExamQuestion);
                QuestionIdentifier.Text = question.QuestionIdentifier;
                Question.Text = question.Question;
                Answer.Text = question.Answer;
                Image.Text = question.Image;
                QuestionIdentifier.IsEnabled = true;
                Question.IsEnabled = true;
                Answer.IsEnabled = true;
                Image.IsEnabled = true;
                QuestionSave.IsEnabled = true;
                QuestionDelete.IsEnabled = true;

                // Set distractors
                DistractorsListBox.ItemsSource = question.Distractors;
                DistractorCreate.IsEnabled = true;
            }
        }

        private void QuestionDelete_Click(object sender, RoutedEventArgs e)
        {
            (QuestionGroupListBox.SelectedItem as QuestionGroup).Questions.Remove(QuestionsListBox.SelectedItem as ExamQuestion);
        }

        private void QuestionSave_Click(object sender, RoutedEventArgs e)
        {
            ExamQuestion question = (QuestionsListBox.SelectedItem as ExamQuestion);
            question.QuestionIdentifier = QuestionIdentifier.Text;
            question.Question = Question.Text;
            question.Answer = Answer.Text;
            question.Image = Image.Text;
            QuestionsListBox.Items.Refresh();
        }

        private void Question_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                QuestionSave_Click(sender, null);
        }

        /* DISTRACTORS CALLBACKS */
        private void DistractorCreate_Click(object sender, RoutedEventArgs e)
        {
            // Create a new distractor (string)
            string distractor = "New Distractor";
            (QuestionsListBox.SelectedItem as ExamQuestion).Distractors.Add(distractor);
            DistractorsListBox.SelectedIndex = DistractorsListBox.Items.Count - 1;
        }

        private void DistractorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DistractorsListBox.SelectedIndex == -1)
            {
                // Nothing selected
                DistractorTextBox.Text = "";
                DistractorTextBox.IsEnabled = false;
                DistractorSave.IsEnabled = false;
                DistractorDelete.IsEnabled = false;
            }
            else
            {
                // Allow editing of selected distractor
                DistractorTextBox.Text = DistractorsListBox.SelectedItem as string;
                DistractorTextBox.IsEnabled = true;
                DistractorSave.IsEnabled = true;
                DistractorDelete.IsEnabled = true;
            }
        }

        private void DistractorDelete_Click(object sender, RoutedEventArgs e)
        {
            (QuestionsListBox.SelectedItem as ExamQuestion).Distractors.Remove(DistractorsListBox.SelectedItem as string);
        }

        private void DistractorSave_Click(object sender, RoutedEventArgs e)
        {
            string distractor = DistractorTextBox.Text;
            ExamQuestion question = (QuestionsListBox.SelectedItem as ExamQuestion);
            int i = question.Distractors.IndexOf(DistractorsListBox.SelectedItem as string);
            question.Distractors[i] = distractor;
        }

        private void DistractorTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                DistractorSave_Click(sender, null);
        }
    }
}
