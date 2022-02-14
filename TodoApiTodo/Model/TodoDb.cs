using Microsoft.EntityFrameworkCore;

namespace TodoApiTodo.Model
{
	public class TodoDb: DbContext
	{
		public TodoDb(DbContextOptions<TodoDb> options)
	  : base(options) { }

		public DbSet<Todo> Todos => Set<Todo>();
	}
}
