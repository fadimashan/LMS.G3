# Lexicon LMS group 3

Exercise 16 in Lexicon's .NET Programming course: group exercise.

## Project Structure

The whole application consists of two major parts: the API application and the MVC Web application. The MVC application is further split into projects to implement a three-tier architecture: the Core project, the Data project and the Web project. The resulting organisation will look like this:

LMS.G3  
 ├─LMS.API              [API for LMS]  
 ├─LMS.ApiTests         [Tests for the API]  
 ├─LMS.Core             [belongs to MVC]  
 ├─LMS.Data             [belongs to MVC]  
 ├─LMS.Tests            [Tests of the MVC]  
 └─LMS.Web              [MVC main part]  

## MS SQL Server and SQLite DB

The application chooses whether to use SQL Server or SQLite based on the OS on which it is running:
If the underlying OS is Windows then MSSQL Server is chosen, otherwise the application opts for SQLite.
