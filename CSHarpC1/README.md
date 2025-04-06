# Teacher Management System

An ASP.NET Core MVC application for managing teacher records in a school database.

## Features

- View all teachers in a list format
- View detailed information for individual teachers
- Add new teachers to the database
- Delete existing teachers from the database

## API Endpoints

- `GET /api/TeacherAPI` - Retrieves all teachers
- `GET /api/TeacherAPI/{id}` - Retrieves a specific teacher by ID
- `POST /api/TeacherAPI` - Adds a new teacher
- `DELETE /api/TeacherAPI/{id}` - Deletes a teacher by ID

## Web Pages

- `/TeacherPage/List` - Displays a list of all teachers
- `/TeacherPage/Show/{id}` - Displays details for a specific teacher
- `/TeacherPage/Create` - Form to add a new teacher
- `/TeacherPage/Delete/{id}` - Confirmation page for deleting a teacher

## Database

The application uses a MySQL database with the following schema for the teachers table:

```sql
CREATE TABLE `teachers` (
  `teacherid` int(20) NOT NULL,
  `teacherfname` varchar(255) DEFAULT NULL,
  `teacherlname` varchar(255) DEFAULT NULL,
  `employeenumber` varchar(255) DEFAULT NULL,
  `hiredate` datetime DEFAULT NULL,
  `salary` decimal(10,2) DEFAULT NULL
)
```

## Technologies Used

- ASP.NET Core MVC
- C#
- MySQL
- Entity Framework Core 