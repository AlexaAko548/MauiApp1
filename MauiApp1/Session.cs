using System.Collections.Generic;
using MauiApp1.Models;
using System.Linq;        // for ToList() and FirstOrDefault()


namespace MauiApp1
{
    public static class Session
    {
        public static int UserId { get; set; }

        // Add this:
        public static List<TodoItem> CurrentTasks { get; set; } = new List<TodoItem>();
    }
}
