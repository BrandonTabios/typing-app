# typing-app

This is a personal project designed to help users improve their typing speed and accuracy. The application presents typing practice challenges by having users enter provided words. To improve user skills, it tracks performance in terms of Words Per Minute (WPM). The application also stores results in a database after each attempt

Video Showcase of Project: https://www.youtube.com/watch?v=nndMtiG_K7U
# Features

Typing Practice: Users can practice typing by entering provided given text
WPM Tracking: The app calculates and displays Words Per Minute (WPM) based on user input.
Database Integration: Uses SQLite to store WPM data after each attempt, allowing users to track their progress over time.
C# Development: The project is developed using C# and utilizes Windows Presentation Foundation (WPF) for the graphical user interface

# Running the Application

To run the application, simply click on the start.bat which runs the exe. After the application is run enter the provided text given. When the key you type is matching the provided key, the provided key will be highlighted in green. If the key is wrong then it would be highlighted in red. The key that you need to mimic will be highlighted in blue. After completing this for 20 seconds, the textbox will stop and allow you to see your final results which showcase your words per minute, total errors, and accuracy. This data will be stored in the database which can be seen from clicking the "Show Database Content" button.
