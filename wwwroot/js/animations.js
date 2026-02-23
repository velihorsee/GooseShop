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
    const pupils = goose.querySelectorAll('.goose-pupil');
    const banner = document.querySelector('.z-hero-card');

    if (!goose || !banner) return;

    let curX = 80, curY = 50;
    let targetX = 80, targetY = 50;

    document.addEventListener('mousemove', (e) => {
        const rect = banner.getBoundingClientRect();
        const mX = ((e.clientX - rect.left) / rect.width) * 100;
        const mY = ((e.clientY - rect.top) / rect.height) * 100;

        // Рух обох зіниць
        const eyeDX = (e.clientX - (goose.offsetLeft + rect.left + 50)) / 40;
        const eyeDY = (e.clientY - (goose.offsetTop + rect.top + 30)) / 40;
        const moveX = Math.max(-8, Math.min(8, eyeDX));
        const moveY = Math.max(-5, Math.min(8, eyeDY));

        pupils.forEach(p => {
            p.style.transform = `translate(calc(-50% + ${moveX}px), calc(-50% + ${moveY}px))`;
        });

        // Дистанція для агресії
        const dist = Math.sqrt(Math.pow(mX - curX, 2) + Math.pow(mY - curY, 2));

        if (dist < 35) {
            goose.classList.add('aggressive');
            targetX += (mX - targetX) * 0.12;
            targetY += (mY - targetY) * 0.12;

            const dx = mX - curX;
            const dy = mY - curY;
            let angle = Math.atan2(dy, dx) * (180 / Math.PI) + 90;

            goose.style.setProperty('--neck-angle', `${Math.max(-45, Math.min(45, angle))}deg`);
            goose.style.setProperty('--neck-h', `${Math.min(180, (40 - dist) * 10 + 100)}px`);
        } else {
            goose.classList.remove('aggressive');
            goose.style.setProperty('--neck-h', '130px');
            targetX = 80; targetY = 50;
        }

        const flip = mX > curX ? -1 : 1;
        goose.dataset.flip = flip;
    });

    function loop() {
        curX += (targetX - curX) * 0.08;
        curY += (targetY - curY) * 0.08;
        const flip = goose.dataset.flip || 1;

        goose.style.left = curX + '%';
        goose.style.top = curY + '%';
        goose.style.transform = `translate(-50%, -50%) scaleX(${flip})`;

        requestAnimationFrame(loop);
    }
    loop();
};