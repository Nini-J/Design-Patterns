# Book Catalog System

The Book Catalog system was designed to follow Clean Architecture principles, which emphasize separation of concerns and modular design. The application is organized into three main layers: Domain, Application (Use Cases), and Infrastructure.

## Team 2 Members:
- Mariam Rusishvili - Developer 1 . created the main structure of the code and base layer for the program.cs. Also created the main project file in Visual Studio 2022
- Nini Jakhaia - Unit tests  . Created the ReadMe.md. tested the final project's every function. Created unit tests and built and ran them on the code. contributed to the cleanness of the code
- Tamar Tateshvili - Project manager . divided the project tasks accordingly to the group, managed the work throughout the week. organized meetings and contributed to the infrastructure layer, and oversaw the overall cleannes of the code.
- Tamar Kvirikashvili - Developer 2  . added functional to the main project.cs. finished up the main program and fixed runtime errors.

## Clean Architecture Highlights

### 1. Domain Layer
Defines the core entity: Book, which contains properties like Id, Title, Author, Year, and Genre.  
Also includes the `IBookRepository` interface, which declares the operations supported by any book storage mechanism (e.g., add, retrieve, search, update, delete).


