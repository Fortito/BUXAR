const canvas = document.getElementById("gameCanvas");
const ctx = canvas.getContext("2d");

// Oyun parametrləri
const lanes = [100, 200, 300]; // Daha geniş zolaqlar
let gravity = 0.8;
let gameSpeed = 5;
let score = 0;
let highScore = localStorage.getItem('highScore') || 0;
let isGameOver = false;
let coins = [];
let obstacles = [];
let distance = 0;
let gameStarted = false;

// Oyunçu parametrləri
let player = {
    x: lanes[1],
    y: 400,
    width: 40,
    height: 80,
    color: "#FFD700",
    vy: 0,
    grounded: true,
    lane: 1,
    isSliding: false,
    slideTimer: 0
};

// Kontroller (klaviatura)
document.addEventListener("keydown", (e) => {
    if (isGameOver) return;

    if ((e.key === "ArrowLeft" || e.key === "a") && player.lane > 0) {
        player.lane--;
        player.x = lanes[player.lane];
    }
    if ((e.key === "ArrowRight" || e.key === "d") && player.lane < 2) {
        player.lane++;
        player.x = lanes[player.lane];
    }
    if ((e.key === " " || e.key === "ArrowUp" || e.key === "w") && player.grounded) {
        player.vy = -16;
        player.grounded = false;
    }
    if ((e.key === "ArrowDown" || e.key === "s") && player.grounded && !player.isSliding) {
        player.isSliding = true;
        player.slideTimer = 30;
        player.height = 40;
        player.y += 40;
    }
});

// Mobil üçün toxunma kontrolleri
let touchStartX = 0;
let touchStartY = 0;

canvas.addEventListener('touchstart', (e) => {
    touchStartX = e.touches[0].clientX;
    touchStartY = e.touches[0].clientY;

    if (isGameOver) {
        resetGame();
        e.preventDefault();
    } else if (!gameStarted) {
        gameStarted = true;
        e.preventDefault();
    }
});

canvas.addEventListener('touchmove', (e) => {
    if (isGameOver || !gameStarted) return;

    const touchEndX = e.touches[0].clientX;
    const diff = touchStartX - touchEndX;

    if (Math.abs(diff) > 50) {
        if (diff > 0 && player.lane < 2) {
            player.lane++;
            player.x = lanes[player.lane];
        } else if (diff < 0 && player.lane > 0) {
            player.lane--;
            player.x = lanes[player.lane];
        }
        touchStartX = touchEndX;
    }
});

canvas.addEventListener('touchend', (e) => {
    if (isGameOver || !gameStarted) return;

    if (player.grounded) {
        if (e.changedTouches[0].clientY < touchStartY - 30) {
            player.vy = -16;
            player.grounded = false;
        } else if (e.changedTouches[0].clientY > touchStartY + 30) {
            player.isSliding = true;
            player.slideTimer = 30;
            player.height = 40;
            player.y += 40;
        }
    }
});

// Desktop üçün kliklə başlatma
canvas.addEventListener('click', (e) => {
    if (isGameOver) {
        resetGame();
    } else if (!gameStarted) {
        gameStarted = true;
    }
});

// Maneələr
const obstacleTypes = [
    { width: 40, height: 60, color: "#c00" },
    { width: 60, height: 40, color: "#a00" },
    { width: 30, height: 80, color: "#800" }
];

function spawnObstacle() {
    if (Math.random() < 0.02) {
        let lane = Math.floor(Math.random() * 3);
        let type = Math.floor(Math.random() * obstacleTypes.length);
        obstacles.push({
            x: lanes[lane],
            y: -obstacleTypes[type].height,
            width: obstacleTypes[type].width,
            height: obstacleTypes[type].height,
            color: obstacleTypes[type].color
        });
    }

    if (Math.random() < 0.04) {
        let lane = Math.floor(Math.random() * 3);
        coins.push({
            x: lanes[lane] + 10,
            y: -20,
            size: 20,
            rotation: 0,
            spinSpeed: Math.random() * 0.1 + 0.05
        });
    }
}

function update() {
    if (isGameOver || !gameStarted) return;

    distance += gameSpeed / 10;
    score = Math.floor(distance) + (coins.length * 10);
    gameSpeed = 5 + Math.floor(distance / 1000);

    player.vy += gravity;
    player.y += player.vy;

    if (player.y > 400) {
        player.y = 400;
        player.vy = 0;
        player.grounded = true;
    }

    if (player.isSliding) {
        player.slideTimer--;
        if (player.slideTimer <= 0) {
            player.isSliding = false;
            player.height = 80;
            player.y -= 40;
        }
    }

    for (let obs of obstacles) {
        obs.y += gameSpeed;
        if (checkCollision(player, obs)) {
            gameOver();
            return;
        }
    }
    obstacles = obstacles.filter(obs => obs.y < canvas.height + 100);

    for (let coin of coins) {
        coin.y += gameSpeed;
        coin.rotation += coin.spinSpeed;
        if (Math.abs(player.x + 20 - coin.x) < 25 && Math.abs(player.y + 30 - coin.y) < 30) {
            score += 10;
            coins.splice(coins.indexOf(coin), 1);
        }
    }
    coins = coins.filter(c => c.y < canvas.height + 100);
}

function checkCollision(p, o) {
    return (
        p.x < o.x + o.width &&
        p.x + p.width > o.x &&
        p.y < o.y + o.height &&
        p.y + p.height > o.y
    );
}

function resetGame() {
    player = {
        x: lanes[1],
        y: 400,
        width: 40,
        height: 80,
        color: "#FFD700",
        vy: 0,
        grounded: true,
        lane: 1,
        isSliding: false,
        slideTimer: 0
    };

    obstacles = [];
    coins = [];
    distance = 0;
    score = 0;
    gameSpeed = 5;
    isGameOver = false;
    gameStarted = true;
}

function gameOver() {
    isGameOver = true;
    if (score > highScore) {
        highScore = score;
        localStorage.setItem('highScore', highScore);
    }
}

function drawBackground() {
    ctx.fillStyle = "#333";
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    ctx.fillStyle = "#555";
    for (let y = -50; y < canvas.height; y += 50) {
        for (let lane of lanes) {
            ctx.fillRect(lane - 5, (y + distance * 2) % canvas.height, 10, 30);
        }
    }

    ctx.fillStyle = "#444";
    ctx.fillRect(0, 0, 50, canvas.height);
    ctx.fillRect(canvas.width - 50, 0, 50, canvas.height);
}

function drawPlayer() {
    ctx.fillStyle = player.color;
    ctx.fillRect(player.x, player.y, player.width, player.height);

    ctx.fillStyle = "#000";
    ctx.fillRect(player.x + 10, player.y + 15, 5, 5);
    ctx.fillRect(player.x + 25, player.y + 15, 5, 5);
    ctx.fillRect(player.x + 10, player.y + 30, 20, 5);
}

function drawObstacles() {
    for (let obs of obstacles) {
        ctx.fillStyle = obs.color;
        ctx.fillRect(obs.x, obs.y, obs.width, obs.height);

        ctx.fillStyle = "#900";
        ctx.fillRect(obs.x + 5, obs.y + 5, obs.width - 10, obs.height - 10);
    }
}

function drawCoins() {
    for (let coin of coins) {
        ctx.save();
        ctx.translate(coin.x, coin.y);
        ctx.rotate(coin.rotation);

        ctx.beginPath();
        ctx.arc(0, 0, coin.size / 2, 0, Math.PI * 2);
        ctx.fillStyle = "gold";
        ctx.fill();

        ctx.beginPath();
        ctx.arc(0, 0, coin.size / 3, 0, Math.PI * 2);
        ctx.fillStyle = "yellow";
        ctx.fill();

        ctx.restore();
    }
}

function drawScore() {
    ctx.fillStyle = "#fff";
    ctx.font = "24px Arial";
    ctx.fillText("Xal: " + score, 20, 30);
    ctx.fillText("Rekord: " + highScore, 20, 60);
}

function drawGameOver() {
    if (!isGameOver) return;

    ctx.fillStyle = "rgba(0, 0, 0, 0.7)";
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    ctx.fillStyle = "#fff";
    ctx.font = "36px Arial";
    ctx.textAlign = "center";
    ctx.fillText("OYUN BİTTİ", canvas.width / 2, canvas.height / 2 - 40);
    ctx.fillText("Xal: " + score, canvas.width / 2, canvas.height / 2);
    ctx.fillText("Rekord: " + highScore, canvas.width / 2, canvas.height / 2 + 40);
    ctx.font = "24px Arial";
    ctx.fillText("Yenidən başlamaq üçün toxunun", canvas.width / 2, canvas.height / 2 + 80);
    ctx.textAlign = "left";
}

function drawStartScreen() {
    ctx.fillStyle = "rgba(0, 0, 0, 0.7)";
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    ctx.fillStyle = "#FFD700";
    ctx.font = "36px Arial";
    ctx.textAlign = "center";
    ctx.fillText("SUBWAY RUNNER", canvas.width / 2, canvas.height / 2 - 60);

    ctx.fillStyle = "#fff";
    ctx.font = "24px Arial";
    ctx.fillText("Başlamaq üçün toxunun", canvas.width / 2, canvas.height / 2);

    ctx.font = "18px Arial";
    ctx.fillText("↑ Tullanmaq | ↓ Sürüşmək", canvas.width / 2, canvas.height / 2 + 40);
    ctx.fillText("← → Zolaq dəyişmək", canvas.width / 2, canvas.height / 2 + 70);
    ctx.textAlign = "left";
}

function loop() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (gameStarted && !isGameOver) {
        spawnObstacle();
        update();
    }

    drawBackground();
    drawObstacles();
    drawCoins();
    drawPlayer();
    drawScore();
    drawGameOver();

    if (!gameStarted && !isGameOver) {
        drawStartScreen();
    }

    requestAnimationFrame(loop);
}

// Oyunu başlat
resetGame();
gameStarted = false;
loop();