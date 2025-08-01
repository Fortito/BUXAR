const canvas = document.getElementById("gameCanvas");
const ctx = canvas.getContext("2d");
const levelInfo = document.getElementById("levelInfo");

const GRAVITY = 0.5;
const TILE_SIZE = 50;

let currentLevel = 0;
let levels = [
    [
        "00000000000000000000",
        "00000000000000200000",
        "00000000000000110000",
        "00000000000000000000",
        "00000000110000000000",
        "00000000000000000000",
        "00001100000000000000",
        "00000000000000000003",
        "00000000000000000000",
        "11110000111100011111",
    ],
    [
        "00000000000000000000",
        "00000000000000200000",
        "00000000000000110000",
        "00000000000000000000",
        "00000000110000000000",
        "00000000000000000000",
        "00001100000000000000",
        "00000000000000000403",
        "00000000000000000000",
        "11110000111100011111",
    ],
    [
        "00000000000000000000",
        "00000000000000000000",
        "00000000000000200000",
        "00000000000000110000",
        "00000000000000000000",
        "00000000110000000000",
        "00000000000000000000",
        "00001100000000000003",
        "00000000000000000000",
        "11110000111100011111",
    ]
];

let level = levels[currentLevel];
let player = {
    x: 100,
    y: 0,
    width: 40,
    height: 40,
    vx: 0,
    vy: 0,
    speed: 4,
    jump: -15,
    grounded: false,
    lives: 3
};

let keys = {};
let fallingPlatforms = [];
let traps = [];
let exitDoor = null;

// Initialize game elements
function initLevel() {
    fallingPlatforms = [];
    traps = [];
    exitDoor = null;

    for (let row = 0; row < level.length; row++) {
        for (let col = 0; col < level[row].length; col++) {
            const tile = level[row][col];
            const x = col * TILE_SIZE;
            const y = row * TILE_SIZE;

            if (tile === "2") {
                traps.push({ x, y, width: TILE_SIZE, height: TILE_SIZE });
            } else if (tile === "3") {
                exitDoor = { x, y, width: TILE_SIZE, height: TILE_SIZE };
            } else if (tile === "4") {
                fallingPlatforms.push({
                    x,
                    y,
                    width: TILE_SIZE,
                    height: TILE_SIZE,
                    col,
                    row,
                    fallTimer: 0,
                    falling: false,
                    originalY: y
                });
            }
        }
    }
    levelInfo.textContent = `Level: ${currentLevel + 1} | Lives: ${player.lives}`;
}

function drawRect(x, y, w, h, color) {
    ctx.fillStyle = color;
    ctx.fillRect(x, y, w, h);
}

function drawLevel() {
    // Draw background
    drawRect(0, 0, canvas.width, canvas.height, "#222");

    // Draw platforms
    for (let row = 0; row < level.length; row++) {
        for (let col = 0; col < level[row].length; col++) {
            const tile = level[row][col];
            const x = col * TILE_SIZE;
            const y = row * TILE_SIZE;

            if (tile === "1") {
                drawRect(x, y, TILE_SIZE, TILE_SIZE, "#555");
            }
        }
    }

    // Draw traps (spikes)
    traps.forEach(trap => {
        drawRect(trap.x, trap.y, trap.width, trap.height, "#e33");
        // Draw spike pattern
        ctx.fillStyle = "#f55";
        ctx.beginPath();
        ctx.moveTo(trap.x, trap.y + trap.height);
        ctx.lineTo(trap.x + trap.width / 2, trap.y);
        ctx.lineTo(trap.x + trap.width, trap.y + trap.height);
        ctx.fill();
    });

    // Draw exit door
    if (exitDoor) {
        drawRect(exitDoor.x, exitDoor.y, exitDoor.width, exitDoor.height, "#0f0");
        // Draw door details
        ctx.fillStyle = "#0c0";
        ctx.fillRect(exitDoor.x + 10, exitDoor.y + 5, 5, exitDoor.height - 10);
        ctx.fillRect(exitDoor.x + exitDoor.width - 15, exitDoor.y + 5, 5, exitDoor.height - 10);
    }

    // Draw falling platforms
    fallingPlatforms.forEach(fp => {
        if (!fp.falling) {
            drawRect(fp.x, fp.y, fp.width, fp.height, "#aaa");
            // Draw cracks
            ctx.strokeStyle = "#666";
            ctx.lineWidth = 2;
            ctx.beginPath();
            ctx.moveTo(fp.x + 15, fp.y + 10);
            ctx.lineTo(fp.x + 35, fp.y + 15);
            ctx.moveTo(fp.x + 10, fp.y + 25);
            ctx.lineTo(fp.x + 40, fp.y + 30);
            ctx.stroke();
        }
    });
}

function getTile(x, y) {
    const col = Math.floor(x / TILE_SIZE);
    const row = Math.floor(y / TILE_SIZE);
    if (row < 0 || row >= level.length || col < 0 || col >= level[0].length) return "0";
    return level[row][col];
}

function checkCollision(obj1, obj2) {
    return obj1.x < obj2.x + obj2.width &&
        obj1.x + obj1.width > obj2.x &&
        obj1.y < obj2.y + obj2.height &&
        obj1.y + obj1.height > obj2.y;
}

function updatePlayer() {
    // Horizontal movement
    if (keys["ArrowLeft"] || keys["a"]) player.vx = -player.speed;
    else if (keys["ArrowRight"] || keys["d"]) player.vx = player.speed;
    else player.vx = 0;

    // Jumping
    if ((keys["ArrowUp"] || keys["w"] || keys[" "]) && player.grounded) {
        player.vy = player.jump;
        player.grounded = false;
    }

    // Apply velocity
    player.x += player.vx;
    player.y += player.vy;
    player.vy += GRAVITY;

    // Check for collisions with traps
    traps.forEach(trap => {
        if (checkCollision(player, trap)) {
            playerHit();
            return;
        }
    });

    // Check for exit door collision
    if (exitDoor && checkCollision(player, exitDoor)) {
        nextLevel();
        return;
    }

    // Platform collision detection
    player.grounded = false;

    // Check if player is on any platform
    const bottomCenterX = player.x + player.width / 2;
    const bottomCenterY = player.y + player.height + 1;
    const tileUnder = getTile(bottomCenterX, bottomCenterY);

    if (tileUnder === "1" || tileUnder === "4") {
        player.vy = 0;
        player.grounded = true;
        player.y = Math.floor(bottomCenterY / TILE_SIZE) * TILE_SIZE - player.height;
    }

    // Check for falling platforms
    fallingPlatforms.forEach(fp => {
        if (!fp.falling &&
            player.x + player.width > fp.x &&
            player.x < fp.x + fp.width &&
            player.y + player.height >= fp.y - 5 &&
            player.y + player.height <= fp.y + 5) {

            fp.falling = true;
        }

        if (fp.falling) {
            fp.fallTimer++;
            if (fp.fallTimer > 30) {
                fp.y += 3; // Platform falls

                // If player is on the falling platform, fall with it
                if (player.x + player.width > fp.x &&
                    player.x < fp.x + fp.width &&
                    player.y + player.height >= fp.y - 5 &&
                    player.y + player.height <= fp.y + 5) {

                    player.y = fp.y - player.height;
                }
            }
        }
    });

    // Boundary checks
    if (player.x < 0) player.x = 0;
    if (player.x + player.width > canvas.width) player.x = canvas.width - player.width;

    // Fall out of screen
    if (player.y > canvas.height + 100) {
        playerHit();
    }
}

function playerHit() {
    player.lives--;
    levelInfo.textContent = `Level: ${currentLevel + 1} | Lives: ${player.lives}`;

    if (player.lives <= 0) {
        alert("Game Over! Restarting...");
        player.lives = 3;
        currentLevel = 0;
    }

    resetPlayer();
}

function resetPlayer() {
    player.x = 50;
    player.y = 0;
    player.vx = 0;
    player.vy = 0;
    initLevel();
}

function nextLevel() {
    currentLevel++;
    if (currentLevel >= levels.length) {
        alert("Congratulations! You completed all levels! 🎉");
        currentLevel = 0;
    }
    level = levels[currentLevel];
    resetPlayer();
}

function gameLoop() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    drawLevel();
    updatePlayer();

    // Draw player
    drawRect(player.x, player.y, player.width, player.height, "#0af");

    // Draw player eyes
    ctx.fillStyle = "#fff";
    ctx.fillRect(player.x + 10, player.y + 10, 8, 8);
    ctx.fillRect(player.x + 22, player.y + 10, 8, 8);

    requestAnimationFrame(gameLoop);
}

// Event listeners
document.addEventListener("keydown", e => {
    keys[e.key] = true;
    // Prevent spacebar from scrolling page
    if (e.key === " ") e.preventDefault();
});
document.addEventListener("keyup", e => keys[e.key] = false);

// Initialize and start game
initLevel();
gameLoop();