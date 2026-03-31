# Cybersecurity Awareness Bot
### PROG6221/w — Programming 2A | Part 1
**Student Number:** ST10472501

---

## Description
A command-line chatbot built in C# that educates South African citizens about cybersecurity threats. 
The bot covers topics like phishing, password safety, and safe browsing through a conversational interface.

---

## How to Run

**Requirements**
- Windows PC
- .NET 10 SDK installed — https://dotnet.microsoft.com/download/dotnet/10

**Steps**

1. Clone the repository:
```
   git clone https://github.com/st10472501Ydv/CyberAwarenessBot.git
```
2. Navigate to the project folder:
```
   cd CyberAwarenessBot/CyberAwarenessBot
```
3. Run the application:
```
   dotnet run
```

---

## Features
- Voice greeting plays on startup
- ASCII art logo displayed as header
- Asks for the user's name and personalises responses
- Responds to cybersecurity questions on passwords, phishing and safe browsing
- Handles invalid or empty inputs gracefully
- Coloured text and typing effect for a better user experience

---

## Topics Covered
- Password Safety
- Phishing Scams
- Safe Browsing

---

## GitHub Actions CI
The workflow builds the project automatically on every push to check for errors.

![CI Status](https://github.com/st10472501Ydv/CyberAwarenessBot/actions/workflows/ci.yml/badge.svg)

**CI Build Screenshot**

![CI Green](ci-screenshots/ci-screenshot1.png)
![CI Green](ci-screenshots/ci-screenshot2.png)

---

## Video Presentation
Link: [replace with your YouTube link]

---

## Project Structure
```
CyberAwarenessBot/
├── ci-screenshots/
│   ├── ci-screenshot1.png
│   └── ci-screenshots/
├── src/
│   ├── Services/
│   │   ├── VoiceGreeting.cs
│   │   └── ChatBot.cs
│   └── UI/
│       ├── AsciiArt.cs
│       └── ConsoleHelper.cs
├── assets/
│   └── greeting.wav
├── .github/
│   └── workflows/
│       └── ci.yml
├── Program.cs
└── README.md
```
