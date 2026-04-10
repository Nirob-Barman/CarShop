(function () {
    var d = window._dashboardData;
    if (!d) return;
    new Chart(document.getElementById('ordersChart'), {
        type: 'bar',
        data: { labels: d.labels, datasets: [{ label: 'Orders', data: d.counts, backgroundColor: '#0d6efd' }] },
        options: { responsive: true, plugins: { legend: { display: false } } }
    });
})();
