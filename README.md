# Loan Management App

This repository contains a full-stack Loan Management application with:
- **Frontend:** Angular
- **Backend:** .NET 6 Web API  
- **Database:** SQL Server  

---

## Prerequisites

Before you start, make sure you have installed:
- [Docker](https://www.docker.com/get-started)  
- [Docker Compose](https://docs.docker.com/compose/install/)

---

## Running the Application

1. Open a terminal and navigate to the root folder (where the `docker-compose.yml` is located).

2. Build and start the containers:

```bash
docker-compose up --build
```

This will:
- Build the Angular frontend and serve it via Nginx
- Build the .NET backend and run it in a container
- Start a SQL Server container with the required database

The first run might take a few minutes while Docker builds all images.

3. Access the application:
   - **Frontend (Angular):** http://localhost:4200
   - **Backend API (for testing/debugging):** http://localhost:5000/loan

---

## Docker Compose Setup

The setup consists of three main services:

### frontend
- Serves Angular files via Nginx
- Exposed on port 4200

### backend
- .NET 6 Web API
- Exposed on port 5000
- Connects to SQL Server for data

### sqldb
- SQL Server 2022 container
- Exposed on port 1433
- Default SA password set in environment variable

All services are connected through a Docker network called `loan-network`.

---

## Notes

- **CORS:** The backend allows requests from http://localhost:4200 for frontend API calls.
- **Database:** The backend will automatically apply migrations on startup.
- **Ports:** If ports 4200 or 5000 are already in use, adjust the `docker-compose.yml` file accordingly.

---

## Stopping the App

To stop and remove the containers:

```bash
docker-compose down
```