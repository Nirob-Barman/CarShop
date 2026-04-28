(function () {
    var newPwd     = document.getElementById('newPwd');
    var confirmPwd = document.getElementById('confirmPwd');
    if (!newPwd || !confirmPwd) return;

    var strengthColors = ['#ef4444', '#f97316', '#eab308', '#3b82f6', '#22c55e'];
    var strengthTexts  = ['Very Weak', 'Weak', 'Fair', 'Strong', 'Very Strong'];

    newPwd.addEventListener('input', function () {
        var p    = this.value;
        var wrap = document.getElementById('strengthWrap');
        if (!p) { wrap.style.display = 'none'; return; }
        wrap.style.display = 'block';

        var score = [p.length >= 6, /\d/.test(p), /[a-z]/.test(p), /[A-Z]/.test(p), /\W/.test(p)].filter(Boolean).length;
        var bar   = document.getElementById('strengthBar');
        bar.style.width      = (score * 20) + '%';
        bar.style.background = strengthColors[score - 1] || strengthColors[0];
        document.getElementById('strengthLabel').textContent = strengthTexts[score - 1] || strengthTexts[0];
    });

    confirmPwd.addEventListener('input', function () {
        var match = this.value === newPwd.value;
        document.getElementById('matchError').style.display = match ? 'none' : 'block';
    });
})();
