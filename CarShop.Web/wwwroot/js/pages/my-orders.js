window.openCancelModal = function (orderId, carTitle) {
    document.getElementById('cancelOrderId').value = orderId;
    document.getElementById('orderCarTitle').textContent = carTitle;
    new bootstrap.Modal(document.getElementById('cancelOrderModal')).show();
};

(function () {
    var cfg = window._ordersConfig;
    if (!cfg || cfg.totalPages <= 1) return;

    var cards   = Array.from(document.querySelectorAll('.order-card'));
    var current = 1;

    function render(page) {
        current = page;
        var start = (page - 1) * cfg.pageSize;
        cards.forEach(function (c, i) {
            c.style.display = (i >= start && i < start + cfg.pageSize) ? '' : 'none';
        });

        var ul = document.getElementById('pagControls');
        ul.innerHTML = '';
        for (var p = 1; p <= cfg.totalPages; p++) {
            var li = document.createElement('li');
            li.className = 'page-item' + (p === page ? ' active' : '');
            li.innerHTML = '<button class="page-link" onclick="goPage(' + p + ')">' + p + '</button>';
            ul.appendChild(li);
        }
    }

    window.goPage = render;
    render(1);
})();
