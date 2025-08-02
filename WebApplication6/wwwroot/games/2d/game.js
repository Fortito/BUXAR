const canvas = document.getElementById("gameCanvas");
const ctx = canvas.getContext("2d");

const gravity = 0.5;

let player = {
    x: 100, y: 100,
    width: 40, height: 40,
    dx: 0, dy: 0,
    speed: 4,
    jumpForce: 12,
    grounded: false,
    sprite: new Image()
};
player.sprite.src = "player.png";

let keys = {}; 
const levels = [
    {
        platforms: [
            { x: 0, y: 500, w: 960, h: 40 },
            { x: 200, y: 420, w: 120, h: 20 },
            { x: 360, y: 350, w: 100, h: 20 }
        ],
        traps: [
            { x: 300, y: 495, w: 30, h: 5 }
        ],
        door: { x: 700, y: 460, w: 40, h: 40 }
    },
    {
        platforms: [
            { x: 0, y: 500, w: 960, h: 40 },
            { x: 250, y: 420, w: 100, h: 20 },
            { x: 400, y: 350, w: 100, h: 20 },
            { x: 550, y: 280, w: 100, h: 20 },
        ],
        traps: [
            { x: 500, y: 495, w: 30, h: 5 },
        ],
        door: { x: 650, y: 240, w: 40, h: 40 }
    },
    {
        platforms: [
            { x: 0, y: 500, w: 960, h: 40 },
            { x: 150, y: 430, w: 100, h: 20 },
            { x: 300, y: 360, w: 100, h: 20 },
            { x: 450, y: 290, w: 100, h: 20 },
            { x: 600, y: 220, w: 100, h: 20 },
        ],
        traps: [
            { x: 350, y: 495, w: 30, h: 5 },
        ],
        door: { x: 700, y: 180, w: 40, h: 60 }
    }
];

let currentLevel = 0;
let platforms = levels[currentLevel].platforms;
let traps = levels[currentLevel].traps;
let door = levels[currentLevel].door;

function loadLevel(index) {
    const level = levels[index];
    platforms = level.platforms;
    traps = level.traps;
    door = level.door;
    player.x = 100;
    player.y = 100;
    player.dy = 0;
}

document.addEventListener("keydown", e => keys[e.key.toLowerCase()] = true);
document.addEventListener("keyup", e => keys[e.key.toLowerCase()] = false);


function isColliding(a, b) {
    return a.x < b.x + b.w &&
        a.x + a.width > b.x &&
        a.y < b.y + b.h &&
        a.y + a.height > b.y;
}

function update() {
    player.dx = 0;

    if (keys["a"]) player.dx = -player.speed;
    if (keys["d"]) player.dx = player.speed;

    if (keys["w"] && player.grounded) {
        player.dy = -player.jumpForce;
        player.grounded = false;
    }


    platforms.forEach(p => {
        if (isColliding(player, p)) {
            if (player.dy > 0 && player.y + player.height - player.dy <= p.y) {
                player.y = p.y - player.height;
                player.dy = 0;
                player.grounded = true;
            }
        }
    });

    traps.forEach(t => {
        if (isColliding(player, t)) {
            alert("☠️ ");
            loadLevel(currentLevel); 
        }
    });

    if (isColliding(player, door)) {
        if (currentLevel < levels.length - 1) {
            currentLevel++;
            alert("🎉 ");
            loadLevel(currentLevel);
        } else {
            alert("🏆 ");
            location.reload(); 
        }
    }

    if (player.y > canvas.height) {
        alert("⬇️ .");
        loadLevel(currentLevel);
    }
}

function draw() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    ctx.fillStyle = "#654321";
    platforms.forEach(p => ctx.fillRect(p.x, p.y, p.w, p.h));

    ctx.fillStyle = "crimson";
    traps.forEach(t => ctx.fillRect(t.x, t.y, t.w, t.h));

    ctx.fillStyle = "green";
    ctx.fillRect(door.x, door.y, door.w, door.h);

    ctx.drawImage(player.sprite, player.x, player.y, player.width, player.height);
}

function gameLoop() {
    update();
    draw();
    requestAnimationFrame(gameLoop);
}

loadLevel(currentLevel);
gameLoop();
