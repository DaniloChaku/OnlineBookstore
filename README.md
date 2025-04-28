# OnlineBookstore

## Setting Up 

### Prerequisites

- Git installed on your machine
- Docker and Docker Compose installed

### Step-by-Step Guide

### 1. Clone the Repository

```bash
git clone https://github.com/DaniloChaku/OnlineBookstore.git
cd SimpleSocialMediaAPI
```

### 2. Set Up Environment Variables

```bash
cp .env.example .env
```

Open the `.env` file in your preferred text editor and fill in the required values.

### 3. Build and Run with Docker Compose

Once your environment variables are configured, you can start the application:

```bash
docker-compose up -d
```

The `-d` flag runs the containers in detached mode (in the background).

### 4. Access The Application

Once the containers are up and running, you can access the application at the configured port (the default is http://localhost:8080). You can interact with the application on http://localhost:8080/swagger

## Quality Analysis
Link: [https://sonarcloud.io/project/overview?id=danilochaku_simplesocialmediaapi.](https://sonarcloud.io/summary/overall?id=danilochaku_onlinebookstore&branch=main).

## AI Task Completion Feedback
- Was it easy to complete the task using **AI**?
  
  Yes, while I still had to do some things manually, AI was of great help.

- How long did task take you to complete? (*Please be honest, we need it to gather anonymized statistics*)
  
  4 hours.

- Was the code ready to run after generation? What did you have to change to make it usable?
  
  ChatGPT confused the names of the classes, using AppDbContext instead of ApplicationDbContext (I have memory turned off, so my previous conversations shouldn't have affected it). I also had to refactor the code to make it cleaner and suggest improvements to the AI, such as creating an interface for entities with an Id field or replacing magic literals with constants, etc.

- Which challenges did you face during completion of the task?
  
  The task wasn't challenging. With the help of AI and some manual refactoring, the process was smooth.
  
- Which specific prompts you learned as a good practice to complete the task?
  
  I didn't really learn any specific prompts. The pattern I used was task + [technologies] + [examples]
