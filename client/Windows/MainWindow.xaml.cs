using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CourseWork.Database;
using MaterialDesignThemes.Wpf;

namespace CourseWork
{
    public partial class MainWindow : Window
    {
        #region fields
        public bool IsDarkTheme { get; set; }
        private readonly PaletteHelper paletteHelper = new PaletteHelper();
        private DbWorker worker { get; init; }
        private static readonly string[] btns =
        {
            "btn_ins_st", "btn_upd_st", "btn_del_st",
            "btn_ins_gr", "btn_upd_gr", "btn_del_gr",
            "btn_ins_mr", "btn_upd_mr", "btn_del_mr",
            "btn_ins_sj", "btn_upd_sj", "btn_del_sj",
            "btn_ins_dp", "btn_upd_dp", "btn_del_dp",
            "btn_upd_mv"
        };
        #endregion

        public MainWindow(string login, string password)
        {
            InitializeComponent();

            worker = new DbWorker(login, password);
            usernameText.Text = login.ToUpper();

            if (login.ToLower() == "user")
            {
                tab.Items.RemoveAt(6);
                foreach (string btn in btns)
                {
                    var button = body.FindName(btn) as Control;
                    if (button != null)
                    {
                        button.IsEnabled = false;
                        button.ToolTip = "Действие невозможно для пользователя " + login;
                        ToolTipService.SetShowOnDisabled(button, true);
                    }
                }
            }

            update_all_db();
        }

        #region DataGrid Work
        private void query_insert(object sender, RoutedEventArgs e)
        {
            var name = ((Button)sender).Name;
            ModWindow mw;
            if (Application.Current.Windows.OfType<ModWindow>().Any())
                Application.Current.Windows.OfType<ModWindow>().First().Close();
            switch (name)
            {    
                case "btn_ins_st":
                    mw = new ModWindow(DataTypes.Students, worker);
                    mw.Show();
                    break;
                case "btn_ins_gr":
                    mw = new ModWindow(DataTypes.Groups, worker);
                    mw.Show();
                    break;
                case "btn_ins_mr":
                    mw = new ModWindow(DataTypes.Marks, worker);
                    mw.Show();
                    break;
                case "btn_ins_sj":
                    mw = new ModWindow(DataTypes.Subjects, worker);
                    mw.Show();
                    break;
                case "btn_ins_dp":
                    mw = new ModWindow(DataTypes.Departments, worker);
                    mw.Show();
                    break;
            }
        }

        private void query_update(object sender, RoutedEventArgs e)
        {
            var name = ((Button)sender).Name;
            ModWindow mw;
            if (Application.Current.Windows.OfType<ModWindow>().Any())
                Application.Current.Windows.OfType<ModWindow>().First().Close();
            switch (name)
            {
                case "btn_upd_st":
                    var selectedItems = DataGrid_st.SelectedItems;
                    if (selectedItems.Count > 1)
                    {
                        responseText_st.Text = "Невозможно редактировать более 1 строки.";
                        break;
                    }
                    if (selectedItems.Count == 0)
                    {
                        responseText_st.Text = "Выберите строку для редактирования.";
                        break;
                    }
                    var index = Convert.ToInt32((selectedItems[0] as DataRowView)[0]);
                    mw = new ModWindow(DataTypes.Students, worker, index);
                    mw.Show();
                    break;
                case "btn_upd_gr":
                    selectedItems = DataGrid_gr.SelectedItems;
                    if (selectedItems.Count > 1)
                    {
                        responseText_gr.Text = "Невозможно редактировать более 1 строки.";
                        break;
                    }
                    if (selectedItems.Count == 0)
                    {
                        responseText_gr.Text = "Выберите строку для редактирования.";
                        break;
                    }
                    index = Convert.ToInt32((selectedItems[0] as DataRowView)[0]);
                    mw = new ModWindow(DataTypes.Groups, worker, index);
                    mw.Show();
                    break;
                case "btn_upd_mr":
                    selectedItems = DataGrid_mr.SelectedItems;
                    if (selectedItems.Count > 1)
                    {
                        responseText_mr.Text = "Невозможно редактировать более 1 строки.";
                        break;
                    }
                    if (selectedItems.Count == 0)
                    {
                        responseText_mr.Text = "Выберите строку для редактирования.";
                        break;
                    }
                    index = Convert.ToInt32((selectedItems[0] as DataRowView)[0]);
                    mw = new ModWindow(DataTypes.Marks, worker, index);
                    mw.Show();
                    break;
                case "btn_upd_sj":
                    selectedItems = DataGrid_sj.SelectedItems;
                    if (selectedItems.Count > 1)
                    {
                        responseText_sj.Text = "Невозможно редактировать более 1 строки.";
                        break;
                    }
                    if (selectedItems.Count == 0)
                    {
                        responseText_sj.Text = "Выберите строку для редактирования.";
                        break;
                    }
                    index = Convert.ToInt32((selectedItems[0] as DataRowView)[0]);
                    mw = new ModWindow(DataTypes.Subjects, worker, index);
                    mw.Show();
                    break;
                case "btn_upd_dp":
                    selectedItems = DataGrid_dp.SelectedItems;
                    if (selectedItems.Count > 1)
                    {
                        responseText_dp.Text = "Невозможно редактировать более 1 строки.";
                        break;
                    }
                    if (selectedItems.Count == 0)
                    {
                        responseText_dp.Text = "Выберите строку для редактирования.";
                        break;
                    }
                    index = Convert.ToInt32((selectedItems[0] as DataRowView)[0]);
                    mw = new ModWindow(DataTypes.Departments, worker, index);
                    mw.Show();
                    break;
                case "btn_upd_mv":
                    selectedItems = DataGrid_mv.SelectedItems;
                    if (selectedItems.Count > 1)
                    {
                        responseText_mv.Text = "Невозможно редактировать более 1 строки.";
                        break;
                    }
                    if (selectedItems.Count == 0)
                    {
                        responseText_mv.Text = "Выберите строку для редактирования.";
                        break;
                    }
                    index = Convert.ToInt32((selectedItems[0] as DataRowView)[0]);
                    mw = new ModWindow(DataTypes.Marks_View, worker, index);
                    mw.Show();
                    break;
            }
        }

        private void query_delete(object sender, RoutedEventArgs e)
        {
            var name = ((Button)sender).Name;
            switch(name)
            {
                case "btn_del_st":
                    var selectedItems = DataGrid_st.SelectedItems;
                    if (selectedItems.Count == 0)
                    {
                        responseText_st.Text = "Выберите строки для удаления.";
                        break;
                    }
                    if (!confirmDelete()) break;
                    foreach (var item in selectedItems)
                    {
                        var id = Convert.ToInt32((item as DataRowView)[0]);
                        try
                        {
                            var response = worker.makeQuery($"SELECT delete_student({id});");
                            responseText_st.Text = response.Tables[0].Rows[0][0].ToString();
                        }
                        catch (Npgsql.PostgresException exception)
                        {
                            responseText_st.Text = exception.Message;
                            break;
                        }
                    }
                    update_all_db();
                    break;
                case "btn_del_gr":
                    selectedItems = DataGrid_gr.SelectedItems;
                    if (selectedItems.Count == 0)
                    {
                        responseText_gr.Text = "Выберите строки для удаления.";
                        break;
                    }
                    if (!confirmDelete()) break;
                    foreach (var item in selectedItems)
                    {
                        var id = Convert.ToInt32((item as DataRowView)[0]);
                        try
                        {
                            var response = worker.makeQuery($"SELECT delete_group({id});");
                            responseText_gr.Text = response.Tables[0].Rows[0][0].ToString();
                        }
                        catch (Npgsql.PostgresException exception)
                        {
                            responseText_gr.Text = exception.Message;
                            break;
                        }
                    }
                    update_all_db();
                    break;
                case "btn_del_mr":
                    selectedItems = DataGrid_mr.SelectedItems;
                    if (selectedItems.Count == 0)
                    {
                        responseText_mr.Text = "Выберите строки для удаления.";
                        break;
                    }
                    if (!confirmDelete()) break;
                    foreach (var item in selectedItems)
                    {
                        var id = Convert.ToInt32((item as DataRowView)[0]);
                        try
                        {
                            var response = worker.makeQuery($"SELECT delete_mark({id});");
                            responseText_mr.Text = response.Tables[0].Rows[0][0].ToString();
                        }
                        catch (Npgsql.PostgresException exception)
                        {
                            responseText_mr.Text = exception.Message;
                            break;
                        }
                    }
                    update_all_db();
                    break;
                case "btn_del_sj":
                    selectedItems = DataGrid_sj.SelectedItems;
                    if (selectedItems.Count == 0)
                    {
                        responseText_sj.Text = "Выберите строки для удаления.";
                        break;
                    }
                    if (!confirmDelete()) break;
                    foreach (var item in selectedItems)
                    {
                        var id = Convert.ToInt32((item as DataRowView)[0]);
                        try
                        {
                            var response = worker.makeQuery($"SELECT delete_subject({id});");
                            responseText_sj.Text = response.Tables[0].Rows[0][0].ToString();
                        }
                        catch (Npgsql.PostgresException exception)
                        {
                            responseText_sj.Text = exception.Message;
                            break;
                        }
                    }
                    update_all_db();
                    break;
                case "btn_del_dp":
                    selectedItems = DataGrid_dp.SelectedItems;
                    if (selectedItems.Count == 0)
                    {
                        responseText_dp.Text = "Выберите строки для удаления.";
                        break;
                    }
                    if (!confirmDelete()) break;
                    foreach (var item in selectedItems)
                    {
                        var id = Convert.ToInt32((item as DataRowView)[0]);
                        try
                        {
                            var response = worker.makeQuery($"SELECT delete_department({id});");
                            responseText_dp.Text = response.Tables[0].Rows[0][0].ToString();
                        }
                        catch (Npgsql.PostgresException exception)
                        {
                            responseText_dp.Text = exception.Message;
                            break;
                        }
                    }
                    update_all_db();
                    break;
            }
        }

        private void make_task(object sender, RoutedEventArgs e)
        {
            switch(task_combobox.SelectedIndex)
            {
                case 0:
                    var dataset = worker.makeQuery(
                        "SELECT marks.id AS mark_id, " +
                        "s.surname, " +
                        "s.name, " +
                        "s.middle_name, " +
                        "sj.title AS subject, " +
                        "CASE " +
                            "WHEN mark = 5 THEN 'Отлично' " +
                            "WHEN mark = 4 THEN 'Хорошо' " +
                            "WHEN mark = 3 THEN 'Удовлетворительно' " +
                            "WHEN mark = 2 THEN 'Пересдача' END AS mark " +
                        "FROM marks " +
                            "JOIN students s ON marks.student_id = s.id " +
                            "JOIN subjects sj ON marks.subject_id = sj.id " +
                        "ORDER BY surname;");
                    if (dataset != null)
                        DataGrid_tasks.ItemsSource = dataset.Tables[0].DefaultView;
                    break;
                case 1:
                    MessageBox.Show("Выполнение задания 3b - во вкладке MVIEW", "Внимание");
                    break;
                case 2:
                    dataset = worker.makeQuery(
                        "SELECT (SELECT id FROM students WHERE surname = 'Иванов' LIMIT 1);"
                    );
                    if (dataset != null)
                        DataGrid_tasks.ItemsSource = dataset.Tables[0].DefaultView;
                    break;
                case 3:
                    dataset = worker.makeQuery(
                        "SELECT * FROM (SELECT surname, name, group_id FROM students) AS m " +
                        "WHERE m.group_id = 3; ");
                    if (dataset != null)
                        DataGrid_tasks.ItemsSource = dataset.Tables[0].DefaultView;
                    break;
                case 4:
                    dataset = worker.makeQuery(
                        "SELECT count(mark) AS resits FROM marks " +
                        "WHERE mark = (SELECT MIN(mark) FROM marks); "
                    );
                    if (dataset != null)
                        DataGrid_tasks.ItemsSource = dataset.Tables[0].DefaultView;
                    break;
                case 5:
                    dataset = worker.makeQuery(
                        "SELECT surname, name, " +
                            "(SELECT cypher FROM \"groups\" WHERE id = group_id) AS \"group\" " +
                        "FROM students;"
                    );
                    if (dataset != null)
                        DataGrid_tasks.ItemsSource = dataset.Tables[0].DefaultView;
                    break;
                case 6:
                    dataset = worker.makeQuery(
                        "SELECT * FROM \"groups\" g WHERE NOT " +
                            "exists(SELECT * FROM students s WHERE g.id = s.group_id) " +
                            "ORDER BY id;"
                    );
                    if (dataset != null)
                        DataGrid_tasks.ItemsSource = dataset.Tables[0].DefaultView;
                    break;
                case 7:
                    dataset = worker.makeQuery(
                        "SELECT id, mark, passes, " +
                           "(SELECT surname FROM students WHERE student_id = students.id), " +
                           "(SELECT title FROM subjects WHERE subject_id = subjects.id) " +
                        "FROM marks; "
                    );
                    if (dataset != null)
                        DataGrid_tasks.ItemsSource = dataset.Tables[0].DefaultView;
                    break;
                case 8:
                    dataset = worker.makeQuery(
                        "SELECT surname, name, MIN(mark) as min_mark " +
                        "FROM marks JOIN students s on marks.student_id = s.id " +
                        "GROUP BY surname, name " +
                        "HAVING MIN(mark) = 3;"
                    );
                    if (dataset != null)
                        DataGrid_tasks.ItemsSource = dataset.Tables[0].DefaultView;
                    break;
                case 9:
                    dataset = worker.makeQuery(
                        "SELECT * FROM marks_view WHERE group_name = ANY('{БИСО-01-20,БИСО-02-20}');"
                    );
                    if (dataset != null)
                        DataGrid_tasks.ItemsSource = dataset.Tables[0].DefaultView;
                    break;
                case 10:
                    dataset = worker.makeQuery(
                        "SELECT cypher, student_count " +
                        "FROM \"groups\" " +
                        "WHERE student_count > ALL(SELECT AVG(student_count) FROM \"groups\");"
                    );
                    if (dataset != null)
                        DataGrid_tasks.ItemsSource = dataset.Tables[0].DefaultView;
                    break;
            }
        }

        private void reload_db(object sender, RoutedEventArgs e) =>
            update_all_db();

        private void update_all_db()
        {
            var dataset = worker.makeQuery(
                "SELECT students.id \"ID\", " +
                       "surname     \"Фамилия\", " +
                       "name        \"Имя\", " +
                       "middle_name \"Отчество\", " +
                       "d.title  AS \"Кафедра\", " +
                       "g.cypher AS \"Группа\" " +
                "FROM students " +
                         "JOIN departments d on students.department_id = d.id " +
                         "JOIN groups g on students.group_id = g.id " +
                "ORDER BY students.id; ");
            if (dataset != null)
                DataGrid_st.ItemsSource = dataset.Tables[0].DefaultView;

            dataset = worker.makeQuery(
                "SELECT groups.id \"ID\", cypher \"Шифр\", student_count \"Количество студентов\", d.title \"Кафедра\" " +
                "FROM groups " +
                         "JOIN departments d on groups.department_id = d.id " +
                "ORDER BY groups.id; ");
            if (dataset != null)
                DataGrid_gr.ItemsSource = dataset.Tables[0].DefaultView;

            dataset = worker.makeQuery(
                "SELECT marks.id                                    \"ID\", " +
                "mark                                               \"Оценка\", " +
                "passes                                             \"Пропуски\", " +
                "CONCAT(s.surname, ' ', s.name, ' ', s.middle_name) \"Студент\", " +
                "sj.title                                           \"Предмет\" " +
                "FROM marks " +
                        "JOIN students s on marks.student_id = s.id " +
                        "JOIN subjects sj on marks.subject_id = sj.id " +
                "ORDER BY marks.id; ");
            if (dataset != null)
                DataGrid_mr.ItemsSource = dataset.Tables[0].DefaultView;

            dataset = worker.makeQuery("SELECT id \"ID\", title \"Наименование\" FROM subjects;");
            if (dataset != null)
                DataGrid_sj.ItemsSource = dataset.Tables[0].DefaultView;

            dataset = worker.makeQuery("SELECT id \"ID\", title \"Наименование\", head \"Заведующий\", classroom \"Аудитория\" FROM departments;");
            if (dataset != null)
                DataGrid_dp.ItemsSource = dataset.Tables[0].DefaultView;

            dataset = worker.makeQuery(
                "SELECT mark_id     \"MARK_ID\", " +
                       "group_name  \"Группа\", " +
                       "surname     \"Фамилия\", " +
                       "name        \"Имя\", " +
                       "middle_name \"Отчество\", " +
                       "subj_title  \"Предмет\", " +
                       "mark        \"Оценка\", " +
                       "passes      \"Пропуски\" " +
                "FROM marks_view ORDER BY mark_id; ");
            if (dataset != null)
                DataGrid_mv.ItemsSource = dataset.Tables[0].DefaultView;
        }
        #endregion

        #region utils
        private void toggleTheme(object sender, RoutedEventArgs e)
        {
            var theme = paletteHelper.GetTheme();

            if (IsDarkTheme)
            {
                IsDarkTheme = false;
                theme.SetBaseTheme(Theme.Light);
            }
            else
            {
                IsDarkTheme = true;
                theme.SetBaseTheme(Theme.Dark);
            }
            paletteHelper.SetTheme(theme);
        }

        private bool confirmDelete() =>
            MessageBox.Show("Удалить выбранные строки?",
                            "Подтверждение удаления",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning)
            == MessageBoxResult.Yes ? true : false;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "ID"
                || e.Column.Header.ToString() == "MARK_ID")
                e.Column.Visibility = Visibility.Collapsed;
        }

        private void exitApp(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();
        #endregion
    }
}
