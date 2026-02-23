window.gooseGame = {
    intervalId: null,
    start: function () {
        if (this.intervalId) clearInterval(this.intervalId);

        this.intervalId = setInterval(() => {
            const track = document.querySelector('.z-goose-track');
            const goose = document.querySelector('.z-running-goose');

            if (!track || !goose) return;

            const rect = goose.getBoundingClientRect();
            const trackRect = track.getBoundingClientRect();

            // Створюємо яйце
            const egg = document.createElement('div');
            egg.className = 'z-egg';
            egg.innerText = '🥚';

            // Визначаємо куди повернутий гусак через ComputedStyle
            const style = window.getComputedStyle(goose);
            const matrix = new WebKitCSSMatrix(style.transform);
            // Якщо matrix.a від'ємний — гусак розвернутий (scaleX(-1))
            const isFlipped = matrix.a < 0;

            // Корекція "точки виходу"
            const offset = isFlipped ? 45 : 5;

            egg.style.left = (rect.left - trackRect.left + offset) + 'px';
            egg.style.top = '30px';

            track.appendChild(egg);

            // Анімація розбиття
            setTimeout(() => {
                egg.innerText = '🍳';
                setTimeout(() => {
                    egg.style.opacity = '0';
                    egg.style.transition = 'opacity 0.3s';
                    setTimeout(() => egg.remove(), 300);
                }, 400);
            }, 750);
        }, 1000);
    },
    stop: function () {
        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
        }
    }
};


window.initGuardGoose = () => {
    const goose = document.getElementById('guard-goose');
    const banner = document.querySelector('.z-hero-card');
    if (!goose || !banner) return;

    let curX = 75, curY = 50;  // Починаємо з 75% по горизонталі та 50% по вертикалі
    let targetX = 75, targetY = 50;
    let isScared = false;

    document.addEventListener('mousemove', (e) => {
        const rect = banner.getBoundingClientRect();
        const mX = ((e.clientX - rect.left) / rect.width) * 100;
        const mY = ((e.clientY - rect.top) / rect.height) * 100;

        const dist = Math.sqrt(Math.pow(mX - curX, 2) + Math.pow(mY - curY, 2));

        if (dist < 7 && !isScared) {
            isScared = true;
            goose.classList.add('scared');
            targetX = Math.random() * 80 + 10;
            targetY = Math.random() * 50 + 20;
            setTimeout(() => { isScared = false; goose.classList.remove('scared'); }, 1000);
        }
        else if (dist < 25 && dist > 8 && !isScared) {
            goose.classList.add('aggressive');
            targetX += (mX - targetX) * 0.05;
            targetY += (mY - targetY) * 0.05;
        }
        else {
            goose.classList.remove('aggressive');
        }
        goose.style.transform = `scaleX(${mX > curX ? -1 : 1})`;
    });

    function loop() {
        curX += (targetX - curX) * 0.05;
        curY += (targetY - curY) * 0.05;
        goose.style.left = curX + '%';
        goose.style.top = curY + '%';
        requestAnimationFrame(loop);
    }
    loop();
};