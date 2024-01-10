using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace To_Do_List
{
    public partial class ToDoList : Form
    {
        private SQLiteConnection connection;
        private DataTable todoList = new DataTable();
        private bool isEditing = false;
        private Timer timer = new Timer();

        public ToDoList()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeTodoList();
            InitializeTimer();
        }

        private void InitializeDatabase()
        {
            // Создание базы данных, если её не существует
            SQLiteConnection.CreateFile("todo.db");

            // Открытие соединения с базой данных
            connection = new SQLiteConnection("Data Source=todo.db;Version=3;");
            connection.Open();

            // Создание таблицы, если её не существует
            string sql = "CREATE TABLE IF NOT EXISTS TodoList (id INTEGER PRIMARY KEY AUTOINCREMENT, title TEXT, description TEXT, date TEXT)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private void InitializeTodoList()
        {
            // Загрузка записей из базы данных
            string sql = "SELECT * FROM TodoList";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, connection);
            adapter.Fill(todoList);

            // Указание источника данных
            ToDoListView.DataSource = todoList;
        }

        private void InitializeTimer()
        {
            // Установка интервала таймера на 1 минуту
            timer.Interval = 60000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Проверка каждой записи на наличие уведомления
            foreach (DataRow row in todoList.Rows)
            {
                DateTime date = DateTime.Parse(row["date"].ToString());
                if (date.Subtract(DateTime.Now).TotalMinutes <= 5)
                {
                    MessageBox.Show("Напоминание: row["title"]", "To-Do List", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }}}

        private void EditButton_Click(object sender, EventArgs e)
        {
            isEditing = true;
            // Наполнение текстовых полей информацией из таблицы
            label3.Text = todoList.Rows[ToDoListView.CurrentCell.RowIndex].ItemArray[1].ToString();
            label2.Text = todoList.Rows[ToDoListView.CurrentCell.RowIndex].ItemArray[2].ToString();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(todoList.Rows[ToDoListView.CurrentCell.RowIndex]["id"].ToString());
                string sql ="DELETE FROM TodoList WHERE id=id";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();

                todoList.Rows[ToDoListView.CurrentCell.RowIndex].Delete();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                int id = int.Parse(todoList.Rows[ToDoListView.CurrentCell.RowIndex]["id"].ToString());
                string sql = "UPDATE TodoList SET title='label3.Text', description='label2.Text' WHERE id=id";
                SQLiteCommand command = new SQLiteCommand(sql,connection);
                command.ExecuteNonQuery();

                todoList.Rows[ToDoListView.CurrentCell.RowIndex]["title"] = label3.Text;
                todoList.Rows[ToDoListView.CurrentCell.RowIndex]["description"] = label2.Text;
            }
            else
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string sql ="INSERT INTO TodoList (title, description, date) VALUES ('label3.Text', 'label2.Text', 'date')";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();

                todoList.Rows.Add(null, label3.Text, label2.Text, date);
            }

            // Очистка полей
            label3.Text = "";
            label2.Text = "";
            isEditing = false;
        }
    }
