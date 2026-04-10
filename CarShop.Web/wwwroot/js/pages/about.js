(function () {
    var counters = document.querySelectorAll('.stat-number[data-target]');
    if (!counters.length) return;

    var animated = false;

    function runCounters() {
        if (animated) return;
        animated = true;
        counters.forEach(function (el) {
            var target = parseInt(el.dataset.target, 10);
            if (!target) { el.textContent = '0'; return; }
            var duration = 1600;
            var frameRate = 16;
            var steps = duration / frameRate;
            var increment = Math.ceil(target / steps);
            var current = 0;
            var timer = setInterval(function () {
                current = Math.min(current + increment, target);
                el.textContent = current.toLocaleString();
                if (current >= target) clearInterval(timer);
            }, frameRate);
        });
    }

    var section = document.querySelector('.about-stats');
    if (!section) return;

    if ('IntersectionObserver' in window) {
        new IntersectionObserver(function (entries, obs) {
            if (entries[0].isIntersecting) {
                runCounters();
                obs.disconnect();
            }
        }, { threshold: 0.25 }).observe(section);
    } else {
        runCounters();
    }
})();
