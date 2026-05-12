## Description
Prictionary is a service that allows users to create their own dictionaries. Users can store translations and meanings for language units (a common name for both words and phrases) and then access them whenever they need. Language units can be grouped for easier navigation.

## Intended usage
App can have several use cases based on its configuration. Here is some of them: 
- Local hosting for personal usage. Creates a single user in the system, no registration is allowed.
- Local hosting for several users. A single administrator can create other users manually, but self-registration is not allowed.
- Public service. Has an administrator, but users can freely sign up.

## Plans
- Create client apps for the API. Add local storage feature for having access to dictionary without internet connection.
- Move from monolith to microservice architecture to simplify extensibility and make it more scalable (especially important if the app is used as a public service).
- Integrate an LLM or similar service to auto-generate translations.
- Allow inter-user interactions, such as words/groups sharing. This would be useful for language classes where a teacher can publish a vocabulary list and students can add it to their own dictionary.
- Add some tools to simplify learning and verify progress (such as [flashcards](https://en.wikipedia.org/wiki/Flashcard)).

## Tech Stack
- Core: .net 10, asp.net core.
- Data: entity framework core, PostgreSQL.
- Authentication: asp.net core identity, JWT.
- Tests: xUnit, Moq.

## Building & deploying
```bash
docker-compose up --build
```
