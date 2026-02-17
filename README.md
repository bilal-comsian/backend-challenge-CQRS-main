# backend-challenge

The project only has one entity and one controller. You need to leave the solution ready for future features, thinking in the scalability and future maintainability.

 ## Tasks
These are the main tasks to be completed (no need to do them in order, but it is recommended):
1. Reorganize the solution (if you think it's necessary) to improve the scalability and maintainability.
2. Currently the source of data is a json file (data/headphones.json), we want a database instead, so make the necessary things to convert that info to a database (SQL or NOSQL, whatever you prefer) and use the ORM you prefer
3. Add Create, Update and Delete endpoints for Headphones
4. Add new entity called "Keyboard":
	Properties:  `"Name", "Description", "Price", "ImageFileName", "Wireless", "Weight", "IsMechanical".`
	It shares some properties with existing "Headphones", what should we do?
5. Add CRUD endpoints for Keyboard

 ## Good to have
The idea is to see how the candidate works, these are some additional improvement that can be applied while you do the above tasks:
- What good practices and principles you apply? Naming conventions, SOLID, DRY, etc
- Use the architecture you prefer: vertical slices, onion, nLayer..
- How you organize the solution? Add the projects you think can help with the maintainability/scalability
- What design patterns you use? Like CQS/CQRS, Mediatr, other?
- How do you validate the api calls?
- Testing?

## Don't Forget!
You will need to share your screen, we want to see how you think please explain your process of thought while you code, you can do whatever you like (check other projects, google, AI, whatever). Use the tools you are comfortable with.
