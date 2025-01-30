import express, { Request, Response } from 'express';

const app = express();
const port = 80;

app.get('/', (req: Request, res: Response) => {
  res.send('Hello from Express with TypeScript!');
});

app.listen(port, () => {
  console.log(`Server is listening on port ${port}`);
});