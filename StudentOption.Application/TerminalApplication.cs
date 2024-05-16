﻿namespace StudentOption.Application;

using StudentOption.Data;
using StudentOption.Classes;
using System.Text;

public class TerminalApplication(string connectionString)
{
    #region ConstStrings // TO-DO: Output formatting

    private const string _coursesText = "There are the following courses:";
    private const string _courseHeadersText = "ID\tTitle\tCategory\tExam Board";
    private const string _coursePromptText = "Please input the desired Course ID:";
    private const string _classSetText = "There are the following class sets for Course ID @1, Name @2, Category @3, Exam Board @4:";
    private const string _classSetWithStudentText = "There are the following class sets for Student ID @1, Name @2, DoB @3:";
    private const string _classSetHeadersText = "ID\tTeacher";
    private const string _classSetHeadersWithCourseText = "ID\tCourse\tTeacher";
    private const string _classSetPromptText = "Please input the desired Class Set ID:";
    private const string _studentText = "There are the following students for Class Set ID @1 in Subject @2 with Teacher @3:";
    private const string _studentsText = "There are the following students:";
    private const string _studentHeadersText = "ID\tName\tDate of Birth";
    private const string _studentPromptText = "Please input the desired Student ID:";
    private const string _waitToContinueText = "Press enter to continue ...";
    private const string _studentValidateText = "There are @0 students for Class Set ID @1 in Subject @2 with Teacher @3.";
    private const string _validText = "It is valid.";
    private const string _invalidText = "It is invalid.";
    private const string _mainPromptText = @"Please input the relavant number to execute the relavant function.
1. Display classes for a course subject.
2. Display students enrolled in a class.
3. Display the classes a student is enrolled in.
4. Add/Remove/Update data. *** NOT IMPLEMENTED ***
5. Validate class for number of students enrolled.
6. Validate student subject choices. *** NOT IMPLEMENTED ***

0. Exit

Please input your choice:";

    #endregion
    private readonly StudentOptionDB _dataBase = new(connectionString);
    public void MainInterface()
    {
        int choice = -1;
        while (choice != 0)
        {
            Console.Clear();
            Console.WriteLine(_mainPromptText);

            string input = Console.ReadLine() ?? string.Empty;

            if (int.TryParse(input, out choice) && choice >= 0 && choice <= 6)
            {
                try
                {
                    switch (choice)
                    {
                        case 1:
                            ClassFromCourseInterface();
                            break;
                        case 2:
                            StudentFromClassInterface();
                            break;
                        case 3:
                            ClassFromStudentInterface();
                            break;
                        case 4:
                            EditInterface();
                            break;
                        case 5:
                            ValidateClassInterface();
                            break;
                        case 6:
                            ValidateStudentInterface();
                            break;
                        default:
                            break;
                    }
                }
                catch (NotImplementedException e)
                {
                    Console.Clear();
                    Console.WriteLine($"Not Implemented: {e.Source}, {e.Message}");
                    Console.WriteLine(_waitToContinueText);
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.Clear();
                    Console.WriteLine($"Exception: {e.Source}, {e.Message}");
                    Console.WriteLine(_waitToContinueText);
                    Console.ReadLine();
                }

            }
            else
            {
                choice = -1;
            }
        }

        return;
    }

    #region DisplayMethods

    private string DisplayCourses()
    {
        StringBuilder sb = new();
        List<Course> courses = _dataBase.GetCourses();

        sb.AppendLine(_coursesText);
        sb.AppendLine(_courseHeadersText);
        foreach (Course course in courses)
        {
            sb.AppendLine($"{course.ID}\t{course.Title}\t{course.Category}\t{course.ExamBoard}");
        }

        return sb.ToString();
    }

    private string DisplayClassSetsFromCourse(Course course)
    {
        StringBuilder sb = new();
        List<ClassSet> classSets = _dataBase.GetClassSetsFromCoruse(course);

        sb.AppendLine(_classSetText.Replace("@1", course.ID.ToString()).Replace("@2", course.Title).Replace("@3", course.Category).Replace("@4", course.ExamBoard));

        sb.AppendLine(_classSetHeadersText);
        foreach (ClassSet classSet in classSets)
        {
            sb.AppendLine($"{classSet.ID}\t{classSet.Teacher.Title} {classSet.Teacher.FirstName} {classSet.Teacher.LastName}");
        }

        return sb.ToString();
    }

    private string DisplayStudentsFromClassSet(ClassSet classSet)
    {
        StringBuilder sb = new();
        List<Student> students = _dataBase.GetStudentsFromClassSet(classSet);

        sb.AppendLine(_studentText.Replace("@1", classSet.ID.ToString()).Replace("@2", classSet.Course.Title).Replace("@3", $"{classSet.Teacher.Title} {classSet.Teacher.FirstName} {classSet.Teacher.LastName}"));

        sb.AppendLine(_studentHeadersText);
        foreach (Student student in students)
        {
            sb.AppendLine($"{student.ID}\t{student.FirstName} {student.LastName}\t{student.DateOfBirth}");
        }

        return sb.ToString();
    }

    private string DisplayStudents()
    {
        StringBuilder sb = new();
        List<Student> students = _dataBase.GetStudents();

        sb.AppendLine(_studentsText);
        sb.AppendLine(_studentHeadersText);
        foreach (Student student in students)
        {
            sb.AppendLine($"{student.ID}\t{student.FirstName} {student.LastName}\t{student.DateOfBirth}");
        }

        return sb.ToString();
    }

    private string DisplayClassSetsFromStudent(Student student)
    {
        StringBuilder sb = new();
        List<ClassSet> classSets = _dataBase.GetClassSetsFromStudent(student);

        sb.AppendLine(_classSetWithStudentText.Replace("@1", student.ID.ToString()).Replace("@2", $"{student.FirstName} {student.LastName}").Replace("@3", student.DateOfBirth.ToString()));

        sb.AppendLine(_classSetHeadersWithCourseText);
        foreach (ClassSet classSet in classSets)
        {
            sb.AppendLine($"{classSet.ID}\t{classSet.Course.Title}\t{classSet.Teacher.Title} {classSet.Teacher.FirstName} {classSet.Teacher.LastName}");
        }

        return sb.ToString();
    }

    #endregion

    #region ChooseInterfaces

    private Course ChooseCourseInterface()
    {
        bool valid = false;
        Course courseChoice = Course.Default;
        string displayCourses = DisplayCourses();

        while (!valid)
        {
            Console.Clear();
            Console.WriteLine(displayCourses);

            Console.WriteLine(_coursePromptText);
            string input = Console.ReadLine() ?? string.Empty;

            if (int.TryParse(input, out int idChoice) && _dataBase.ExistCourseID(idChoice))
            {
                courseChoice = _dataBase.GetCourseByID(idChoice);
                valid = true;
            }
        }

        return courseChoice;
    }

    private ClassSet ChooseClassSetFromCourseInterface(Course course)
    {
        bool valid = false;
        ClassSet classSetChoice = ClassSet.Default;
        string displayClassSetsFromCourse = DisplayClassSetsFromCourse(course);

        while (!valid)
        {
            Console.Clear();
            Console.WriteLine(displayClassSetsFromCourse);

            Console.WriteLine(_classSetPromptText);
            string input = Console.ReadLine() ?? string.Empty;

            if (int.TryParse(input, out int idChoice) && _dataBase.ExistClassSetIDWithCourse(course, idChoice))
            {
                classSetChoice = _dataBase.GetClassSetByIDWithCourse(course, idChoice);
                valid = true;
            }
        }

        return classSetChoice;
    }

    private Student ChooseStudentInterface()
    {
        bool valid = false;
        Student studentChoice = Student.Default;
        string displayStudents = DisplayStudents();

        while (!valid)
        {
            Console.Clear();
            Console.WriteLine(displayStudents);

            Console.WriteLine(_studentPromptText);
            string input = Console.ReadLine() ?? string.Empty;

            if (int.TryParse(input, out int idChoice) && _dataBase.ExistStudentID(idChoice))
            {
                studentChoice = _dataBase.GetStudentByID(idChoice);
                valid = true;
            }
        }

        return studentChoice;
    }

    #endregion

    #region FunctionalInteraces

    private void ClassFromCourseInterface()
    {
        Course courseChoice = ChooseCourseInterface();
        string displayClassSets = DisplayClassSetsFromCourse(courseChoice);

        Console.Clear();
        Console.WriteLine(displayClassSets);

        Console.WriteLine(_waitToContinueText);
        Console.ReadLine();

        return;
    }

    private void StudentFromClassInterface()
    {
        ClassSet classSetChoice = ChooseClassSetFromCourseInterface(ChooseCourseInterface());
        string displayStudentsFromClassSet = DisplayStudentsFromClassSet(classSetChoice);

        Console.Clear();
        Console.WriteLine(displayStudentsFromClassSet);

        Console.WriteLine(_waitToContinueText);
        Console.ReadLine();

        return;
    }

    private void ClassFromStudentInterface()
    {
        Student studentChoice = ChooseStudentInterface();
        string displayClassSetsFromStudent = DisplayClassSetsFromStudent(studentChoice);

        Console.Clear();
        Console.WriteLine(displayClassSetsFromStudent);

        Console.WriteLine(_waitToContinueText);
        Console.ReadLine();

        return;
    }

    private void EditInterface()
    {
        throw new NotImplementedException();
    }

    private const int _minStudentNo = 5;
    private const int _maxStudentNo = 15;

    private void ValidateClassInterface()
    {
        ClassSet classSet = ChooseClassSetFromCourseInterface(ChooseCourseInterface());
        int studentNo = _dataBase.GetStudentNoByClassSet(classSet);

        Console.Clear();
        Console.WriteLine(_studentValidateText.Replace("@0", studentNo.ToString()).Replace("@1", classSet.ID.ToString()).Replace("@2", classSet.Course.Title).Replace("@3", $"{classSet.Teacher.Title} {classSet.Teacher.FirstName} {classSet.Teacher.LastName}"));

        if (studentNo >= _minStudentNo && studentNo <= _maxStudentNo)
        {
            Console.WriteLine(_validText);
        }
        else
        {
            Console.WriteLine(_invalidText);
        }

        Console.WriteLine(_waitToContinueText);
        Console.ReadLine();

        return;
    }

    private void ValidateStudentInterface()
    {
        throw new NotImplementedException();
    }

    #endregion
}