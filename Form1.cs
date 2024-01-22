using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace To_Do_List_App
{
    public partial class ToDoList : Form
    {
        public ToDoList()
        {
            InitializeComponent();
        }

        DataTable todoList = new DataTable();
        bool isEditing = false;
        private void ToDoList_Load(object sender, EventArgs e)
        {
            // Создание колонок
            todoList.Columns.Add("Название");
            todoList.Columns.Add("Описание");

            // Указание на источник
            toDoListView.DataSource = todoList;

            // Подключение к базе данных SQLite
            string connectionString = "Data Source=имя_файла.sqlite;Version=3;";
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();


            // Создание таблицы todoList
            string createTableQuery = "CREATE TABLE IF NOT EXISTS todoList (id INTEGER PRIMARY KEY AUTOINCREMENT, Название TEXT, Описание TEXT)";
            SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection);
            createTableCommand.ExecuteNonQuery();

            // Выполнение запроса к базе данных
            string sqlQuery = "SELECT * FROM todoList";
            SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string title = reader["Название"].ToString();
                string description = reader["Описание"].ToString();
                todoList.Rows.Add(title, description);
            }

            // Закрытие соединения с базой данных
            connection.Close();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            titleTextBox.Text = "";
            descriptionTextbox.Text = "";
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            isEditing = true;
            // Заполнение текстовых полей информацией из таблицы
            titleTextBox.Text = todoList.Rows[toDoListView.CurrentCell.RowIndex].ItemArray[0].ToString();
            descriptionTextbox.Text = todoList.Rows[toDoListView.CurrentCell.RowIndex].ItemArray[1].ToString();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                todoList.Rows[toDoListView.CurrentCell.RowIndex].Delete();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (isEditing)
            {
                todoList.Rows[toDoListView.CurrentCell.RowIndex]["Название"] = titleTextBox.Text;
                todoList.Rows[toDoListView.CurrentCell.RowIndex]["Описание"] = descriptionTextbox.Text;
            }
            else
            {
                todoList.Rows.Add(titleTextBox.Text, descriptionTextbox.Text);
            }
            // Очистка полей
            titleTextBox.Text = "";
            descriptionTextbox.Text = "";
            isEditing = false;
        }
    }
}