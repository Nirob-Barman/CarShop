var DESKTOP = 992;

function toggleSidebar() {
    if (window.innerWidth >= DESKTOP) {
        document.body.classList.toggle('sb-collapsed');
        try { localStorage.setItem('sb', document.body.classList.contains('sb-collapsed') ? '1' : '0'); } catch (e) { }
    } else {
        bootstrap.Offcanvas.getOrCreateInstance(document.getElementById('adminSidebar')).toggle();
    }
}

// Restore desktop collapsed state on load
try {
    if (window.innerWidth >= DESKTOP && localStorage.getItem('sb') === '1') {
        document.body.classList.add('sb-collapsed');
    }
} catch (e) { }
