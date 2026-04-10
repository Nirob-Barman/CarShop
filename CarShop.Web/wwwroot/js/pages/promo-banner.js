function copyPromoCode(el) {
    navigator.clipboard.writeText(el.textContent.trim()).then(function () {
        var orig = el.textContent;
        el.textContent = 'Copied!';
        setTimeout(function () { el.textContent = orig; }, 1500);
    });
}

(function () {
    var banner = document.getElementById('promoBanner');
    var total = banner ? parseInt(banner.dataset.count, 10) : 0;
    if (total <= 1) return;

    var current = 0;
    var timer = null;

    function show(idx) {
        document.querySelectorAll('.promo-slide').forEach(function (s, i) {
            s.classList.toggle('d-none', i !== idx);
        });
        document.querySelectorAll('.promo-dot').forEach(function (d, i) {
            d.style.background = i === idx ? '#fff' : 'rgba(255,255,255,.4)';
        });
        current = idx;
    }

    function next() { show((current + 1) % total); }
    function prev() { show((current - 1 + total) % total); }

    function startTimer() { timer = setInterval(next, 4000); }
    function resetTimer() { clearInterval(timer); startTimer(); }

    window.promoNext = function () { next(); resetTimer(); };
    window.promoPrev = function () { prev(); resetTimer(); };
    window.promoGoTo = function (i) { show(i); resetTimer(); };

    startTimer();
})();
