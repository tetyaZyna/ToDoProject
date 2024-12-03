Instructions to Run the Application
To run the application, you have two options for setting up the database connection:

Option 1: Using Docker Compose
 1. Create a .env file in the root of the project directory.
 2. Copy the contents from the example.env file and paste it into the newly created .env file and replace plaseholders by your data.
 3. Run the application using Docker Compose:
      docker-compose up --build
    
Option 2: Configuring Database Connection For Manual Start
 1. Open the appsettings.Development.json file.
 2. Locate the "Database" section and modify the ConnectionString as follows:
      "Database": {
        "ConnectionString": "Server=YOUR_SERVER;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;Port=YOUR_PORT;Database=YOUR_DATABASE"
      }
      Replace YOUR_SERVER, YOUR_USERNAME, YOUR_PASSWORD, YOUR_PORT, and YOUR_DATABASE with the appropriate values for your database.
 3. Run the application using your preferred method (e.g., via Visual Studio or dotnet run).
