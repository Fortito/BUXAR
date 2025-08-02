
const canvas = document.getElementById("gameCanvas");
const ctx = canvas.getContext("2d");

let gravity = 0.5;
let friction = 0.8;
let levelIndex = 0;
let keys = {};

document.addEventListener("keydown", e => keys[e.key.toLowerCase()] = true);
document.addEventListener("keyup", e => keys[e.key.toLowerCase()] = false);

let player = {
    x: 50, y: 350,
    width: 30, height: 30,
    vx: 0, vy: 0,
    jumping: false,
    color: "#5cf4e5"
};

const levels = [
    {
        platforms: [
            {x: 0, y: 420, w: 800, h: 30},
            {x: 150, y: 350, w: 100, h: 10},
            {x: 300, y: 280, w: 100, h: 10},
            {x: 500, y: 220, w: 100, h: 10}
        ],
        traps: [
            {x: 400, y: 405, w: 20, h: 15},
            {x: 600, y: 405, w: 20, h: 15}
        ],
        door: {x: 700, y: 180, w: 40, h: 40}
    }
];

function resetPlayer() {
    player.x = 50;
    player.y = 350;
    player.vx = 0;
    player.vy = 0;
}

function update() {
    let level = levels[levelIndex];

    if (keys["a"]) player.vx -= 0.5;
    if (keys["d"]) player.vx += 0.5;
    if ((keys["w"] || keys[" "]) && !player.jumping) {
        player.vy = -12;
        player.jumping = true;
    }

    player.vy += gravity;
    player.x += player.vx;
    player.y += player.vy;
    player.vx *= friction;

    level.platforms.forEach(p => {
        if (player.x < p.x + p.w &&
            player.x + player.width > p.x &&
            player.y + player.height < p.y + 10 &&
            player.y + player.height + player.vy >= p.y) {
            player.y = p.y - player.height;
            player.vy = 0;
            player.jumping = false;
        }
    });

    level.traps.forEach(t => {
        if (player.x < t.x + t.w &&
            player.x + player.width > t.x &&
            player.y < t.y + t.h &&
            player.y + player.height > t.y) {
            alert("PLAY AGAIN");
            resetPlayer();
        }
    });

    const d = level.door;
    if (player.x < d.x + d.w &&
        player.x + player.width > d.x &&
        player.y < d.y + d.h &&
        player.y + player.height > d.y) {
        levelIndex++;
        if (levelIndex >= levels.length) {
            alert("great!");
            levelIndex = 0;
        }
        resetPlayer();
    }

    if (player.y > canvas.height) resetPlayer();
}

function draw() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    const level = levels[levelIndex];

    ctx.fillStyle = "#3c6e47";
    level.platforms.forEach(p => ctx.fillRect(p.x, p.y, p.w, p.h));

    ctx.fillStyle = "crimson";
    level.traps.forEach(t => ctx.fillRect(t.x, t.y, t.w, t.h));

    const d = level.door;
    ctx.fillStyle = "gold";
    ctx.fillRect(d.x, d.y, d.w, d.h);

    ctx.fillStyle = player.color;
    ctx.fillRect(player.x, player.y, player.width, player.height);

    ctx.fillStyle = "#fff";
    ctx.font = "16px Arial";
    ctx.fillText("Level: " + (levelIndex + 1), 20, 30);
}

function loop() {
    update();
    draw();
    requestAnimationFrame(loop);
}

resetPlayer();
loop();
