using FluentMigrator;
using System.Data;

namespace TODO_API_net.Migrations
{
    [Migration(0001)]
    public class _001Seed_Data : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("Username").AsString(50).NotNullable()
                .WithColumn("FirstName").AsString(100).NotNullable()
                .WithColumn("LastName").AsString(100).NotNullable()
                .WithColumn("MiddleName").AsString(100).Nullable()
                .WithColumn("Email").AsString(100).NotNullable().Unique()
                .WithColumn("PasswordHash").AsString(255).NotNullable()
                .WithColumn("CreatedAt").AsCustom("TIMESTAMP").WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("Status").AsString(1).NotNullable().WithDefaultValue("A");

            Create.Table("todos")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("users", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Title").AsString(255).NotNullable()
                .WithColumn("Description").AsCustom("TEXT").Nullable()
                .WithColumn("IsCompleted").AsBoolean().WithDefaultValue(false)
                .WithColumn("Status").AsString(1).NotNullable().WithDefaultValue("A")
                .WithColumn("CreatedAt").AsCustom("TIMESTAMP").WithDefault(SystemMethods.CurrentDateTime);

            // Seed Data for Users
            Insert.IntoTable("users").Row(new { Username = "johndoe", FirstName = "John", LastName = "Doe", MiddleName = "Michael", Email = "johndoe@example.com", PasswordHash = "hashed_password_1" });
            Insert.IntoTable("users").Row(new { Username = "janedoe", FirstName = "Jane", LastName = "Doe", MiddleName = "null", Email = "janedoe@example.com", PasswordHash = "hashed_password_2" });
            Insert.IntoTable("users").Row(new { Username = "admin", FirstName = "Admin", LastName = "User", MiddleName = "null", Email = "admin@example.com", PasswordHash = "hashed_password_3" });

            // Seed Data for Todos
            Insert.IntoTable("todos").Row(new { UserId = 1, Title = "Buy groceries", Description = "Milk, Eggs, Bread, Butter", IsCompleted = false });
            Insert.IntoTable("todos").Row(new { UserId = 1, Title = "Read a book", Description = "Finish reading Clean Code", IsCompleted = false });
            Insert.IntoTable("todos").Row(new { UserId = 2, Title = "Go jogging", Description = "Run 5km in the morning", IsCompleted = true });
            Insert.IntoTable("todos").Row(new { UserId = 2, Title = "Learn SQL", Description = "Practice writing complex queries", IsCompleted = false });
            Insert.IntoTable("todos").Row(new { UserId = 3, Title = "System maintenance", Description = "Check server logs and update packages", IsCompleted = true });
        }

        public override void Down()
        {
            Delete.Table("todos");
            Delete.Table("users");
        }
    }
}
