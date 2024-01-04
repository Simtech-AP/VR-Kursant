using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Enumeration allowing for differentiation of tasks
/// </summary>
public enum Task
{
    TASK_1 = 0,
    TASK_2 = 1,
    TASK_3 = 2,
    TASK_4 = 3,
    TASK_5 = 4,
    TASK_6 = 5
}

/// <summary>
/// Class representing a single test points sctructure
/// </summary>
[System.Serializable]
public class TestPoint
{
    /// <summary>
    /// Name of this test point
    /// </summary>
    public string Name;
    /// <summary>
    /// Points awarded for completing the task
    /// </summary>
    public int Points;
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="v1">Name of the test point award</param>
    /// <param name="v2">Amount of points awarded</param>
    public TestPoint(string v1, int v2)
    {
        Name = v1;
        Points = v2;
    }
    /// <summary>
    /// Sets current points for test award
    /// </summary>
    /// <param name="v">Amount of points to award</param>
    public void SetPoints(int v)
    {
        Points = v;
    }
}

/// <summary>
/// Class containing data for test 
/// </summary>
[System.Serializable]
public class TestData
{
    public int? customId = null;
    /// <summary>
    /// Name of user
    /// </summary>
    public string Name = "Guest";
    /// <summary>
    /// Time taken to finish a single task
    /// </summary>
    public float Time;
    /// <summary>
    /// List of errors that user can do in a test
    /// </summary>
    public List<TestPoint> Errors;
    /// <summary>
    /// List of test elements awarding points
    /// </summary>
    public List<TestPoint> Points;
    /// <summary>
    /// Total count of test points
    /// </summary>
    public int TotalPoints;
    /// <summary>
    /// Dictionary containing all task points
    /// </summary>
    public Dictionary<Task, Tuple<float, int>> TaskPoints;
    /// <summary>
    /// Total time that test has taken
    /// </summary>
    public float TotalTime = 0;
}

/// <summary>
/// Module allowing test results and tracking
/// </summary>
// TODO: This code is hard to understand in parts and replacable with single methods elsewhere, check for refactor
public class TestModule : MonoBehaviour
{
    /// <summary>
    /// Main conatiner of test data
    /// </summary>
    [SerializeField]
    public TestData Data;
    /// <summary>
    /// Dictionary of actions connected to tasks
    /// </summary>
    private Dictionary<Task, Action[]> taskActions;
    /// <summary>
    /// Flag allowing for enabling and disabling counting time of the test
    /// </summary>
    private bool Pending = false;
    /// <summary>
    /// Reference to instruction controller on scene
    /// </summary>
    private InstructionController iController;

    /// <summary>
    /// Sets up references
    /// Initializes default data
    /// Connects the tasks to actions
    /// </summary>
    private void Awake()
    {
        iController = ControllersManager.Instance.GetController<InstructionController>();

        // Data.customId = 2;
        Data.Time = 0f;
        Data.Name = "Guest";
        Data.Errors = new List<TestPoint>();
        Data.Points = new List<TestPoint>();
        Data.TotalPoints = 0;
        Data.TotalTime = 0;

        taskActions = new Dictionary<Task, Action[]>();

        taskActions.Add(Task.TASK_1, new Action[2]
        {
            () =>
            {
                Task1();
            },
            () =>
            {
                FinishTask(Task.TASK_1);
            }
        });
        taskActions.Add(Task.TASK_2, new Action[2]
        {
            () =>
            {
                Task2();
            },
            () =>
            {
                FinishTask(Task.TASK_2);
            }
        });
        taskActions.Add(Task.TASK_3, new Action[2]
        {
            () =>
            {
                Task3();
            },
            () =>
            {
                FinishTask(Task.TASK_3);
            }
        });
        taskActions.Add(Task.TASK_4, new Action[2]
        {
            () =>
            {
                Task4();
            },
            () =>
            {
                FinishTask(Task.TASK_4);
            }
        });
        taskActions.Add(Task.TASK_5, new Action[2]
        {
            () =>
            {
                Task5();
            },
            () =>
            {
                FinishTask(Task.TASK_5);
            }
        });

        Data.TaskPoints = new Dictionary<Task, Tuple<float, int>>();
        Data.TaskPoints.Add(Task.TASK_1, new Tuple<float, int>(0f, 0));
        Data.TaskPoints.Add(Task.TASK_2, new Tuple<float, int>(0f, 0));
        Data.TaskPoints.Add(Task.TASK_3, new Tuple<float, int>(0f, 0));
        Data.TaskPoints.Add(Task.TASK_4, new Tuple<float, int>(0f, 0));
        Data.TaskPoints.Add(Task.TASK_5, new Tuple<float, int>(0f, 0));
    }

    /// <summary>
    /// Debug method for testing created data
    /// </summary>
    [ContextMenu("CreateTestFiles")]
    void TestData()
    {
        Awake();
        SetScoreText();
    }

    /// <summary>
    /// Finishes currently started task
    /// </summary>
    /// <param name="task"></param>
    private void FinishTask(Task task)
    {
        Data.TotalPoints += Data.Points.Sum(x => x.Points);
        Data.TaskPoints[task] = new Tuple<float, int>(Data.Time, Data.Points.Sum(x => x.Points));
        Data.TotalTime += Data.Time;
        Data.Errors = Data.Errors.Concat(Data.Points.FindAll(x => x.Points < 0)).ToList();
        Data.Points.Clear();
        Data.Time = 0f;
        SaveScore();
    }

    /// <summary>
    /// Sets up test tasks for test task 1
    /// </summary>
    private void Task1()
    {
        Data.Points.Add(new TestPoint("Zad.1: Zatrzymanie robota przyciskiem STOP na pulpicie: ", 0));
        Data.Points.Add(new TestPoint("Zad.1: Zatrzymanie robota wyłącznikiem E-STOP: ", 0));
        Data.Points.Add(new TestPoint("Zad.1: Otwarcie bramki bez wcześniejszego zatrzymania robota ", 0));
        Data.Points.Add(new TestPoint("Zad.1: Otwarcie bramki: ", 0));
        Data.Points.Add(new TestPoint("Zad.1: Założenie kłódki: ", 0));
        Data.Points.Add(new TestPoint("Zad.1: Założenie czapki: ", 0));
        Data.Points.Add(new TestPoint("Zad.1: Wejście do celi: ", 0));
        Data.Points.Add(new TestPoint("Zad.1: Wejście do celi bez czapki: ", 0));
        Data.Points.Add(new TestPoint("Zad.1: Wejście do celi bez kłódki: ", 0));
    }

    /// <summary>
    /// Sets up test tasks for test task 2
    /// </summary>
    private void Task2()
    {
        Data.Points.Add(new TestPoint("Zad.2: Wyjście z celi: ", 0));
        Data.Points.Add(new TestPoint("Zad.2: Zdjęcie kłódki przed próbą zamknięcia bramki: ", 0));
        Data.Points.Add(new TestPoint("Zad.2: Wejście do celi, gdy kłódka jest zdjęta: ", 0));
        Data.Points.Add(new TestPoint("Zad.2: Próba zamknięcia bramki przed zdjęciem kłódki: ", 0));
        Data.Points.Add(new TestPoint("Zad.2: Zamknięcie bramki: ", 0));
        Data.Points.Add(new TestPoint("Zad.2: Przełączenie robota w AUTO przed próbą skasowania błędów i naciśnięciem START: ", 0));
        Data.Points.Add(new TestPoint("Zad.2: Naciśnięcie START podczas, gdy robot nie jest w AUTO: ", 0));
        Data.Points.Add(new TestPoint("Zad.2: Naciśnięcie START podczas, gdy bramka jest otwarta: ", 0));
        Data.Points.Add(new TestPoint("Zad.2: Skasowanie błędów, gdy bramka jest zamknięta i robot jest w AUTO: ", 0));
        Data.Points.Add(new TestPoint("Zad.2: Uruchomienie produkcji przyciskiem START po spełnieniu warunków: ", 0));
    }

    /// <summary>
    /// Sets up test tasks for test task 3
    /// </summary>
    private void Task3()
    {
        Data.Points.Add(new TestPoint("Zad.4: Zwrócenie uwagi na listę alarmową, gdy alarm aktywny: ", 0));
        Data.Points.Add(new TestPoint("Zad.4: Zwrócenie uwagi na kolumnę świetlną, gdy alarm aktywny: ", 0));
        Data.Points.Add(new TestPoint("Zad.4: Wyciągnięcie przycisku E-STOP na szafie robota: ", 0));
        Data.Points.Add(new TestPoint("Zad.4: Skasowanie błędów bezpieczeństwa na pulpicie: ", 0));
        Data.Points.Add(new TestPoint("Zad.4: Skasowanie pozostałych błędów na pulpicie: ", 0));
        Data.Points.Add(new TestPoint("Zad.4: Wejście do celi bez czapki: ", 0));
        Data.Points.Add(new TestPoint("Zad.4: Wejście do celi bez kłódki: ", 0));
        Data.Points.Add(new TestPoint("Zad.4: Przełączenie robota w T1: ", 0));
        Data.Points.Add(new TestPoint("Zad.4: Przełączenie robota w T2: ", 0));
        Data.Points.Add(new TestPoint("Zad.4: Zazbrojenie robota i próba poruszania bez wcześniejszego usunięcia błędu: ", 0));
        Data.Points.Add(new TestPoint("Zad.4: Zazbrojenie robota i poruszanie w dowolnym układzie po usunięciu błędu: ", 0));
    }

    /// <summary>
    /// Sets up test tasks for test task 4
    /// </summary>
    private void Task4()
    {
        Data.Points.Add(new TestPoint("Zad.5: Utworzenie nowego programu: ", 0));
        Data.Points.Add(new TestPoint("Zad.5: Dodanie punktu: ", 0));
        Data.Points.Add(new TestPoint("Zad.5: Uruchomienie programu w trybie krokowym: ", 0));
        Data.Points.Add(new TestPoint("Zad.5: Uruchomienie programu w trybie ciągłym: ", 0));
        Data.Points.Add(new TestPoint("Zad.5: Każdy z trzech punktów jest w innym miejscu: ", 0));
        Data.Points.Add(new TestPoint("Zad.5: Uruchomienie programu z mniejszą liczbą punktów niż 3: ", 0));
    }

    /// <summary>
    /// Sets up test tasks for test task 5
    /// </summary>
    private void Task5()
    {
        Data.Points.Add(new TestPoint("Zad.6: Usunięcie starych punktów w programie: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Dodanie instrukcji zamykającej chwytak TOOL ON w miejscu pobrania palety: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Dodanie instrukcji otwierającej chwytak TOOL OFF w miejscu odłożenia palety: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Brak instrukcji w programie do zamykania chwytaka: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Brak instrukcji w programie do otwierania chwytaka: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Punkt dojazdu robota nad paletę na przenośniku 1 jako JOINT: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Brak punktu dojazdu nad paletę na przenośniku 1 jako JOINT: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Punkt dojazdu robota do miejsca pobrania jako LIN: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Brak punktu dojazdu LIN: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Punkt odjazdu robota z miejsca odłożenia jako LIN: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Brak punktu odjazdu LIN: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Przejazd z jednego przenośnika nad drugi jako JOINT: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Brak JOINT z jednego przenośnika na drugi: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Prawidłowe odłożenie palety na przenośnik: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Uruchomienie programu w trybie ręcznym ciągłym: ", 0));
        Data.Points.Add(new TestPoint("Zad.6: Wykonanie jednego pełnego cyklu przekładania palety: ", 0));
    }

    /// <summary>
    /// Sets up test tasks for test task 6
    /// </summary>
    private void Task6()
    {
    }

    /// <summary>
    /// Counts time according to flag
    /// </summary>
    private void Update()
    {
        if (Pending)
        {
            Data.Time += Time.deltaTime;
            // iController.AddText(Data.Time.ToString());
        }
    }

    public TestPoint GetPointByName(string name)
    {
        return Data.Points.Find(x => x.Name.Equals(name));
    }

    /// <summary>
    /// Checks if specified task was finished
    /// </summary>
    /// <param name="task">Index of a task to check</param>
    [EnumAction(typeof(Task))]
    public void CheckTask(int task)
    {
        taskActions[(Task)task][1].Invoke();
        Pending = false;
    }

    /// <summary>
    /// Starts specified task
    /// </summary>
    /// <param name="task">Index of a task to start</param>
    [EnumAction(typeof(Task))]
    public void PrepareTask(int task)
    {
        taskActions[(Task)task][0].Invoke();
        Pending = true;
    }

    /// <summary>
    /// Sets ending text of test on frame
    /// </summary>
    public void SetScoreText()
    {
        string text = "Uzyskany wynik : " + Data.TotalPoints.ToString() + "\n";
        text += "Całkowity czas: " + Data.TotalTime.ToString() + "\n";
        text += "Popełnione błędy podczas egzaminu: \n\n";
        for (int i = 0; i < Data.Errors.Count; ++i)
        {
            text += i.ToString() + ". " + Data.Errors[i].Name + (Convert.ToBoolean(Data.Errors[i].Points)).ToString() + "\n";
        }

        iController.SetText(text, 21f);

        SaveScore();
        SaveScorePDF(text);
    }

    /// <summary>
    /// Saves PDF with scores
    /// </summary>
    /// <param name="text">Text to insert into PDF</param>
    private void SaveScorePDF(string text)
    {
        PdfDocument document = new PdfDocument();
        document.Info.Title = "Scores for " + DateTime.Now;
        // Create an empty page
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        // Create a font
        XFont font = new XFont("Verdana", 20, XFontStyle.Regular);
        // Draw the text
        gfx.DrawString(text, font, XBrushes.Black,
        new XRect(0, 0, page.Width, page.Height),
        XStringFormats.Center);
        // Save the document...
        string filename = Application.dataPath + "/PDF/Scores" + DateTime.Now.ToString().Replace(':', '-') + ".pdf";
        document.Save(filename);
    }

    /// <summary>
    /// Saves CSV file with scores
    /// </summary>
    private void SaveScore()
    {
        CSVUtility.SaveCSV(Application.dataPath + "/CSV/Scores.csv", Data);
    }
}
