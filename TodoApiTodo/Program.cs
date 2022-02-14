using Microsoft.EntityFrameworkCore;
using TodoApiTodo.Model;

var builder = WebApplication.CreateBuilder(args);
//Đoạn mã được đánh dấu sau đây thêm ngữ cảnh cơ sở dữ liệu vào vùng chứa phụ thuộc (DI) và cho phép hiển thị các ngoại lệ liên quan đến cơ sở dữ liệu:
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/todoitems", async (TodoDb db) =>
	await db.Todos.ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
	await db.Todos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
	await db.Todos.FindAsync(id)
		is Todo todo
			? Results.Ok(todo)
			: Results.NotFound());
//Đoạn mã sau tạo một điểm cuối HTTP POST /todoitems để thêm dữ liệu vào cơ sở dữ liệu trong bộ nhớ:
app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
	db.Todos.Add(todo);
	await db.SaveChangesAsync();

	return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
	var todo = await db.Todos.FindAsync(id);

	if (todo is null) return Results.NotFound();

	todo.Name = inputTodo.Name;
	todo.IsComplete = inputTodo.IsComplete;

	await db.SaveChangesAsync();

	return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
	if (await db.Todos.FindAsync(id) is Todo todo)
	{
		db.Todos.Remove(todo);
		await db.SaveChangesAsync();
		return Results.Ok(todo);
	}

	return Results.NotFound();
});

app.Run();

//class Todo
//{
//	public int Id { get; set; }
//	public string? Name { get; set; }
//	public bool IsComplete { get; set; }
//}

//class TodoDb : DbContext
//{
//	public TodoDb(DbContextOptions<TodoDb> options)
//		: base(options) { }

//	public DbSet<Todo> Todos => Set<Todo>();
//}