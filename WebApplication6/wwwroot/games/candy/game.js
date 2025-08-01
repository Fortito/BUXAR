const board = document.getElementById("board");
const scoreDisplay = document.getElementById("score");

const width = 8;
const candies = [
    'url("/games/candy/Image/red.png")',
    'url("/games/candy/Image/yellow.png")',
    'url("/games/candy/Image/green.png")',
    'url("/games/candy/Image/orange.png")',
    'url("/games/candy/Image/purple.png")',
    'url("/games/candy/Image/blue.jpg")'
];




let squares = [];
let score = 0;
let draggedTile, replacedTile;

function createBoard() {
    for (let i = 0; i < width * width; i++) {
        const square = document.createElement("div");
        square.setAttribute("draggable", true);
        square.setAttribute("id", i);
        square.style.backgroundImage = randomCandy();
        square.classList.add("tile");
        board.appendChild(square);
        squares.push(square);
    }
}

function randomCandy() {
    return candies[Math.floor(Math.random() * candies.length)];
}

function dragEvents() {
    squares.forEach(square => {
        square.addEventListener("dragstart", dragStart);
        square.addEventListener("dragover", dragOver);
        square.addEventListener("drop", dragDrop);
        square.addEventListener("dragend", dragEnd);
    });
}

function dragStart(e) {
    draggedTile = this;
}

function dragOver(e) {
    e.preventDefault();
}

function dragDrop(e) {
    replacedTile = this;
    let draggedId = parseInt(draggedTile.id);
    let replacedId = parseInt(replacedTile.id);

    const validMoves = [
        draggedId - 1,
        draggedId + 1,
        draggedId - width,
        draggedId + width
    ];

    if (validMoves.includes(replacedId)) {
        let temp = draggedTile.style.backgroundImage;
        draggedTile.style.backgroundImage = replacedTile.style.backgroundImage;
        replacedTile.style.backgroundImage = temp;
    }
}

function dragEnd() {
    checkRow();
    checkColumn();
    dropCandies();
}

function checkRow() {
    for (let i = 0; i < width * width; i++) {
        let row = [i, i + 1, i + 2];
        let valid = !(i % width > width - 3);
        if (valid && row.every(idx => squares[idx].style.backgroundImage === squares[i].style.backgroundImage)) {
            row.forEach(idx => squares[idx].style.backgroundImage = '');
            score += 3;
            scoreDisplay.textContent = "Score: " + score;
        }
    }
}

function checkColumn() {
    for (let i = 0; i < width * (width - 2); i++) {
        let col = [i, i + width, i + width * 2];
        if (col.every(idx => squares[idx].style.backgroundImage === squares[i].style.backgroundImage)) {
            col.forEach(idx => squares[idx].style.backgroundImage = '');
            score += 3;
            scoreDisplay.textContent = "Score: " + score;
        }
    }
}

function dropCandies() {
    for (let i = width * (width - 1) - 1; i >= 0; i--) {
        if (squares[i + width] && squares[i + width].style.backgroundImage === '') {
            squares[i + width].style.backgroundImage = squares[i].style.backgroundImage;
            squares[i].style.backgroundImage = '';
        }
    }
    for (let i = 0; i < width; i++) {
        if (squares[i].style.backgroundImage === '') {
            squares[i].style.backgroundImage = randomCandy();
        }
    }
}

createBoard();
dragEvents();
setInterval(() => {
    checkRow();
    checkColumn();
    dropCandies();
}, 200);
