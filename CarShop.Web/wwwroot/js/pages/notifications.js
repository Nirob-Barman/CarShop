(function () {
    var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    var antiForgeryToken = tokenInput ? tokenInput.value : '';

    window.markRead = async function (id, link) {
        try {
            await fetch('/Notification/MarkRead', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': antiForgeryToken
                },
                body: 'id=' + id
            });
        } catch (e) { /* ignore — navigate anyway */ }

        if (link && link !== '') window.location.href = link;
        else window.location.reload();
    };
})();
