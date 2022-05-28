using CourseWork.Database;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CourseWork
{
    public partial class ModWindow : Window
    {
        #region constants
        private static readonly string[][] hints =
        {
            new string[] { "ID Студента", "Группа студента", "Кафедра студента", "Фамилия студента", "Имя студента", "Отчество студента" },
            new string[] { "ID Группы", "Кафедра группы", "Шифр Группы", "Количество студентов" },
            new string[] { "ID Оценки", "Студент", "Предмет", "Оценка", "Количество пропусков" },
            new string[] { "ID Кафедры", "Наименование кафедры", "Заведующий кафедрой", "Аудитория кафедры" },
            new string[] { "ID Предмета", "Наименование предмета" },
            new string[] { "ID Оценки", "Наименование предмета", "Фамилия студента", "Имя студента", "Отчество студента", "Шифр группы", "Оценка", "Количество пропусков"}
        };

        private static readonly string[][] tooltips =
        {
            new string[] { "ID Студента генерируется автоматически", "Выберите группу студента из списка",
                           "Выберите кафедру студента из списка", "Введите фамилию студента",
                           "Введите имя студента", "Введите отчество студента" },
            new string[] { "ID Группы генерируется автоматически", "Выберите кафедру группы из списка",
                           "Введите шифр группы",
                           "Невозможно явно указывать количество студентов. Поле обновляется автоматически."},
            new string[] { "ID Оценки генерируется автоматически", "Выберите студента из списка",
                           "Выберите предмет из списка", "Введите оценку", "Введите количество пропусков"},
            new string[] { "ID Кафедры генерируется автоматически", "Введите наименование кафедры",
                           "Введите ФИО заведующего кафедрой", "Введите аудиторию кафедры"},
            new string[] { "ID Предмета генерируется автоматически", "Введите наименование предмета"},
            new string[] { "ID Оценки генерируется автоматически", "Введите наименование предмета",
                           "Введите фамилию студента", "Введите имя студента", "Введите отчество студента",
                           "Введите шифр группы", "Введите оценку", "Введите количество пропусков" }
        };

        private static readonly string[] inputs =
        {
            "tbLeftTop", "cbLeftMiddle", "cbLeftBottom", "tbLeftBottom2",
            "tbRightTop", "tbRightMiddle", "tbRightBottom", "tbRightBottom2",
            "tbLeftMiddle", "tbLeftBottom"
        };

        private static readonly string[][] collapes =
        {
            new string[] { },
            new string[] { "tbRightBottom", "cbLeftBottom" },
            new string[] { "tbRightBottom" },
            new string[] { "cbLeftMiddle", "cbLeftBottom" },
            new string[] { "tbRightMiddle", "tbRightBottom", "cbLeftMiddle", "cbLeftBottom" },
            new string[] { "cbLeftMiddle", "cbLeftBottom" }
        };
        #endregion

        #region fields
        private DataTypes DataType { get; init; }
        private bool IsUpdating { get; init; } = false;
        private int UpdatingId { get; init; }
        private DbWorker Worker { get; init; } = default!;
        private List<int> dbId1 = new List<int>();
        private List<int> dbId2 = new List<int>();
        #endregion

        #region ModWindow constructors
        private ModWindow()
        {
            InitializeComponent();
            ToolTipService.SetShowOnDisabled(tbLeftTop, true);
            ToolTipService.SetShowOnDisabled(tbRightMiddle, true);
        }

        public ModWindow(DataTypes dataType, DbWorker worker)
            : this()
        {
            DataType = dataType;
            windowTitle.Text = $"{windowTitle.Text} {dataType}";
            Worker = worker;
            setInputs();
        }

        public ModWindow(DataTypes dataType, DbWorker worker, int updatingId)
            : this()
        {
            DataType = dataType;
            windowTitle.Text = $"Изменение {dataType}";
            IsUpdating = true;
            UpdatingId = updatingId;
            Worker = worker;
            setInputs();
        }
        #endregion

        #region Configuring form inputs
        private void setInputs()
        {
            switch (DataType)
            {
                case DataTypes.Students:
                    setCollapsed();
                    setHintsAndTooltips();

                    tbRightTop.MaxLength = 20;
                    tbRightMiddle.MaxLength = 20;
                    tbRightBottom.MaxLength = 20;
                    tbRightTop.PreviewTextInput += TextValidationTextBox;
                    tbRightMiddle.PreviewTextInput += TextValidationTextBox;
                    tbRightBottom.PreviewTextInput += TextValidationTextBox;
                    DataSet groups = Worker.makeQuery("SELECT id, cypher " +
                            "FROM groups ORDER BY id;");
                    cbLeftMiddle.Items.Clear();
                    foreach (DataRow row in groups.Tables[0].Rows)
                    {
                        dbId1.Add(Convert.ToInt32(row[0].ToString()));
                        cbLeftMiddle.Items.Add($"{row[1]}");
                    }

                    DataSet deparments = Worker.makeQuery("SELECT id, title " +
                        "FROM departments ORDER BY id;");
                    cbLeftBottom.Items.Clear();
                    foreach (DataRow row in deparments.Tables[0].Rows)
                    {
                        dbId2.Add(Convert.ToInt32(row[0].ToString()));
                        cbLeftBottom.Items.Add($"{row[1]}");
                    }

                    if (!IsUpdating)
                    {
                        DataSet student = Worker.makeQuery("SELECT last_value " +
                            "FROM students_id_seq;");
                        int st_id = Convert.ToInt32(student.Tables[0].Rows[0][0].ToString());
                        tbLeftTop.Text = (st_id + 1).ToString();
                    } else
                    {
                        DataSet student = Worker.makeQuery($"SELECT * FROM students WHERE id={UpdatingId};");
                        var st = student.Tables[0].Rows[0];
                        tbLeftTop.Text = UpdatingId.ToString();
                        tbRightTop.Text = st[1].ToString();
                        tbRightMiddle.Text = st[2].ToString();
                        tbRightBottom.Text = st[3].ToString();
                        cbLeftBottom.SelectedIndex = Convert.ToInt32(st[4]) - 1;
                        cbLeftMiddle.SelectedIndex = Convert.ToInt32(st[5]) - 1;
                    }

                    break;
                case DataTypes.Groups:
                    setCollapsed();
                    setHintsAndTooltips();

                    tbRightTop.MaxLength = 10;
                    tbRightMiddle.IsEnabled = false;
                    tbRightMiddle.Text = "0";
                    cbLeftMiddle.PreviewTextInput += TextValidationTextBox;

                    deparments = Worker.makeQuery("SELECT id, title " +
                        "FROM departments ORDER BY id;");
                    cbLeftMiddle.Items.Clear();
                    foreach (DataRow row in deparments.Tables[0].Rows)
                    {
                        cbLeftMiddle.Items.Add($"{row[1]}");
                        dbId1.Add(Convert.ToInt32(row[0].ToString()));
                    }

                    if (!IsUpdating)
                    {
                        DataSet group = Worker.makeQuery("SELECT last_value FROM groups_id_seq;");
                        int gr_id = Convert.ToInt32(group.Tables[0].Rows[0][0].ToString());
                        tbLeftTop.Text = (gr_id + 1).ToString();
                    }
                    else {
                        DataSet group = Worker.makeQuery($"SELECT * FROM groups WHERE id={UpdatingId};");
                        var gr = group.Tables[0].Rows[0];
                        tbLeftTop.Text = UpdatingId.ToString();
                        tbRightTop.Text = gr[1].ToString();
                        tbRightMiddle.Text = gr[2].ToString();
                        cbLeftMiddle.SelectedIndex = Convert.ToInt32(gr[3]) - 1;
                    }

                    Height = 250;
                    break;
                case DataTypes.Marks:
                    setCollapsed();
                    setHintsAndTooltips();

                    tbRightTop.MaxLength = 1;
                    tbRightTop.PreviewTextInput += MarkValidationTextBox;
                    tbRightMiddle.PreviewTextInput += NumberValidationTextBox;

                    DataSet students = Worker.makeQuery("SELECT id, surname, name, " +
                        "middle_name FROM students ORDER BY id;");
                    cbLeftMiddle.Items.Clear();
                    foreach (DataRow row in students.Tables[0].Rows)
                    {
                        cbLeftMiddle.Items.Add($"{row[1]} {row[2]} {row[3]}");
                        dbId1.Add(Convert.ToInt32(row[0].ToString()));
                    }

                    DataSet subjects = Worker.makeQuery("SELECT * FROM subjects ORDER BY id;");
                    cbLeftBottom.Items.Clear();
                    foreach (DataRow row in subjects.Tables[0].Rows)
                    {
                        cbLeftBottom.Items.Add($"{row[1]}");
                        dbId2.Add(Convert.ToInt32(row[0].ToString()));
                    }

                    if (!IsUpdating)
                    {
                        DataSet mark = Worker.makeQuery("SELECT last_value FROM marks_id_seq;");
                        int mr_id = Convert.ToInt32(mark.Tables[0].Rows[0][0].ToString());
                        tbLeftTop.Text = (mr_id + 1).ToString();
                    }
                    else
                    {
                        DataSet mark = Worker.makeQuery($"SELECT * FROM marks WHERE id={UpdatingId};");
                        var mr = mark.Tables[0].Rows[0];
                        tbLeftTop.Text = UpdatingId.ToString();
                        tbRightTop.Text = mr[1].ToString();
                        tbRightMiddle.Text = mr[2].ToString();
                        cbLeftMiddle.SelectedIndex = Convert.ToInt32(mr[3]) - 1;
                        cbLeftBottom.SelectedIndex = Convert.ToInt32(mr[4]) - 1;
                    }

                    break;
                case DataTypes.Departments:
                    setCollapsed();
                    setHintsAndTooltips();

                    tbRightTop.MaxLength = 100;
                    tbRightMiddle.MaxLength = 50;
                    tbRightBottom.MaxLength = 6;
                    tbRightMiddle.PreviewTextInput += TextValidationTextBox;

                    if (!IsUpdating)
                    {
                        DataSet department = Worker.makeQuery("SELECT " +
                            "last_value FROM departments_id_seq;");
                        int dp_id = Convert.ToInt32(department.Tables[0].Rows[0][0].ToString());
                        tbLeftTop.Text = (dp_id + 1).ToString();
                    } else
                    {
                        DataSet department = Worker.makeQuery($"SELECT * FROM departments WHERE id={UpdatingId};");
                        var dp = department.Tables[0].Rows[0];
                        tbLeftTop.Text = UpdatingId.ToString();
                        tbRightTop.Text = dp[1].ToString();
                        tbRightMiddle.Text = dp[2].ToString();
                        tbRightBottom.Text = dp[3].ToString();
                    }
                    break;
                case DataTypes.Subjects:
                    setCollapsed();
                    setHintsAndTooltips();

                    tbRightTop.MaxLength = 30;
                    tbRightTop.PreviewTextInput += TextValidationTextBox;

                    if (!IsUpdating)
                    {
                        DataSet subject = Worker.makeQuery("SELECT " +
                            "last_value FROM subjects_id_seq;");
                        int sj_id = Convert.ToInt32(subject.Tables[0].Rows[0][0].ToString());
                        tbLeftTop.Text = (sj_id + 1).ToString();
                    } else
                    {
                        DataSet subject = Worker.makeQuery($"SELECT * FROM subjects WHERE id={UpdatingId};");
                        var sj = subject.Tables[0].Rows[0];
                        tbLeftTop.Text = UpdatingId.ToString();
                        tbRightTop.Text = sj[1].ToString();
                    }

                    Height = 230;
                    break;
                case DataTypes.Marks_View:
                    grid.RowDefinitions.Add(new RowDefinition());
                    cbLeftMiddle.Visibility = Visibility.Collapsed;
                    cbLeftBottom.Visibility = Visibility.Collapsed;
                    tbLeftMiddle.Visibility = Visibility.Visible;
                    tbLeftBottom.Visibility = Visibility.Visible;
                    tbLeftBottom2.Visibility = Visibility.Visible;
                    tbRightBottom2.Visibility = Visibility.Visible;
                    setHintsAndTooltips();
                    tbLeftMiddle.MaxLength = 1;
                    tbLeftMiddle.PreviewTextInput += MarkValidationTextBox;
                    tbLeftBottom.PreviewTextInput += NumberValidationTextBox;
                    tbLeftBottom2.MaxLength = 30;
                    tbLeftBottom2.PreviewTextInput += TextValidationTextBox;
                    tbRightTop.MaxLength = 20;
                    tbRightTop.PreviewTextInput += TextValidationTextBox;
                    tbRightMiddle.MaxLength = 20;
                    tbRightMiddle.PreviewTextInput += TextValidationTextBox;
                    tbRightBottom.MaxLength = 20;
                    tbRightBottom.PreviewTextInput += TextValidationTextBox;
                    tbRightBottom2.MaxLength = 10;

                    DataSet mview = Worker.makeQuery($"SELECT mark_id, mark, passes, " +
                        $"subj_title, surname, name, middle_name, group_name FROM marks_view " +
                        $"WHERE mark_id={UpdatingId};");
                    var mv = mview.Tables[0].Rows[0];
                    tbLeftTop.Text = UpdatingId.ToString();
                    tbLeftMiddle.Text = mv[1].ToString();
                    tbLeftBottom.Text = mv[2].ToString();
                    tbLeftBottom2.Text = mv[3].ToString();
                    tbRightTop.Text = mv[4].ToString();
                    tbRightMiddle.Text = mv[5].ToString();
                    tbRightBottom.Text = mv[6].ToString();
                    tbRightBottom2.Text = mv[7].ToString();
                    
                    Height += 50;
                    break;
            }
        }

        private void setHintsAndTooltips()
        {
            int i = 0;
            foreach (string input in inputs)
            {
                var inp = grid.FindName(input) as Control;
                if (inp != null && inp.Visibility != Visibility.Collapsed)
                {
                    inp.ToolTip = tooltips[(int)DataType][i];
                    HintAssist.SetHint(inp, hints[(int)DataType][i++]);
                }
            }
        }

        private void setCollapsed()
        {
            foreach (string input in collapes[(int)DataType])
            {
                var inp = grid.FindName(input) as Control;
                if (inp != null)
                    inp.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        private void makeChanges(object sender, RoutedEventArgs e)
        {
            switch (DataType)
            {
                case DataTypes.Students:
                    var surname = tbRightTop.Text;
                    var name = tbRightMiddle.Text;
                    var midname = tbRightBottom.Text;
                    var group_id = cbLeftMiddle.SelectedIndex == -1 ?
                        0 : dbId1[cbLeftMiddle.SelectedIndex];
                    var department_id = cbLeftBottom.SelectedIndex == -1 ?
                        0 : dbId2[cbLeftBottom.SelectedIndex];

                    if (string.IsNullOrWhiteSpace(surname)
                        || string.IsNullOrWhiteSpace(name)
                        || string.IsNullOrWhiteSpace(midname)
                        || group_id == 0
                        || department_id == 0)
                    {
                        responseText.Text = "Заполните все поля!";
                        return;
                    }

                    try
                    {
                        if (!IsUpdating)
                        {
                            var response = Worker.makeQuery($"SELECT add_student('{surname}', " +
                                $"'{name}', '{midname}', {department_id}, {group_id})");
                            responseText.Text = response.Tables[0].Rows[0][0].ToString();
                        } else
                        {
                            var response = Worker.makeQuery($"SELECT update_student('{UpdatingId}', " +
                                $"'{surname}', '{name}', '{midname}', {department_id}, {group_id})");
                            responseText.Text = response.Tables[0].Rows[0][0].ToString();
                            StartCloseTimer();
                            btn_confirm.IsEnabled = false;
                            break;
                        }
                    } catch (Npgsql.PostgresException exception)
                    {
                        responseText.Text = exception.Message;
                        break;
                    }
                    tbLeftTop.Text = (Convert.ToInt32(tbLeftTop.Text) + 1).ToString();
                    cbLeftMiddle.SelectedIndex = -1;
                    cbLeftBottom.SelectedIndex = -1;
                    tbRightTop.Clear();
                    tbRightMiddle.Clear();
                    tbRightBottom.Clear();
                    break;
                case DataTypes.Groups:
                    var cypher = tbRightTop.Text;
                    department_id = cbLeftMiddle.SelectedIndex == -1 ?
                        0 : dbId1[cbLeftMiddle.SelectedIndex];

                    if (string.IsNullOrWhiteSpace(cypher)
                        || department_id == 0)
                    {
                        responseText.Text = "Заполните все поля!";
                        return;
                    }

                    try
                    {
                        if (!IsUpdating)
                        {
                            var response = Worker.makeQuery($"SELECT add_group('{cypher}', " +
                                $"'{department_id}');");
                            responseText.Text = response.Tables[0].Rows[0][0].ToString();
                        } else
                        {
                            var response = Worker.makeQuery($"SELECT update_group('{UpdatingId}', " +
                                $"'{cypher}', '{department_id}');");
                            responseText.Text = response.Tables[0].Rows[0][0].ToString();
                            StartCloseTimer();
                            btn_confirm.IsEnabled = false;
                            break;
                        }
                    } catch (Npgsql.PostgresException exception)
                    {
                        responseText.Text = exception.Message;
                        break;
                    }

                    tbLeftTop.Text = (Convert.ToInt32(tbLeftTop.Text) + 1).ToString();
                    cbLeftMiddle.SelectedIndex = -1;
                    tbRightTop.Clear();
                    break;
                case DataTypes.Marks:
                    var mark = tbRightTop.Text;
                    var passes = tbRightMiddle.Text;
                    var student_id = cbLeftMiddle.SelectedIndex == -1 ?
                        0 : dbId1[cbLeftMiddle.SelectedIndex];
                    var subject_id = cbLeftBottom.SelectedIndex == -1 ?
                        0 : dbId2[cbLeftBottom.SelectedIndex];

                    if (string.IsNullOrWhiteSpace(mark)
                        || string.IsNullOrWhiteSpace(passes)
                        || student_id == 0
                        || subject_id == 0)
                    {
                        responseText.Text = "Заполните все поля!";
                        return;
                    }

                    try
                    {
                        if (!IsUpdating)
                        {
                            var response = Worker.makeQuery($"SELECT add_mark('{mark}', " +
                                $"'{passes}', '{student_id}', '{subject_id}');");
                            responseText.Text = response.Tables[0].Rows[0][0].ToString();
                        } else
                        {
                            var response = Worker.makeQuery($"SELECT update_mark('{UpdatingId}', " +
                                $"'{mark}', '{passes}', '{student_id}', '{subject_id}');");
                            responseText.Text = response.Tables[0].Rows[0][0].ToString();
                            StartCloseTimer();
                            btn_confirm.IsEnabled = false;
                            break;
                        }
                    } catch (Npgsql.PostgresException exception)
                    {
                        responseText.Text = exception.MessageText;
                        break;
                    }

                    tbLeftTop.Text = (Convert.ToInt32(tbLeftTop.Text) + 1).ToString();
                    cbLeftMiddle.SelectedIndex = -1;
                    cbLeftBottom.SelectedIndex = -1;
                    tbRightTop.Clear();
                    tbRightMiddle.Clear();
                    break;
                case DataTypes.Departments:
                    var title = tbRightTop.Text;
                    var head = tbRightMiddle.Text;
                    var classroom = tbRightBottom.Text;

                    if (string.IsNullOrWhiteSpace(title)
                        || string.IsNullOrWhiteSpace(head)
                        || string.IsNullOrWhiteSpace(classroom))
                    {
                        responseText.Text = "Заполните все поля!";
                        return;
                    }

                    try
                    {
                        if (!IsUpdating)
                        {
                            var response = Worker.makeQuery($"SELECT add_department('{title}', " +
                                $"'{head}', '{classroom}');");
                            responseText.Text = response.Tables[0].Rows[0][0].ToString();
                        } else
                        {
                            var response = Worker.makeQuery($"SELECT update_department('{UpdatingId}', " +
                                $"'{title}', '{head}', '{classroom}');");
                            responseText.Text = response.Tables[0].Rows[0][0].ToString();
                            StartCloseTimer();
                            btn_confirm.IsEnabled = false;
                            break;
                        }
                    } catch (Npgsql.PostgresException exception)
                    {
                        responseText.Text = exception.Message;
                        break;
                    }

                    tbLeftTop.Text = (Convert.ToInt32(tbLeftTop.Text) + 1).ToString();
                    tbRightTop.Clear();
                    tbRightMiddle.Clear();
                    tbRightBottom.Clear();
                    break;
                case DataTypes.Subjects:
                    var sj_title = tbRightTop.Text;
                    if (string.IsNullOrWhiteSpace(sj_title))
                    {
                        responseText.Text = "Заполните все поля!";
                        return;
                    }

                    try
                    {
                        if (!IsUpdating)
                        {
                            var response = Worker.makeQuery($"SELECT add_subject('{sj_title}');");
                            responseText.Text = response.Tables[0].Rows[0][0].ToString();
                        } else
                        {
                            var response = Worker.makeQuery($"SELECT update_subject('{UpdatingId}', '{sj_title}');");
                            responseText.Text = response.Tables[0].Rows[0][0].ToString();
                            StartCloseTimer();
                            btn_confirm.IsEnabled = false;
                            break;
                        }
                    } catch (Npgsql.PostgresException exception)
                    {
                        responseText.Text = exception.Message;
                        break;
                    }

                    tbLeftTop.Text = (Convert.ToInt32(tbLeftTop.Text) + 1).ToString();
                    tbRightTop.Clear();
                    break;
                case DataTypes.Marks_View:
                    mark = tbLeftMiddle.Text;
                    passes = tbLeftBottom.Text;
                    var subj_title = tbLeftBottom2.Text;
                    surname = tbRightTop.Text;
                    name = tbRightMiddle.Text;
                    midname = tbRightBottom.Text;
                    cypher = tbRightBottom2.Text;

                    try
                    {
                        var response = Worker.makeQuery($"SELECT update_marks_view('{UpdatingId}', " +
                            $"'{cypher}', '{surname}', '{name}', '{midname}', '{subj_title}', '{mark}', '{passes}');");
                        responseText.Text = response.Tables[0].Rows[0][0].ToString();
                        StartCloseTimer();
                        btn_confirm.IsEnabled = false;
                        break;
                    } catch (Npgsql.PostgresException exception)
                    {
                        responseText.Text = exception.Message;
                        break;
                    }
            }
        }

        #region utils
        private void closeWindow(object sender, RoutedEventArgs e) =>
            Close();

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
        
        private void MarkValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[2-5]$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void TextValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^[a-zA-Zа-яА-Я\s] *$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        private void StartCloseTimer()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3d);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            Close();
        }
        #endregion
    }
}
