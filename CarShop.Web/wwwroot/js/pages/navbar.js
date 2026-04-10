(async function pollNotifications() {
    try {
        var res = await fetch('/Notification/UnreadCount');
        var data = await res.json();
        var badge = document.getElementById('notif-badge');
        if (badge) {
            if (data.count > 0) { badge.textContent = data.count; badge.style.display = ''; }
            else badge.style.display = 'none';
        }
    } catch (e) { }
    setTimeout(pollNotifications, 60000);
})();
