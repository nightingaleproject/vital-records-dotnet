"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const app = (0, express_1.default)();
const port = 443;
app.get('/', (req, res) => {
    res.send('Hello from Express with TypeScript!');
});
app.listen(port, () => {
    console.log(`Server is listening on port ${port}`);
});
