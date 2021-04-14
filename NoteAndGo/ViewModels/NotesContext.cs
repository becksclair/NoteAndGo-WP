using System.Data.Linq;

namespace NoteAndGo.Data
{
    public class NotesContext : DataContext
    {
        public static string DBConnectionString = "Data Source=isostore:/Notes_db.sdf";
        public Table<Note> Notes;

        public NotesContext(string ConnectionString) : base(ConnectionString) { }
    }
}
