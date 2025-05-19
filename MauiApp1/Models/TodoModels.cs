// Models/TodoModels.cs
using System.Collections.Generic;

namespace MauiApp1.Models
{
    public class TodoItem
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_description { get; set; }
        public string status { get; set; }
        public int user_id { get; set; }
        public string timemodified { get; set; }

        // Helpers for UI
        public string Title => item_name;
        public bool IsCompleted => status == "inactive";
    }

    public class TodoApiResponse
    {
        public int status { get; set; }
        public Dictionary<string, TodoItem> data { get; set; }
        public string message { get; set; }   // ? add this
        public string count { get; set; }
    }
}
