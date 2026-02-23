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